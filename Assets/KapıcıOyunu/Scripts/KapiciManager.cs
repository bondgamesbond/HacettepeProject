using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class KapiciManager : MonoBehaviour
{
    public static KapiciManager Instance;

    public Transform leftWindows, rightWindows, makara, makaraBack, rope, sepet, movingSut, movingEkmek, connectorMakara, suBorusu, thanksSounds, makaraObjectsParent;

    public RectTransform tutorialBackground, tutorialArrow, zilButtons, warningPopUp;

    public GameObject tutorialDevamButton, tutorialsParent, tutorialAnimsParent, finishScene, winParticle, borders, homeButton;

    public Text tutorialText, warningText;

    public string[] tutorialTexts;

    List<Window> leftWindowList, rightWindowList;
 
    public bool[] ekmekRequests, sutRequests;

    public List<Window> activeWindows;

    public float[] floarYValues, floorZilTimers;

    public float floarThreshold;

    public List<int> requestingFloorIds;

    public int[] floorNeededZilCounts;

    public bool isLeftSided;

    public bool sepetOnHold, ekmekOnHold, sutOnHold, isOnTutorial, isGameOver;

    public Image warningPopUpImage;

    Touch touch;

    Ray screenRay;

    RaycastHit hit;

    Transform onHoldMovingObject;

    public int tutorialId = 0;

    int touchCount, onSepetTouchId, onMovingObjectTouchId, leftEkmekCount = 6, leftSutCount = 6, zilWaitTime = 3, tutorialAnimId, makaraHighLevel;

    Vector3 initialMovingEkmekPos, initialMovingSutPos, tempMovingObjectPos;

    public AudioSource dingDongSound, takeFromSepetSound, putMovingObjectSound, putMovingObjectToWrongPlaceSound, finishSound, buttonSound, openLightSound, makaraRotateSound;

    public List<Button> zilButtonList = new List<Button>();

    List<GameObject> zilEkmekIconList = new List<GameObject>(), zilSutIconList = new List<GameObject>();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
            
	void Start ()
    {
        requestingFloorIds = new List<int>();
        leftWindowList = new List<Window>();
        rightWindowList = new List<Window>();
        activeWindows = new List<Window>();
        for (int i = 0; i < leftWindows.childCount; i++)
        {
            leftWindowList.Add(leftWindows.GetChild(i).GetComponent<Window>());
            rightWindowList.Add(rightWindows.GetChild(i).GetComponent<Window>());
        }
        floorNeededZilCounts = new int[leftWindowList.Count];
        floorZilTimers = new float[leftWindowList.Count];
        ekmekRequests = new bool[leftWindowList.Count];
        sutRequests = new bool[leftWindowList.Count];
        for (int i = 0; i < floorNeededZilCounts.Length; i++)
        {
            floorNeededZilCounts[i] = i + 1;
            floorZilTimers[i] = zilWaitTime;
            ekmekRequests[i] = true;
            sutRequests[i] = true;
        }
       
        initialMovingEkmekPos = movingEkmek.localPosition;
        initialMovingSutPos = movingSut.localPosition;
        onMovingObjectTouchId = -1;
        isOnTutorial = true;
        tutorialId = -1;
        tutorialsParent.SetActive(true);
        foreach (Button child in zilButtons.GetComponentsInChildren<Button>())
        {
            zilButtonList.Add(child);
            child.enabled = false;
        }
        for (int i = 0; i < zilButtons.childCount; i++)
        {
            zilEkmekIconList.Add(zilButtons.GetChild(i).FindChild("ekmek").gameObject);
            zilSutIconList.Add(zilButtons.GetChild(i).FindChild("sut").gameObject);
        }
    }
	
	void Update ()
    {
        if (!isGameOver)
        {
            touchCount = Input.touchCount;

            if (touchCount > 0)
            {
                for (int i = 0; i < touchCount; i++)
                {
                    touch = Input.GetTouch(i);
                    if (touch.phase == TouchPhase.Began)  //On Touch Down
                    {
                        screenRay = Camera.main.ScreenPointToRay(touch.position);
                        if (Physics.Raycast(screenRay, out hit))
                        {
                            if (hit.transform.tag == "Sepet")
                            {
                                if (sepetOnHold)
                                {
                                    if (leftEkmekCount > 0)
                                    {
                                        ekmekOnHold = true;
                                        onMovingObjectTouchId = i;
                                        onHoldMovingObject = movingEkmek;
                                        takeFromSepetSound.Play();
                                    }
                                    else if (leftEkmekCount == 0 && leftSutCount > 0)
                                    {
                                        sutOnHold = true;
                                        onMovingObjectTouchId = i;
                                        onHoldMovingObject = movingSut;
                                        takeFromSepetSound.Play();
                                    }
                                    if (isOnTutorial && tutorialId == 5)
                                        removeTutorialAnim();
                                }
                                else
                                {
                                    sepetOnHold = true;
                                    onSepetTouchId = i;
                                    Sepet.Instance.sepetGlowSprite.enabled = true;
                                }
                            }
                        }
                    }
                    if (touch.phase == TouchPhase.Moved)
                    {
                        if (i == onMovingObjectTouchId)
                        {
                            if (onHoldMovingObject != null)
                            {
                                tempMovingObjectPos = Camera.main.ScreenToWorldPoint(touch.position);
                                tempMovingObjectPos.x -= 0.5f;
                                tempMovingObjectPos.y += 0.3f;
                                tempMovingObjectPos.z = 0;
                                onHoldMovingObject.position = tempMovingObjectPos;
                            }
                        }
                    }
                    if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                    {
                        if (i == onSepetTouchId)
                        {
                            sepetOnHold = false;
                            if (Sepet.Instance != null)
                                Sepet.Instance.sepetGlowSprite.enabled = false;
                            if (ekmekOnHold)
                                releaseEkmekOrSut(true);
                            if (sutOnHold)
                                releaseEkmekOrSut(false);
                        }
                        else if (i == onMovingObjectTouchId)
                        {
                            screenRay = Camera.main.ScreenPointToRay(touch.position);
                            if (Physics.Raycast(screenRay, out hit))
                            { 
                                if (hit.transform.tag == "Balloon")
                                {
                                    if (activeWindows[tempRequestId].isReadyToGetOrder)
                                    {
                                        if (ekmekOnHold)
                                        {
                                            releaseEkmekOrSut(true);
                                            leftEkmekCount--;
                                            if (leftEkmekCount == 0)
                                            {
                                                Sepet.Instance.swapSutAndEkmek();
                                                for (int j = 0; j < floorNeededZilCounts.Length; j++)
                                                {
                                                    floorNeededZilCounts[j] = j + 1;
                                                    floorZilTimers[j] = zilWaitTime;
                                                }
                                            }
                                            activeWindows[tempRequestId].completeOrder();
                                            zilEkmekIconList[tempRequestId].SetActive(true);
                                            thanksSounds.GetChild(Random.Range(0, thanksSounds.childCount)).GetComponent<AudioSource>().Play();
                                            requestingFloorIds.Remove(tempRequestId);
                                            if (!isOnTutorial)
                                                getNextOrder();
                                            else
                                                getNextTutorial();
                                            Sepet.Instance.sepetCollider.enabled = false;
                                            putMovingObjectSound.Play();
                                        }
                                        if (sutOnHold)
                                        {
                                            releaseEkmekOrSut(false);
                                            leftSutCount--;
                                            activeWindows[tempRequestId].completeOrder();
                                            zilSutIconList[tempRequestId].gameObject.SetActive(true);
                                            thanksSounds.GetChild(Random.Range(0, thanksSounds.childCount)).GetComponent<AudioSource>().Play();
                                            requestingFloorIds.Remove(tempRequestId);
                                            getNextOrder();
                                            Sepet.Instance.sepetCollider.enabled = false;
                                            putMovingObjectSound.Play();
                                        }
                                    }
                                    else
                                    {
                                        putMovingObjectToWrongPlaceSound.Play();
                                        warningPopUp.position = new Vector2(0.45f, hit.transform.position.y);
                                        StartCoroutine(popUpGetter());
                                        if (ekmekOnHold)
                                            releaseEkmekOrSut(true);
                                        if (sutOnHold)
                                            releaseEkmekOrSut(false);
                                    }
                                }
                                else
                                {
                                    putMovingObjectToWrongPlaceSound.Play();
                                    if (ekmekOnHold)
                                        releaseEkmekOrSut(true);
                                    if (sutOnHold)
                                        releaseEkmekOrSut(false);
                                }
                            }
                            else
                            {
                                if (ekmekOnHold)
                                    releaseEkmekOrSut(true);
                                if (sutOnHold)
                                    releaseEkmekOrSut(false);
                            }
                        }
                    }
                }
            }
            else
            {
                if (sepetOnHold)
                    sepetOnHold = false;
                if (ekmekOnHold)
                    ekmekOnHold = false;
                if (sutOnHold)
                    sutOnHold = false;
                onSepetTouchId = 0;
                if (Sepet.Instance != null)
                    Sepet.Instance.sepetGlowSprite.enabled = false;
            }

            for (int i = 0; i < requestingFloorIds.Count; i++)
            {
                if (floorNeededZilCounts[requestingFloorIds[i]] < requestingFloorIds[i] + 1 && floorNeededZilCounts[requestingFloorIds[i]] > 0)
                {
                    floorZilTimers[requestingFloorIds[i]] -= Time.deltaTime;
                    if (floorZilTimers[requestingFloorIds[i]] <= 0f)
                    {
                        floorZilTimers[requestingFloorIds[i]] = zilWaitTime;
                        floorNeededZilCounts[requestingFloorIds[i]] = requestingFloorIds[i] + 1;
                    }
                }
            }
            if(popUpActive)
            {
                tempColor.a -= Time.deltaTime * 0.7f;
                tempColorBlack.a -= Time.deltaTime * 0.7f;
                if (tempColor.a <= 0)
                {
                    warningPopUp.gameObject.SetActive(false);
                    popUpActive = false;
                }
                warningPopUpImage.color = tempColor;
                warningText.color = tempColorBlack;
            }
        }
    }

    Color tempColor, tempColorBlack;
    bool popUpActive;

    IEnumerator popUpGetter()
    {
        tempColor = Color.white;
        tempColorBlack = Color.black;
        warningPopUpImage.color = tempColor;
        warningText.color = tempColorBlack;
        warningPopUp.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        popUpActive = true;
    }

    void releaseEkmekOrSut(bool ekmek)
    {
        if(ekmek)
        {
            movingEkmek.localPosition = initialMovingEkmekPos;
            ekmekOnHold = false;
        }
        else
        {
            movingSut.localPosition = initialMovingSutPos;
            sutOnHold = false;
        }
        onMovingObjectTouchId = -1;
        onHoldMovingObject = null;
    }

    public void dingDong(int zilId)
    {
        dingDongSound.Play();
        if (requestingFloorIds.Contains(zilId))
        {
            floorNeededZilCounts[zilId]--;
            floorZilTimers[zilId] = zilWaitTime;
            if (floorNeededZilCounts[zilId] == 0)
            {
                activeWindows[zilId].setReadyToTakeOrder();
                if (isOnTutorial && tutorialId == 2)
                    getNextTutorial();
                openLightSound.Play();
            }
            if (isOnTutorial && tutorialId == 2)
                removeTutorialAnim();
        }
    }

    public void removeTutorialAnim()
    {
        tutorialBackground.gameObject.SetActive(false);
        tutorialAnimsParent.transform.GetChild(tutorialAnimId).gameObject.SetActive(false);
        tutorialAnimsParent.transform.GetChild(tutorialAnimId).GetComponent<SkeletonAnimation>().enabled = false;
    }

    int tempRequestId;
    void getNextOrder()
    {
        tempRequestId = Random.Range(0, activeWindows.Count);
        if (leftEkmekCount > 0)
        {
            while (ekmekRequests[tempRequestId] == false)
                tempRequestId = Random.Range(0, activeWindows.Count);
            ekmekRequests[tempRequestId] = false;
            activeWindows[tempRequestId].requestOrder(true);
            requestingFloorIds.Add(tempRequestId);
        }
        else if (leftSutCount > 0)
        {
            while (sutRequests[tempRequestId] == false)
                tempRequestId = Random.Range(0, activeWindows.Count);
            sutRequests[tempRequestId] = false;
            activeWindows[tempRequestId].requestOrder(false);
            requestingFloorIds.Add(tempRequestId);
        }
        else
        {
            finishScene.SetActive(true);
            winParticle.SetActive(true);
            isGameOver = true;
            if (warningPopUp.gameObject.activeSelf)
                warningPopUp.gameObject.SetActive(false);
            finishSound.Play();
        }
    }

    void getSpecificOrder(int orderId, bool isEkmek)
    {
        tempRequestId = orderId;
        if(isEkmek)
        {
            ekmekRequests[tempRequestId] = false;
            activeWindows[tempRequestId].requestOrder(true);
        }
        else
        {
            sutRequests[tempRequestId] = false;
            activeWindows[tempRequestId].requestOrder(false);
        }
        requestingFloorIds.Add(tempRequestId);
    }

    public void getNextTutorial()
    {
        tutorialId++;
        if (tutorialTexts.Length > tutorialId)
            tutorialText.text = tutorialTexts[tutorialId];
        if (tutorialId == 0)
        {
            borders.SetActive(true);
            homeButton.SetActive(false);
            tutorialDevamButton.SetActive(false);
        }
        else if (tutorialId == 1)
        {
            tutorialDevamButton.SetActive(true);
            if (isLeftSided)
                tutorialBackground.localPosition = new Vector3(179f, 229f, 0);
            else
                tutorialBackground.localPosition = new Vector3(480f, 229f, 0);
            tutorialArrow.gameObject.SetActive(true);
            getSpecificOrder(4, true);
        }
        else if (tutorialId == 2)
        {
            if (isLeftSided)
            {
                tutorialBackground.localPosition = new Vector3(113f, -260f, 0);
                tutorialArrow.eulerAngles = new Vector3(0, 180, 0);
                tutorialArrow.localPosition = new Vector3(212.2f, 23.2f, 0);
            }
            else
            {
                tutorialBackground.localPosition = new Vector3(-118.5f, -260f, 0);
                tutorialArrow.eulerAngles = new Vector3(0, 0, 0);
                tutorialArrow.localPosition = new Vector3(-208.3f, 23.2f, 0);
                tutorialAnimsParent.transform.GetChild(tutorialAnimId).transform.localPosition = new Vector3(-5, -3.44f, 0);
            }
            tutorialAnimsParent.transform.GetChild(tutorialAnimId).GetComponent<SkeletonAnimation>().enabled = true;
            tutorialAnimsParent.transform.GetChild(tutorialAnimId).gameObject.SetActive(true);
            tutorialDevamButton.SetActive(false);
            for (int i = 0; i < zilButtonList.Count; i++)
                zilButtonList[i].enabled = true;
        }
        else if (tutorialId == 3)
        {
            if (isLeftSided)
            {
                tutorialBackground.localPosition = new Vector3(-420f, 214f, 0);
            }
            else
            {
                tutorialBackground.localPosition = new Vector3(-118f, 214f, 0);
                tutorialArrow.eulerAngles = new Vector3(0, 180, 0);
                tutorialArrow.localPosition = new Vector3(212.2f, 23.2f, 0);
            }
            tutorialBackground.gameObject.SetActive(true);
            tutorialDevamButton.SetActive(true);
        }
        else if (tutorialId == 4)
        {
            tutorialBackground.gameObject.SetActive(true);
            tutorialAnimId++;
            if (isLeftSided)
            {
                if (makaraHighLevel == 1)
                    tutorialBackground.localPosition = new Vector3(-726f, 386f, 0);
                else if (makaraHighLevel == 2)
                {
                    tutorialBackground.localPosition = new Vector3(-726f, 91.2f, 0);
                    tutorialAnimsParent.transform.GetChild(tutorialAnimId).transform.localPosition = new Vector3(-4.8f, 0, 0);
                }
                else if (makaraHighLevel == 3)
                {
                    tutorialBackground.localPosition = new Vector3(-726f, -200f, 0);
                    tutorialAnimsParent.transform.GetChild(tutorialAnimId).transform.localPosition = new Vector3(-4.8f, -2.96f, 0);
                }
            }
            else
            {
                tutorialAnimsParent.transform.GetChild(tutorialAnimId).transform.eulerAngles = new Vector3(0, 180f, 0);
                if (makaraHighLevel == 4)
                {
                    tutorialBackground.localPosition = new Vector3(144f, 386f, 0);
                    tutorialAnimsParent.transform.GetChild(tutorialAnimId).transform.localPosition = new Vector3(4.8f, 2.96f, 0);
                }
                else if (makaraHighLevel == 5)
                {
                    tutorialBackground.localPosition = new Vector3(144f, 91.2f, 0);
                    tutorialAnimsParent.transform.GetChild(tutorialAnimId).transform.localPosition = new Vector3(4.8f, 0, 0);
                }
                else if (makaraHighLevel == 6)
                {
                    tutorialBackground.localPosition = new Vector3(144f, -200f, 0);
                    tutorialAnimsParent.transform.GetChild(tutorialAnimId).transform.localPosition = new Vector3(4.8f,-2.96f, 0);
                }
                tutorialArrow.eulerAngles = new Vector3(0, 180, 0);
                tutorialArrow.localPosition = new Vector3(210.6f, 23.2f, 0);
            }
            tutorialAnimsParent.transform.GetChild(tutorialAnimId).GetComponent<SkeletonAnimation>().enabled = true;
            tutorialAnimsParent.transform.GetChild(tutorialAnimId).gameObject.SetActive(true);
            tutorialDevamButton.SetActive(false);
            MakaraRotater.Instance.thisCollider.enabled = true;
        }
        else if (tutorialId == 5)
        {
            tutorialAnimId++;
            if (isLeftSided)
            {
                tutorialBackground.localPosition = new Vector3(-664f, 211f, 0);
            }
            else
            {
                tutorialBackground.localPosition = new Vector3(608.3f, 211f, 0);
                tutorialArrow.eulerAngles = new Vector3(0, 0, 0);
                tutorialArrow.localPosition = new Vector3(-208.3f, 23.2f, 0);
                tutorialAnimsParent.transform.GetChild(tutorialAnimId).transform.eulerAngles = new Vector3(0, 180f, 0);
                tutorialAnimsParent.transform.GetChild(tutorialAnimId).transform.localPosition = new Vector3(1.25f, 0.06f, 0);
            }
            tutorialBackground.gameObject.SetActive(true);
            tutorialAnimsParent.transform.GetChild(tutorialAnimId).GetComponent<SkeletonAnimation>().enabled = true;
            tutorialAnimsParent.transform.GetChild(tutorialAnimId).gameObject.SetActive(true);
        }
        else if (tutorialId == 6)
        {
            tutorialBackground.localPosition = new Vector3(0, 100f, 0);
            tutorialBackground.gameObject.SetActive(true);
            tutorialArrow.gameObject.SetActive(false);
            tutorialDevamButton.SetActive(true);
        }
        else
        {
            isOnTutorial = false;
            tutorialsParent.SetActive(false);
            tutorialAnimsParent.SetActive(false);
            MakaraRotater.Instance.thisCollider.enabled = true;
            getNextOrder();
        }
    }

    public void chooseMakaraLevel(int id)
    {
        if (id > 3)
        {
            isLeftSided = false;
            activeWindows = rightWindowList;
            zilButtons.localPosition = new Vector3(-zilButtons.localPosition.x, zilButtons.localPosition.y, 0);
            for (int i = 0; i < zilButtons.childCount; i++)
            {
                zilEkmekIconList[i].transform.localPosition = new Vector3(-zilEkmekIconList[i].transform.localPosition.x, zilEkmekIconList[i].transform.localPosition.y, 0);
                zilSutIconList[i].transform.localPosition = new Vector3(-zilSutIconList[i].transform.localPosition.x, zilSutIconList[i].transform.localPosition.y, 0);
            }
            suBorusu.localScale = new Vector3(-1, 1, 1);
            suBorusu.localPosition = new Vector3(-suBorusu.localPosition.x, suBorusu.localPosition.y, 0);
            sepet.localScale = new Vector3(-sepet.localScale.x, sepet.localScale.y, 1);
            sepet.localPosition = new Vector3(-sepet.localPosition.x, sepet.localPosition.y, 0);
            rope.localPosition = new Vector3(-rope.localPosition.x, rope.localPosition.y, 0);
            connectorMakara.localScale = new Vector3(-1, 1, 1);
            connectorMakara.localPosition = new Vector3(-connectorMakara.localPosition.x, connectorMakara.localPosition.y, 0);
        }
        else
        {
            isLeftSided = true;
            activeWindows = leftWindowList;
        }
        for (int i = 0; i < makaraObjectsParent.childCount; i++)
            makaraObjectsParent.GetChild(i).gameObject.SetActive(false);
        if (id != 1 && id != 4)
            connectorMakara.gameObject.SetActive(true);
        else
            connectorMakara.gameObject.SetActive(false);
        borders.SetActive(false);
        makaraObjectsParent.GetChild(id - 1).gameObject.SetActive(true);
        rope.gameObject.SetActive(true);
        sepet.gameObject.SetActive(true);
        zilButtons.gameObject.SetActive(true);
        homeButton.SetActive(true);
        makaraHighLevel = id;
        getNextTutorial();
    }

    public void playButtonSound()
    {
        buttonSound.Play();
    }

    public void returnToMainMenu()
    {
        PlayerPrefs.SetInt("ReturningFromGame", 1);
        SceneManager.LoadSceneAsync("MainMenu");
    }

    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
