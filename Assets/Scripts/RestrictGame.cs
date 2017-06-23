using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO;

public class RestrictGame : MonoBehaviour
{
    bool isFirstTouch, timeRunning, isLoadingSceneActive, isTouchesActive, isGameOver;

    public Transform checkPoints, bestBaloon, mapList, mapListGlobal;

    public GameObject finishMenu, loadingMenu, saveMenu, mapMenu;

    public Text questionText, mapTimeText;

    GameObject activeBaloon, tempBaloon;

    public List<GameObject> usableCheckPointsList = new List<GameObject>();

    Vector2 bestBaloonPosition;

    public float[] checkPointTimes, globalCheckPointTimes;

    public float time;

    float loadingTimer, loadingWaitTime, redAreaCount = 0;

    int formulaPower = 2, mapIndex = 0;

    public int pixelRange = 4;

    Collider2D tempHitCollider;

    Texture2D savingTexture, savingTextureGlobal;

    public GameObject difficultyMap, difficultyMapGlobal;

    PNGExporter pngSaver;

    public AudioSource pop1, pop2, pop3;

	public Text perc5, perc4, perc3, perc2, perc1, perc5Global, perc4Global, perc3Global, perc2Global, perc1Global;

    public Vector3[] checkPointPositions;

    void Start()
    {
        checkPointTimes = new float[checkPoints.childCount];
        for (int i = 0; i < checkPoints.childCount; i++)
            usableCheckPointsList.Add(checkPoints.GetChild(i).gameObject);
        isFirstTouch = true;
        pngSaver = new PNGExporter();
        isTouchesActive = true;

		perc5.color = Color.red;
		perc4.color = Color.blue;
		perc3.color = Color.cyan;
		perc2.color = Color.green;
		perc1.color = Color.yellow;

        checkPointPositions = new Vector3[checkPointTimes.Length];
        for (int i = 0; i < checkPointTimes.Length; i++)
        {
            checkPointPositions[i] = new Vector3((Camera.main.WorldToScreenPoint(checkPoints.GetChild(i).position).x - Camera.main.WorldToScreenPoint(checkPoints.GetChild(0).position).x) * 1.04986f, (Camera.main.WorldToScreenPoint(checkPoints.GetChild(i).position).y - Camera.main.WorldToScreenPoint(checkPoints.GetChild(0).position).y) * 1.046511f, 0);
        }
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
            if (loadingTimer >= 1.0f)
            {
                isLoadingSceneActive = false;
                StartCoroutine(difficultyMapsCreater());
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

    IEnumerator difficultyMapsCreater()
    {
        createLocalDifficultyMap();
        yield return new WaitForSeconds(3f);
        createGlobalDifficultyMap();
        yield return new WaitForSeconds(2f);
        loadingMenu.SetActive(false);
        isTouchesActive = true;
        saveMenu.SetActive(true);
        questionText.text = "Testi Onaylıyor Musunuz?";
        questionText.gameObject.SetActive(true);
    }

	string date;
	float redCount, yellowCount, greenCount, blueCount, cyanCount;

	void createLocalDifficultyMap()
    {
		redCount = 0; yellowCount = 0; greenCount = 0; blueCount = 0; cyanCount = 0;

        Texture2D mapTexture = new Texture2D(1600, 900);
        difficultyMap.GetComponent<Renderer>().material.mainTexture = mapTexture;
        float[,] difficultyValues = new float[mapTexture.width / pixelRange, mapTexture.height / pixelRange];
        int[][] difficultyAreaValues = new int[mapTexture.height / pixelRange][];
        Color[,] colorValues = new Color[mapTexture.width / pixelRange, mapTexture.height / pixelRange];
        float xMulti = 1, xMultiTotal = 0, subTotal = 0, maxTime = 0, minTime = 100;
        float[] distances = new float[checkPointTimes.Length];

        for (int i = 0; i < mapTexture.height / pixelRange; i++)
            difficultyAreaValues[i] = new int[mapTexture.width / pixelRange];

        for (int i = 0; i < checkPointTimes.Length; i++)
        {
            if (checkPointTimes[i] > maxTime)
                maxTime = checkPointTimes[i];
            if (checkPointTimes[i] < minTime)
                minTime = checkPointTimes[i];
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
                if (difficultyValues[i, j] <= 0.3f)
                {
                    colorValues[i, j] = Color.Lerp(Color.yellow, Color.green, (difficultyValues[i, j]) / (0.3f));
                    difficultyAreaValues[j][i] = 0;
                    if (difficultyValues[i, j] < 0.12f)
                        yellowCount++;
                    else
                        greenCount++;
                }
                else if (difficultyValues[i, j] > 0.3f && difficultyValues[i, j] <= 0.5f)
                {
                    colorValues[i, j] = Color.Lerp(Color.green, Color.cyan, (difficultyValues[i, j] - 0.3f) / (0.2f));
                    difficultyAreaValues[j][i] = 1;
                    if (difficultyValues[i, j] < 0.45f)
                        greenCount++;
                    else
                        cyanCount++;
                }
                else if (difficultyValues[i, j] > 0.5f && difficultyValues[i, j] <= 0.7f)
                {
                    colorValues[i, j] = Color.Lerp(Color.cyan, Color.blue, (difficultyValues[i, j] - 0.5f) / (0.2f));
                    difficultyAreaValues[j][i] = 2;
                    if (difficultyValues[i, j] < 0.55f)
                        cyanCount++;
                    else
                        blueCount++;
                }
                else if (difficultyValues[i, j] > 0.7f && difficultyValues[i, j] <= 0.9f)
                {
                    colorValues[i, j] = Color.Lerp(Color.blue, Color.red, (difficultyValues[i, j] - 0.7f) / (0.2f));
                    if (difficultyValues[i, j] < 0.8f)
                        difficultyAreaValues[j][i] = 3;
                    else
                        difficultyAreaValues[j][i] = 4;
                    if (difficultyValues[i, j] < 0.75f)
                        blueCount++;
                    else
                        redCount++;
                }
                else if (difficultyValues[i, j] > 0.9f)
                {
                    colorValues[i, j] = Color.red;
					redCount++;
                    redAreaCount++;
                    difficultyAreaValues[j][i] = 5;
                }
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

		date = System.DateTime.Now.ToString("yyyy,MM,dd-HH,mm,ss");

		PlayerPrefs.SetFloat("DifficultyMap" + date + "red", redCount / ((mapTexture.width / pixelRange) * (mapTexture.height / pixelRange)) * 100);
		PlayerPrefs.SetFloat("DifficultyMap" + date + "blue", blueCount / ((mapTexture.width / pixelRange) * (mapTexture.height / pixelRange)) * 100);
		PlayerPrefs.SetFloat("DifficultyMap" + date + "cyan", cyanCount / ((mapTexture.width / pixelRange) * (mapTexture.height / pixelRange)) * 100);
		PlayerPrefs.SetFloat("DifficultyMap" + date + "green", greenCount / ((mapTexture.width / pixelRange) * (mapTexture.height / pixelRange)) * 100);
		PlayerPrefs.SetFloat("DifficultyMap" + date + "yellow", yellowCount / ((mapTexture.width / pixelRange) * (mapTexture.height / pixelRange)) * 100);

        PlayerPrefs.SetFloat("CurrentMaxTime", maxTime);
        PlayerPrefs.SetFloat("CurrentMinTime", minTime);
    }

    void createGlobalDifficultyMap()
    {
        redCount = 0; yellowCount = 0; greenCount = 0; blueCount = 0; cyanCount = 0;

        Texture2D mapTexture = new Texture2D(1600, 900);
        difficultyMap.GetComponent<Renderer>().material.mainTexture = mapTexture;
        float[,] difficultyValues = new float[mapTexture.width / pixelRange, mapTexture.height / pixelRange], difficultyValuesMax = new float[mapTexture.width / pixelRange, mapTexture.height / pixelRange];
        int[][] difficultyAreaValues = new int[mapTexture.height / pixelRange][];
        Color[,] colorValues = new Color[mapTexture.width / pixelRange, mapTexture.height / pixelRange];
        float xMulti = 1, xMultiTotal = 0, subTotal = 0, subTotalMax = 0, targetCheckPointTime = 0, minDistance = 0;
        float[] distances = new float[checkPointTimes.Length];
        for (int i = 0; i < mapTexture.height / pixelRange; i++)
            difficultyAreaValues[i] = new int[mapTexture.width / pixelRange];
        
        for (int i = 0; i < mapTexture.width; i += pixelRange)
        {
            for (int j = 0; j < mapTexture.height; j += pixelRange)
            {
                for (int k = 0; k < checkPointTimes.Length; k++)
                {
                    distances[k] = Mathf.Sqrt((Mathf.Pow(checkPointPositions[k].x - i, 2)) + (Mathf.Pow(checkPointPositions[k].y - j, 2))) / 1000;
                    if(targetCheckPointTime == 0 || distances[k] < minDistance)
                    {
                        minDistance = distances[k];
                        targetCheckPointTime = globalCheckPointTimes[k];
                    }
                }
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
                    {
                        subTotal += (checkPointTimes[k] * Mathf.Pow((xMulti / distances[k]), formulaPower));
                        subTotalMax += (targetCheckPointTime * Mathf.Pow((xMulti / distances[k]), formulaPower));
                    }
                }
                difficultyValues[i / pixelRange, j / pixelRange] = (int)((subTotal * 1000 / targetCheckPointTime) / xMultiTotal);
                difficultyValuesMax[i / pixelRange, j / pixelRange] = (int)((subTotalMax * 1000 / targetCheckPointTime) / xMultiTotal);
                xMulti = 1;
                xMultiTotal = 0;
                subTotal = 0;
                subTotalMax = 0;
                targetCheckPointTime = 0;
                minDistance = 0;
            }
        }

        //Normalize data between 0-1
        for (int i = 0; i < mapTexture.width / pixelRange; i++)
        {
            for (int j = 0; j < mapTexture.height / pixelRange; j++)
            {
                difficultyValues[i, j] = (difficultyValues[i, j] / difficultyValuesMax[i, j]);
                if (difficultyValues[i, j] > 1)
                    difficultyValues[i, j] = 1;
                if (difficultyValues[i, j] <= 0.3f)
                {
                    colorValues[i, j] = Color.Lerp(Color.yellow, Color.green, (difficultyValues[i, j]) / (0.3f));
                    difficultyAreaValues[j][i] = 0;
                    if (difficultyValues[i, j] < 0.12f)
                        yellowCount++;
                    else
                        greenCount++;
                }
                else if (difficultyValues[i, j] > 0.3f && difficultyValues[i, j] <= 0.5f)
                {
                    colorValues[i, j] = Color.Lerp(Color.green, Color.cyan, (difficultyValues[i, j] - 0.3f) / (0.2f));
                    difficultyAreaValues[j][i] = 1;
                    if (difficultyValues[i, j] < 0.45f)
                        greenCount++;
                    else
                        cyanCount++;
                }
                else if (difficultyValues[i, j] > 0.5f && difficultyValues[i, j] <= 0.7f)
                {
                    colorValues[i, j] = Color.Lerp(Color.cyan, Color.blue, (difficultyValues[i, j] - 0.5f) / (0.2f));
                    difficultyAreaValues[j][i] = 2;
                    if (difficultyValues[i, j] < 0.55f)
                        cyanCount++;
                    else
                        blueCount++;
                }
                else if (difficultyValues[i, j] > 0.7f && difficultyValues[i, j] <= 0.9f)
                {
                    colorValues[i, j] = Color.Lerp(Color.blue, Color.red, (difficultyValues[i, j] - 0.7f) / (0.2f));
                    difficultyAreaValues[j][i] = 3;
                    if (difficultyValues[i, j] < 0.75f)
                        blueCount++;
                    else
                        redCount++;
                }
                else if (difficultyValues[i, j] > 0.9f)
                {
                    colorValues[i, j] = Color.red;
                    redCount++;
                    redAreaCount++;
                    if (difficultyValues[i, j] > 0.9f)
                        difficultyAreaValues[j][i] = 4;
                    else
                        difficultyAreaValues[j][i] = 5;
                }
            }
        }

        PlayerPrefs.SetFloat("RedAreaRatioGlobal", (redAreaCount / ((mapTexture.width / pixelRange) * (mapTexture.height / pixelRange))) * 100);

        for (int i = 0; i < mapTexture.width; i++)
        {
            for (int j = 0; j < mapTexture.height; j++)
            {
                mapTexture.SetPixel(i, j, colorValues[i / pixelRange, j / pixelRange]);
            }
        }
        mapTexture.Apply();
        difficultyMap.GetComponent<SpriteRenderer>().sprite = Sprite.Create(mapTexture, difficultyMap.GetComponent<SpriteRenderer>().sprite.rect, new Vector2(0.5f, 0.5f));
        savingTextureGlobal = mapTexture;

        PlayerPrefs.SetFloat("DifficultyMapGlobal" + date + "red", redCount / ((mapTexture.width / pixelRange) * (mapTexture.height / pixelRange)) * 100);
        PlayerPrefs.SetFloat("DifficultyMapGlobal" + date + "blue", blueCount / ((mapTexture.width / pixelRange) * (mapTexture.height / pixelRange)) * 100);
        PlayerPrefs.SetFloat("DifficultyMapGlobal" + date + "cyan", cyanCount / ((mapTexture.width / pixelRange) * (mapTexture.height / pixelRange)) * 100);
        PlayerPrefs.SetFloat("DifficultyMapGlobal" + date + "green", greenCount / ((mapTexture.width / pixelRange) * (mapTexture.height / pixelRange)) * 100);
        PlayerPrefs.SetFloat("DifficultyMapGlobal" + date + "yellow", yellowCount / ((mapTexture.width / pixelRange) * (mapTexture.height / pixelRange)) * 100);
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
        PlayerPrefs.SetInt("ReturningFromGame", 1);
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
		pngSaver.SaveTexture2Folder(Application.streamingAssetsPath + "/Maps", "DifficultyMap" + date, savingTexture);
        pngSaver.SaveTexture2Folder(Application.streamingAssetsPath + "/MapsGlobal", "DifficultyMapGlobal" + date, savingTextureGlobal);
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

		textureNames.Reverse();
		textures.Reverse();

        for (int i = 0; i < textures.Count; i++)
        {
            Texture2D texture = textures[i];
            if (i == 0)
            {
                difficultyMap.GetComponent<Renderer>().material.mainTexture = texture;
                difficultyMap.GetComponent<SpriteRenderer>().sprite = Sprite.Create(texture, difficultyMap.GetComponent<SpriteRenderer>().sprite.rect, new Vector2(0.5f, 0.5f));
                difficultyMap.name = textureNames[i];
				difficultyMap.transform.localScale = Vector3.one * 0.45f;
				SetPercentages(difficultyMap.name);
            }
            else
            {
                GameObject map = (GameObject)Instantiate(difficultyMap, difficultyMap.transform.position, difficultyMap.transform.rotation);
				map.transform.localScale = Vector3.one * 0.45f;
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


        List<Texture2D> texturesGlobal = new List<Texture2D>();
        List<string> textureNamesGlobal = new List<string>();
        DirectoryInfo dirGlobal = new DirectoryInfo(Application.streamingAssetsPath + "/MapsGlobal");
        FileInfo[] infoGlobal = dirGlobal.GetFiles("*.*");
        foreach (FileInfo fi in infoGlobal)
        {
            string fileExtension = fi.Extension;
            if (fileExtension != ".meta")
            {
                string imageNameGlobal = Path.GetFileName(fi.Name);
                string textureNameGlobal = Path.GetFileNameWithoutExtension(fi.Name);
                WWW www = new WWW("file://" + Application.streamingAssetsPath + "/MapsGlobal/" + imageNameGlobal);
                textureNamesGlobal.Add(textureNameGlobal);

                if (!string.IsNullOrEmpty(www.error))
                {
                    Debug.Log(www.error);
                }
                else
                {
                    www.texture.name = textureNameGlobal;
                    texturesGlobal.Add(www.texture);
                }
            }
        }

        textureNamesGlobal.Reverse();
        texturesGlobal.Reverse();

        for (int i = 0; i < texturesGlobal.Count; i++)
        {
            Texture2D textureGlobal = texturesGlobal[i];
            if (i == 0)
            {
                difficultyMapGlobal.GetComponent<Renderer>().material.mainTexture = textureGlobal;
                difficultyMapGlobal.GetComponent<SpriteRenderer>().sprite = Sprite.Create(textureGlobal, difficultyMapGlobal.GetComponent<SpriteRenderer>().sprite.rect, new Vector2(0.5f, 0.5f));
                difficultyMapGlobal.name = textureNamesGlobal[i];
                difficultyMapGlobal.transform.localScale = Vector3.one * 0.45f;
                SetPercentagesGlobal(difficultyMapGlobal.name);
            }
            else
            {
                GameObject mapGlobal = (GameObject)Instantiate(difficultyMapGlobal, difficultyMapGlobal.transform.position, difficultyMapGlobal.transform.rotation);
                mapGlobal.transform.localScale = Vector3.one * 0.45f;
                mapGlobal.transform.parent = difficultyMapGlobal.transform.parent;
                mapGlobal.GetComponent<Renderer>().material.mainTexture = textureGlobal;
                mapGlobal.GetComponent<SpriteRenderer>().sprite = Sprite.Create(textureGlobal, difficultyMap.GetComponent<SpriteRenderer>().sprite.rect, new Vector2(0.5f, 0.5f));
                mapGlobal.name = textureNamesGlobal[i];
            }
        }

        if (textures.Count > 0)
        {
            mapListGlobal.GetChild(mapIndex).gameObject.SetActive(true);
        }
    }
    

    public void getNextMap()
    {
        if (mapList.childCount <= mapIndex + 2)
            mapMenu.transform.FindChild("NextButton").gameObject.SetActive(false);
        mapList.GetChild(mapIndex).gameObject.SetActive(false);
        mapList.GetChild(mapIndex + 1).gameObject.SetActive(true);
        mapListGlobal.GetChild(mapIndex).gameObject.SetActive(false);
        mapListGlobal.GetChild(mapIndex + 1).gameObject.SetActive(true);
        mapIndex++;
        if (!mapMenu.transform.FindChild("PreviousButton").gameObject.activeSelf)
            mapMenu.transform.FindChild("PreviousButton").gameObject.SetActive(true);
        mapTimeText.text = mapList.GetChild(mapIndex).name.Remove(0, 13);
		SetPercentages(mapList.GetChild(mapIndex).name);
        SetPercentagesGlobal(mapListGlobal.GetChild(mapIndex).name);
    }
    public void getPreviousMap()
    {
        if (mapIndex - 1 == 0)
            mapMenu.transform.FindChild("PreviousButton").gameObject.SetActive(false);
        mapList.GetChild(mapIndex).gameObject.SetActive(false);
        mapList.GetChild(mapIndex - 1).gameObject.SetActive(true);
        mapListGlobal.GetChild(mapIndex).gameObject.SetActive(false);
        mapListGlobal.GetChild(mapIndex - 1).gameObject.SetActive(true);
        mapIndex--;
        if (!mapMenu.transform.FindChild("NextButton").gameObject.activeSelf)
            mapMenu.transform.FindChild("NextButton").gameObject.SetActive(true);
        mapTimeText.text = mapList.GetChild(mapIndex).name.Remove(0, 13);
		SetPercentages(mapList.GetChild(mapIndex).name);
        SetPercentagesGlobal(mapListGlobal.GetChild(mapIndex).name);
    }

    public void clearMaps()
    {
        for (int i = 0; i < mapList.childCount; i++)
        {
            mapList.GetChild(i).gameObject.SetActive(false);
            mapListGlobal.GetChild(i).gameObject.SetActive(false);
        }
        mapList.gameObject.SetActive(false);
        mapListGlobal.gameObject.SetActive(false);
    }

	private void SetPercentages(string fileName)
	{
		perc5.text = "% " + PlayerPrefs.GetFloat(fileName + "red").ToString("0.0");
		perc4.text = "% " + PlayerPrefs.GetFloat(fileName + "blue").ToString("0.0");
		perc3.text = "% " + PlayerPrefs.GetFloat(fileName + "cyan").ToString("0.0");
		perc2.text = "% " + PlayerPrefs.GetFloat(fileName + "green").ToString("0.0");
		perc1.text = "% " + PlayerPrefs.GetFloat(fileName + "yellow").ToString("0.0");
    }

    private void SetPercentagesGlobal(string fileNameGlobal)
    {
        perc5Global.text = "% " + PlayerPrefs.GetFloat(fileNameGlobal + "red").ToString("0.0");
        perc4Global.text = "% " + PlayerPrefs.GetFloat(fileNameGlobal + "blue").ToString("0.0");
        perc3Global.text = "% " + PlayerPrefs.GetFloat(fileNameGlobal + "cyan").ToString("0.0");
        perc2Global.text = "% " + PlayerPrefs.GetFloat(fileNameGlobal + "green").ToString("0.0");
        perc1Global.text = "% " + PlayerPrefs.GetFloat(fileNameGlobal + "yellow").ToString("0.0");
    }
}