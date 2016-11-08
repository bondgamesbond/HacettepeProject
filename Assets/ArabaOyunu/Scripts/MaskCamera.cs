using UnityEngine;
using System.Collections;

public class MaskCamera : MonoBehaviour
{
    // RenderTexture to read.
    public RenderTexture renderTexture;

    private Texture2D tex;
    private bool canRead;

    public GameObject dust;
    public Material EraserMaterial;
    private bool firstFrame;
    private Vector2? newHolePosition;

    private void CutHole(Vector2 imageSize, Vector2 imageLocalPosition)
    {
        float restrictRatio = 1;

        Rect textureRect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
        Rect positionRect = new Rect(
            (imageLocalPosition.x - 0.5f * EraserMaterial.mainTexture.width) / imageSize.x,
            (imageLocalPosition.y - 0.5f * EraserMaterial.mainTexture.height) / imageSize.y,
            EraserMaterial.mainTexture.width / imageSize.x,
            EraserMaterial.mainTexture.height / imageSize.y
        );
        GL.PushMatrix();
        GL.LoadOrtho();
        if (EraserMaterial.color.a < 1)
        {
            EraserMaterial.color = new Color(EraserMaterial.color.r, EraserMaterial.color.g, EraserMaterial.color.b, restrictRatio + tex.GetPixel(960, 540).a);
        }
        for (int i = 0; i < EraserMaterial.passCount; i++)
        {
            EraserMaterial.SetPass(i);
            GL.Begin(GL.QUADS);
            GL.Color(new Color(0.1f, 0.1f, 0.1f, 1.0f));
            print(EraserMaterial.color.a);
            GL.TexCoord2(textureRect.xMin, textureRect.yMax);
            GL.Vertex3(positionRect.xMin, positionRect.yMax, 0.0f);
            GL.TexCoord2(textureRect.xMax, textureRect.yMax);
            GL.Vertex3(positionRect.xMax, positionRect.yMax, 0.0f);
            GL.TexCoord2(textureRect.xMax, textureRect.yMin);
            GL.Vertex3(positionRect.xMax, positionRect.yMin, 0.0f);
            GL.TexCoord2(textureRect.xMin, textureRect.yMin);
            GL.Vertex3(positionRect.xMin, positionRect.yMin, 0.0f);
            GL.End();
        }
        GL.PopMatrix();
    }

    public void Awake()
    {
        EraserMaterial.color = new Color(1.0f, 1.0f, 1.0f, 0.25f);
    }

    public void Start()
    {
        firstFrame = true;

        // Create Texture2D with the screen width and height.
        tex = new Texture2D(1920, 1080);

        // Make render texture pixels readable every second.
        InvokeRepeating("MakeReadable", 1, 1);
    }

    public void Update()
    {
        newHolePosition = null;
        if (Input.GetMouseButton(0))
        {
            Vector2 v = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Rect worldRect = new Rect(-9.0f, -5.0f, 18.0f, 10.0f);
            if (worldRect.Contains(v))
                newHolePosition = 
                    new Vector2(1920 * (v.x - worldRect.xMin) / worldRect.width,
                                1080 * (v.y - worldRect.yMin) / worldRect.height);
        }
        // Player stops touching to screen.
        if (Input.GetMouseButtonUp(0))
        {
            CheckPixels();
        }
    }

	public void OnPostRender()
	{
	    if (firstFrame)
	    {
	        firstFrame = false;
            GL.Clear(false, true, new Color(0.0f, 0.0f, 0.0f, 0.0f));
	    }
        if (newHolePosition != null)
            CutHole(new Vector2(1920.0f, 1080.0f), newHolePosition.Value);

        if (canRead)
        {
            // Read the pixels from the render texture
            tex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            tex.Apply();
            canRead = false;
        }
    }

    void MakeReadable()
    {
        canRead = true;
    }

    void CheckPixels()
    {
        float total = 1920 * 1080;

        // Control pixels
        for (int x = 0; x < 1920; x++)
        {
            for (int y = 0; y < 1080; y++)
            {
                total -= tex.GetPixel(x, y).a;
            }
        }
        
        if (total < 10000)
        {
            GameManagerAraba.Instance.ChangeToNextMaskCamera();
        }
    }
}
