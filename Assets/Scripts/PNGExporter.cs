using UnityEngine;
using System.Collections;
using System.IO;

public class PNGExporter : MonoBehaviour
{

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public enum ImageExtension { PNG, JPG };

    public static bool CacheContainsTheFile(string filePath)
    {
        return File.Exists(filePath);
    }

    public static bool CacheContainsTheFolder(string folderPath)
    {
        return Directory.Exists(folderPath);
    }

    public static string CreateFolder(string path)
    {
        try
        {
            // Determine whether the directory exists.
            if (Directory.Exists(path))
                Debug.Log("Folder already exists");
            else
                Directory.CreateDirectory(path);

            return path;
        }
        catch (System.Exception e)
        {
            Debug.Log("The process failed : " + e.ToString());
        }

        return "";
    }

    public void SaveTexture2Folder(string folderPath, string filename, Texture2D texture)
    {
        if (Directory.Exists(folderPath))
        {
            byte[] bytes;
            bytes = texture.EncodeToPNG();
            File.WriteAllBytes(folderPath + "/" + filename + ".png", bytes);
        }
        else
            Debug.LogError("Folder doesn't exist : " + folderPath);
    }

}
