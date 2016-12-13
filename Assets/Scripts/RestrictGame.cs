﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO;

public class RestrictGame : MonoBehaviour
{
    bool isFirstTouch, timeRunning, isLoadingSceneActive, isTouchesActive, isGameOver, loadingFlag;

    public Transform checkPoints, bestBaloon, mapList;

    public GameObject finishMenu, loadingMenu, saveMenu, mapMenu;

    public Text questionText, mapTimeText;

    GameObject activeBaloon, tempBaloon;

    public List<GameObject> usableCheckPointsList = new List<GameObject>();

    Vector2 bestBaloonPosition;

    public float[] checkPointTimes;

    public float time;

    float loadingTimer, loadingWaitTime, redAreaCount = 0;

    int formulaPower = 2, mapIndex = 0;

    public int pixelRange = 4;

    Collider2D tempHitCollider;

    Texture2D savingTexture;

    public GameObject difficultyMap;

    PNGExporter pngSaver;

    public AudioSource pop1, pop2, pop3;

    void Start()
    {
        checkPointTimes = new float[checkPoints.childCount];
        for (int i = 0; i < checkPoints.childCount; i++)
            usableCheckPointsList.Add(checkPoints.GetChild(i).gameObject);
        isFirstTouch = true;
        pngSaver = new PNGExporter();
        isTouchesActive = true;
    }

    void Update()
    {
        if (isTouchesActive && !isGameOver)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 rayPos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
                RaycastHit2D hit = Physics2D.Raycast(rayPos, Vector2.zero, 0f);

                if (hit)
                {
                    tempHitCollider = hit.transform.GetComponent<Collider2D>();
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                Vector2 rayPos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
                RaycastHit2D hit = Physics2D.Raycast(rayPos, Vector2.zero, 0f);

                if (!isFirstTouch)
                {
                    if (hit)
                    {
                        if (tempHitCollider == hit.transform.GetComponent<Collider2D>())
                        {
                            if (hit.transform.name == "baloonTheBest")
                            {
                                if (bestBaloon != null)
                                    timeRunning = true;
                                getNextBaloon(false, false);
                            }
                            else if (hit.transform.name == "baloon")
                            {
                                getNextBaloon(false, true);
                            }
                        }
                    }
                    else
                    {
                        if(bestBaloon.gameObject.activeSelf)
                        {
                            if (bestBaloon != null)
                                timeRunning = true;
                            getNextBaloon(true, false);
                        }
                        else
                        {
                            getNextBaloon(true, true);
                        }
                    }
                }
                else
                {
                    questionText.text = "Şimdi Balonlara Dokunarak Patlatın!";
                    bestBaloon.position = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 0);
                    bestBaloon.gameObject.SetActive(true);
                    bestBaloon.GetComponent<SkeletonAnimation>().AnimationName = "pop";
                    bestBaloonPosition = bestBaloon.position;
                    activeBaloon = bestBaloon.gameObject;
                    isFirstTouch = false;
                }
            }
        }
        if (timeRunning)
            time += Time.deltaTime;
        if (isLoadingSceneActive)
        {
            loadingTimer += Time.deltaTime;
            if (loadingTimer >= 1.0f && !loadingFlag)
            {
                loadingFlag = true;
                createDifficultyMap();
            }
            if (loadingTimer >= loadingWaitTime)
            {
                loadingMenu.SetActive(false);
                isTouchesActive = true;
                saveMenu.SetActive(true);
                questionText.text = "Testi Onaylıyor Musunuz?";
                questionText.gameObject.SetActive(true);
                isLoadingSceneActive = false;
            }
        }
    }
    void getNextBaloon(bool isFail, bool isBestBaloon)
    {
        if (isBestBaloon)
        {
            checkPointTimes[activeBaloon.transform.GetSiblingIndex()] += time;
            time = 0;
        }
        if (questionText.gameObject.activeSelf)
            questionText.gameObject.SetActive(false);
        if (!isFail)
        {
            activeBaloon.GetComponent<SkeletonAnimation>().AnimationName = "boom";
            activeBaloon.GetComponent<Baloon>().isBoomed = true;
            activeBaloon.GetComponent<BoxCollider2D>().enabled = false;
            usableCheckPointsList.Remove(activeBaloon);
            int random = Random.Range(0, 2);
            if (random == 0)
                pop1.Play();
            else if (random == 1)
                pop2.Play();
            else if (random == 2)
                pop3.Play();
        }
        else
        {
            activeBaloon.SetActive(false);
        }
        if (!isBestBaloon)
        {
            if (usableCheckPointsList.Count > 0)
            {
                tempBaloon = usableCheckPointsList[Random.Range(0, usableCheckPointsList.Count)];
                if (usableCheckPointsList.Count > 1)
                {
                    while (tempBaloon == activeBaloon)
                    {
                        tempBaloon = usableCheckPointsList[Random.Range(0, usableCheckPointsList.Count)];
                    }
                }
                activeBaloon = tempBaloon;
                activeBaloon.SetActive(true);
                activeBaloon.GetComponent<SkeletonAnimation>().AnimationName = "pop";
            }
            else
            {
                if (!isGameOver)
                {
                    getLoadingMenu(3, "Erişim Haritası Oluşturuluyor...");
                    timeRunning = false;
                    isGameOver = true;
                    bestBaloon.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            bestBaloon.gameObject.SetActive(true);
            bestBaloon.GetComponent<SkeletonAnimation>().AnimationName = "pop";
            activeBaloon = bestBaloon.gameObject;
            activeBaloon.GetComponent<BoxCollider2D>().enabled = true;
            time = 0;
            timeRunning = false;
        }
    }

    void createDifficultyMap()
    {
        Texture2D mapTexture = new Texture2D(1600, 900);
        difficultyMap.GetComponent<Renderer>().material.mainTexture = mapTexture;
        float[,] difficultyValues = new float[mapTexture.width / pixelRange, mapTexture.height / pixelRange];
        int[][] difficultyAreaValues = new int[mapTexture.height / pixelRange][];
        Color[,] colorValues = new Color[mapTexture.width / pixelRange, mapTexture.height / pixelRange];
        float xMulti = 1, xMultiTotal = 0, subTotal = 0, maxTime = 0;
        float[] distances = new float[checkPointTimes.Length];
        Vector3[] checkPointPositions = new Vector3[checkPointTimes.Length];

        for (int i = 0; i < mapTexture.height / pixelRange; i++)
            difficultyAreaValues[i] = new int[mapTexture.width / pixelRange];

        for (int i = 0; i < checkPointTimes.Length; i++)
        {
            if (checkPointTimes[i] > maxTime)
                maxTime = checkPointTimes[i];
            checkPointPositions[i] = new Vector3((Camera.main.WorldToScreenPoint(checkPoints.GetChild(i).position).x - Camera.main.WorldToScreenPoint(checkPoints.GetChild(0).position).x) * 2.8f, (Camera.main.WorldToScreenPoint(checkPoints.GetChild(i).position).y - Camera.main.WorldToScreenPoint(checkPoints.GetChild(0).position).y) * 2.8f, 0);
            checkPointPositions[i] = checkPointPositions[i];
        }

        for (int i = 0; i < mapTexture.width; i += pixelRange)
        {
            for (int j = 0; j < mapTexture.height; j += pixelRange)
            {
                for (int k = 0; k < checkPointTimes.Length; k++)
                    distances[k] = Mathf.Sqrt((Mathf.Pow(checkPointPositions[k].x - i, 2)) + (Mathf.Pow(checkPointPositions[k].y - j, 2))) / 1000;
                for (int k = 0; k < distances.Length; k++)
                {
                    if (distances[k] != 0)
                        xMulti *= distances[k];
                }
                for (int k = 0; k < distances.Length; k++)
                {
                    if (distances[k] != 0)
                        xMultiTotal += (Mathf.Pow((xMulti / distances[k]), formulaPower));
                }
                for (int k = 0; k < distances.Length; k++)
                {
                    if (distances[k] != 0)
                        subTotal += (checkPointTimes[k] * Mathf.Pow((xMulti / distances[k]), formulaPower));
                }
                difficultyValues[i / pixelRange, j / pixelRange] = (int)((subTotal * 1000 / maxTime) / xMultiTotal);
                xMulti = 1;
                xMultiTotal = 0;
                subTotal = 0;
            }
        }
        float difficultyMin = 0, difficultyMax = 0;
        Vector2 minPos = Vector2.zero, maxPos = Vector2.zero;
        for (int i = 0; i < mapTexture.width / pixelRange; i++)
        {
            for (int j = 0; j < mapTexture.height / pixelRange; j++)
            {
                if (difficultyMin == 0)
                {
                    difficultyMin = difficultyValues[i, j];
                    minPos = new Vector2(i * pixelRange, j * pixelRange);
                }
                if (difficultyMax == 0)
                {
                    difficultyMax = difficultyValues[i, j];
                    maxPos = new Vector2(i * pixelRange, j * pixelRange);
                }
                if (difficultyValues[i, j] < difficultyMin)
                {
                    difficultyMin = difficultyValues[i, j];
                    minPos = new Vector2(i * pixelRange, j * pixelRange);
                }
                if (difficultyValues[i, j] > difficultyMax)
                {
                    difficultyMax = difficultyValues[i, j];
                    maxPos = new Vector2(i * pixelRange, j * pixelRange);
                }
            }
        }
        //Normalize data between 0-1
        for (int i = 0; i < mapTexture.width / pixelRange; i++)
        {
            for (int j = 0; j < mapTexture.height / pixelRange; j++)
            {
                difficultyValues[i, j] = (difficultyValues[i, j] - difficultyMin) / (difficultyMax - difficultyMin);
                if (difficultyValues[i, j] <= 0.2f)
                {
                    colorValues[i, j] = Color.Lerp(Color.yellow, Color.green, (difficultyValues[i, j]) / (0.2f));
                    difficultyAreaValues[j][i] = 0;
                }
                else if (difficultyValues[i, j] > 0.2f && difficultyValues[i, j] <= 0.4f)
                {
                    colorValues[i, j] = Color.Lerp(Color.green, Color.cyan, (difficultyValues[i, j] - 0.2f) / (0.2f));
                    difficultyAreaValues[j][i] = 1;
                }
                else if (difficultyValues[i, j] > 0.4f && difficultyValues[i, j] <= 0.6f)
                {
                    colorValues[i, j] = Color.Lerp(Color.cyan, Color.blue, (difficultyValues[i, j] - 0.4f) / (0.2f));
                    difficultyAreaValues[j][i] = 2;
                }
                else if (difficultyValues[i, j] > 0.6f && difficultyValues[i, j] <= 0.8f)
                {
                    colorValues[i, j] = Color.Lerp(Color.blue, Color.magenta, (difficultyValues[i, j] - 0.6f) / (0.2f));
                    difficultyAreaValues[j][i] = 3;
                }
                else if (difficultyValues[i, j] > 0.8f)
                {
                    colorValues[i, j] = Color.Lerp(Color.magenta, Color.red, (difficultyValues[i, j] - 0.8f) / (0.2f));
                    difficultyAreaValues[j][i] = 4;
                    redAreaCount++;
                }
				if (difficultyValues[i, j] > 0.9f)
				{
					difficultyAreaValues[j][i] = 5;
				}
                //Debug.Log(difficultyAreaValues[0][0] + " , " + difficultyValues[0, 0] + " , " + colorValues[0, 0]);
            }
        }
        for (int i = 0; i < mapTexture.height / pixelRange; i++)
        {
            PlayerPrefsX.SetIntArray("CurrentDifficultyArea" + i, difficultyAreaValues[i]);
        }
        PlayerPrefs.SetFloat("RedAreaRatio", (redAreaCount / ((mapTexture.width / pixelRange) * (mapTexture.height / pixelRange))) * 100);

        for (int i = 0; i < mapTexture.width; i++)
        {
            for (int j = 0; j < mapTexture.height; j++)
            {
                mapTexture.SetPixel(i, j, colorValues[i / pixelRange, j / pixelRange]);
            }
        }
        mapTexture.Apply();
        difficultyMap.GetComponent<SpriteRenderer>().sprite = Sprite.Create(mapTexture, difficultyMap.GetComponent<SpriteRenderer>().sprite.rect, new Vector2(0.5f, 0.5f));
        savingTexture = mapTexture;
    }

    void getLoadingMenu(float waitTime, string loadingText)
    {
        loadingMenu.transform.FindChild("Text").GetComponent<Text>().text = loadingText;
        loadingWaitTime = waitTime;
        isLoadingSceneActive = true;
        loadingMenu.SetActive(true);
        isTouchesActive = false;
    }

    public void restarGame()
    {
        getLoadingMenu(3, "Yeniden Başlatılıyor...");
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
    }

    public void returnToMainMenu()
    {
        PlayerPrefs.SetInt("Babuş", 1);
        SceneManager.LoadSceneAsync("MainMenu");
    }

    //public void loadMapTextures()
    //{
    //    Object[] objects = Resources.LoadAll("Maps");
    //    List<Texture2D> textures = new List<Texture2D>();
    //    for (int i = 0; i < objects.Length; i++)
    //    {
    //        try
    //        {
    //            textures.Add((Texture2D)objects[i]);
    //        }
    //        catch (System.Exception)
    //        { }
    //    }
    //    for (int i = 0; i < textures.Count; i++)
    //    {
    //        Texture2D texture = textures[i];
    //        if (i == 0)
    //        {
    //            difficultyMap.GetComponent<Renderer>().material.mainTexture = texture;
    //            difficultyMap.GetComponent<SpriteRenderer>().sprite = Sprite.Create(texture, difficultyMap.GetComponent<SpriteRenderer>().sprite.rect, new Vector2(0.5f, 0.5f));
    //            difficultyMap.name = texture.name;
    //        }
    //        else
    //        {
    //            GameObject map = (GameObject)Instantiate(difficultyMap, difficultyMap.transform.position, difficultyMap.transform.rotation);
    //            map.transform.parent = difficultyMap.transform.parent;
    //            map.GetComponent<Renderer>().material.mainTexture = texture;
    //            map.GetComponent<SpriteRenderer>().sprite = Sprite.Create(texture, difficultyMap.GetComponent<SpriteRenderer>().sprite.rect, new Vector2(0.5f, 0.5f));
    //            map.name = texture.name;
    //        }
    //    }
    //    if (textures.Count < 0)
    //    {
    //        mapMenu.transform.FindChild("NextButton").gameObject.SetActive(false);
    //        questionText.text = "Herhangi Bir Harita Bulunamadı!!!";
    //        questionText.gameObject.SetActive(true);
    //    }
    //    else
    //    {
    //        mapList.GetChild(mapIndex).gameObject.SetActive(true);
    //        if (mapList.childCount == 1)
    //            mapMenu.transform.FindChild("NextButton").gameObject.SetActive(false);
    //        mapTimeText.text = mapList.GetChild(mapIndex).name.Remove(0, 13);
    //    }
    //}

    public void saveImage()
    {
		//string date = System.DateTime.Now.ToString("dd,MM,yyyy-HH,mm,ss");
		string date = System.DateTime.Now.ToString("yyyy,MM,dd-HH,mm,ss");
		pngSaver.SaveTexture2Folder(Application.streamingAssetsPath + "/Maps", "DifficultyMap" + date, savingTexture);
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }

    public void loadAllImages()
    {
        List<Texture2D> textures = new List<Texture2D>();
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
        for (int i = 0; i < textures.Count; i++)
        {
            Texture2D texture = textures[i];
            if (i == 0)
            {
                difficultyMap.GetComponent<Renderer>().material.mainTexture = texture;
                difficultyMap.GetComponent<SpriteRenderer>().sprite = Sprite.Create(texture, difficultyMap.GetComponent<SpriteRenderer>().sprite.rect, new Vector2(0.5f, 0.5f));
                difficultyMap.name = textureNames[i];
            }
            else
            {
                GameObject map = (GameObject)Instantiate(difficultyMap, difficultyMap.transform.position, difficultyMap.transform.rotation);
                map.transform.parent = difficultyMap.transform.parent;
                map.GetComponent<Renderer>().material.mainTexture = texture;
                map.GetComponent<SpriteRenderer>().sprite = Sprite.Create(texture, difficultyMap.GetComponent<SpriteRenderer>().sprite.rect, new Vector2(0.5f, 0.5f));
                map.name = textureNames[i];
            }
        }
        if (textures.Count == 0)
        {
            mapMenu.transform.FindChild("NextButton").gameObject.SetActive(false);
            questionText.text = "Herhangi Bir Harita Bulunamadı!!!";
            questionText.gameObject.SetActive(true);
        }
        else
        {
            mapList.GetChild(mapIndex).gameObject.SetActive(true);
            if (mapList.childCount == 1)
                mapMenu.transform.FindChild("NextButton").gameObject.SetActive(false);
            mapTimeText.text = mapList.GetChild(mapIndex).name.Remove(0, 13);
        }
    }
    

    public void getNextMap()
    {
        if (mapList.childCount <= mapIndex + 2)
            mapMenu.transform.FindChild("NextButton").gameObject.SetActive(false);
        mapList.GetChild(mapIndex).gameObject.SetActive(false);
        mapList.GetChild(mapIndex + 1).gameObject.SetActive(true);
        mapIndex++;
        if (!mapMenu.transform.FindChild("PreviousButton").gameObject.activeSelf)
            mapMenu.transform.FindChild("PreviousButton").gameObject.SetActive(true);
        mapTimeText.text = mapList.GetChild(mapIndex).name.Remove(0, 13);

    }
    public void getPreviousMap()
    {
        if (mapIndex - 1 == 0)
            mapMenu.transform.FindChild("PreviousButton").gameObject.SetActive(false);
        mapList.GetChild(mapIndex).gameObject.SetActive(false);
        mapList.GetChild(mapIndex - 1).gameObject.SetActive(true);
        mapIndex--;
        if (!mapMenu.transform.FindChild("NextButton").gameObject.activeSelf)
            mapMenu.transform.FindChild("NextButton").gameObject.SetActive(true);
        mapTimeText.text = mapList.GetChild(mapIndex).name.Remove(0, 13);
    }

    public void clearMaps()
    {
        for (int i = 0; i < mapList.childCount; i++)
            mapList.GetChild(i).gameObject.SetActive(false);
        mapList.gameObject.SetActive(false);
    }
}