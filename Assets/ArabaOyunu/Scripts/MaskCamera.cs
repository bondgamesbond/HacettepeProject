using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MaskCamera : MonoBehaviour
{
    public DragDetectorAraba dragDetector { get; set; }

    public Recognizer recognizer;

    // RenderTexture to read.
    public RenderTexture renderTexture;

    [HideInInspector]
    public bool firstTextureApplyPassed;

    private Texture2D tex;
    private bool canRead;

    public GameObject objectToErase;
    public Material EraserMaterial;
    private bool firstFrame;
    private Vector2? newHolePosition;
    private Vector2 v;
    private float restrictRatio;
    private DragDetectorAraba.DragTypeAraba levelDragType;

    private bool infoHasFeedbackText;

    private void CutHole(Vector2 imageSize, Vector2 imageLocalPosition, Vector2 mousePosition)
    {
        switch (GameManagerAraba.Instance.level)
        {
            case 0:
                levelDragType = DragDetectorAraba.DragTypeAraba.Horizontal;
                break;
            case 1:
                levelDragType = DragDetectorAraba.DragTypeAraba.Vertical;
                break;
            case 2:
                levelDragType = DragDetectorAraba.DragTypeAraba.Circular;
                break;
            case 3:
                levelDragType = DragDetectorAraba.DragTypeAraba.AllMotions;
                break;
            default:
                levelDragType = DragDetectorAraba.DragTypeAraba.None;
                break;
        }

        if (levelDragType == dragDetector.currentDragType)
        {
            int difficulty = RestrictionMap.findDifficulty(v);
            //print(difficulty);
        
            switch (difficulty)
            {
                case 1:
                    restrictRatio = 0.5f;
                    break;
                case 2:
                    restrictRatio = 0.33f;
                    break;
                case 3:
                    restrictRatio = 0.5f;
                    break;
                default:
                    restrictRatio = 1.0f;
                    break;
            }

            if (infoHasFeedbackText)
            {
                Text feedback = GameManagerAraba.Instance.feedbackText;

                switch (GameManagerAraba.Instance.level)
                {
                    case 0:
                        feedback.text = GameManagerAraba.Instance.wrongFeedbackTexts[0];
                        break;
                    case 1:
                        feedback.text = GameManagerAraba.Instance.wrongFeedbackTexts[1];
                        break;
                    case 2:
                        feedback.text = GameManagerAraba.Instance.wrongFeedbackTexts[2];
                        break;
                    case 3:
                        //feedback.text = "Herhangi bir hareketle aracý kurulamaya devam ediniz!";
                        feedback.text = GameManagerAraba.Instance.wrongFeedbackTexts[3];
                        break;
                    default:
                        feedback.text = "Herhangi bir hareketle araca cila atmaya devam ediniz!";
                        break;
                }
                infoHasFeedbackText = false;
            }

            dragDetector.lastRightDragTypeTime = Time.time;

            Rect textureRect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
            Rect positionRect = new Rect(
                (imageLocalPosition.x - 0.5f * EraserMaterial.mainTexture.width) / imageSize.x,
                (imageLocalPosition.y - 0.5f * EraserMaterial.mainTexture.height) / imageSize.y,
                EraserMaterial.mainTexture.width / imageSize.x,
                EraserMaterial.mainTexture.height / imageSize.y
            );
            GL.PushMatrix();
            GL.LoadOrtho();
            if (EraserMaterial.color.a <= 2.5f)
            {
                EraserMaterial.color = new Color(EraserMaterial.color.r, EraserMaterial.color.g, EraserMaterial.color.b, restrictRatio + tex.GetPixel((int)mousePosition.x, (int)mousePosition.y).a);
            }
            for (int i = 0; i < EraserMaterial.passCount; i++)
            {
                EraserMaterial.SetPass(i);
                GL.Begin(GL.QUADS);
                GL.Color(new Color(0.1f, 0.1f, 0.1f, 1.0f));
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
		//else
		//{
		//	Text feedback = GameManagerAraba.Instance.feedbackText;

		//	switch (GameManagerAraba.Instance.level)
		//	{
		//		case 0:
		//			feedback.text = GameManagerAraba.Instance.feedbackTexts[0];
		//			break;
		//		case 1:
		//			feedback.text = GameManagerAraba.Instance.feedbackTexts[1];
		//			break;
		//		case 2:
		//			feedback.text = GameManagerAraba.Instance.feedbackTexts[2];
		//			break;
		//		case 3:
  //                  //feedback.text = "Lütfen istediðiniz bir hareketle aracý kurulayýnýz!";
  //                  feedback.text = GameManagerAraba.Instance.feedbackTexts[3];
  //                  break;
		//		default:
		//			feedback.text = "Lütfen istediðiniz bir hareketle araca cila atýnýz!";
		//			break;
		//	}

		//}
    }

    public void Awake()
    {
        EraserMaterial.color = new Color(1.0f, 1.0f, 1.0f, 0.25f);
    }

    public void Start()
    {
        firstFrame = true;

        infoHasFeedbackText = true;

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
            v = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //Rect worldRect = new Rect(-9.0f, -5.0f, 18.0f, 10.0f);
            Rect worldRect = new Rect(-9.6f, -5.4f, 19.2f, 10.8f);
            if (worldRect.Contains(v))
            {
                newHolePosition =
                    new Vector2(1920 * (v.x - worldRect.xMin) / worldRect.width,
                                1080 * (v.y - worldRect.yMin) / worldRect.height);
                dragDetector.isOnCar = true;
            }
        }
        // Player stops touching to screen.
        //if (Input.GetMouseButtonUp(0))
        //{
        //    CheckPixels();
        //}

        if (!infoHasFeedbackText && dragDetector.lastRightDragTypeTime + GameManagerAraba.Instance.textCheckInterval < Time.time)
        {
            if (levelDragType != dragDetector.currentDragType)
            {
                switch (GameManagerAraba.Instance.level)
                {
                    case 0:
                        GameManagerAraba.Instance.feedbackText.text = GameManagerAraba.Instance.feedbackTexts[0];
                        break;
                    case 1:
                        GameManagerAraba.Instance.feedbackText.text = GameManagerAraba.Instance.feedbackTexts[1];
                        break;
                    case 2:
                        GameManagerAraba.Instance.feedbackText.text = GameManagerAraba.Instance.feedbackTexts[2];
                        break;
                    case 3:
                        GameManagerAraba.Instance.feedbackText.text = GameManagerAraba.Instance.feedbackTexts[3];
                        break;
                    default:
                        GameManagerAraba.Instance.feedbackText.text = "Lütfen istediðiniz bir hareketle araca cila atýnýz!";
                        break;
                }
            }
            infoHasFeedbackText = true;
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
            CutHole(new Vector2(1920.0f, 1080.0f), newHolePosition.Value, Input.mousePosition);

        if (canRead)
        {
            // Read the pixels from the render texture
            tex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            tex.Apply();
            firstTextureApplyPassed = true;
            canRead = false;
        }
    }

    void MakeReadable()
    {
        canRead = true;
    }

    public void StartCalculatingTotalAlpha()
    {
        StartCoroutine("CalculateTotalAlpha");
    }

    float total;
    IEnumerator CalculateTotalAlpha()
    {
        total = 1920 * 1080;

        // Control pixels
        for (int x = 0; x < 1920; x++)
        {
            for (int y = 0; y < 1080; y++)
            {
                total -= tex.GetPixel(x, y).a;
            }

            if (x % 100 == 0)
            {
                yield return new WaitForSeconds(0.01f);
            }
        }

        GameManagerAraba.Instance.currentTotalAlpha = total;
        GameManagerAraba.Instance.calculating = false;
        GameManagerAraba.Instance.totalAlphaCalculated = true;
    }

    public void StopCalculatingTotalAlpha()
    {
        StopAllCoroutines();
    }
}
