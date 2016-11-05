using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
public class AnimateGif : MonoBehaviour
{
	public List<Sprite> spriteList = new List<Sprite>();

    public Text title;

	public float framesPerSecond = 0.5f;

	private SpriteRenderer sRenderer;
    private float timer;

    List<Texture2D> textures = new List<Texture2D>();
	private void Awake()
	{
		// Get required components
		sRenderer = GetComponent<SpriteRenderer>();
        loadGifImages();
        createSprites();
	}
    private void createSprites()
    {
        for (int i = 0; i < textures.Count; i++)
        {
            spriteList.Add(Sprite.Create(textures[i],new Rect(0,0,textures[i].width, textures[i].height), new Vector2(0.5f, 0.5f)));
        }
      
    }
    private void loadGifImages()
    {
        List<string> textureNames = new List<string>();
        DirectoryInfo dir = new DirectoryInfo(Application.streamingAssetsPath + "/Maps");
        FileInfo[] info = dir.GetFiles("*.*");
        foreach (FileInfo fi in info)
        {
            string fileExtension = fi.Extension;
            if (fileExtension != ".meta")
            {
                string imageName = Path.GetFileName(fi.Name);
                string textureName = Path.GetFileNameWithoutExtension(fi.Name);
                WWW www = new WWW("file://" + Application.streamingAssetsPath + "/Maps/" + imageName);
                textureNames.Add(textureName);

                if (!string.IsNullOrEmpty(www.error))
                {
                    Debug.Log(www.error);
                }
                else
                {
                    www.texture.name = textureName;
                    textures.Add(www.texture);
                }
            }
        }

       
    }
    
    private void OnEnable()
    {
        timer = 0;
    }

	private void Update()
	{
        // Find the next index for the gif
        timer += Time.deltaTime;
		int index = (int)(timer * framesPerSecond) % spriteList.Count;
        title.text = "" + (index + 1);
		sRenderer.sprite = spriteList[index];
	}
}
