using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class KapiciManager : MonoBehaviour
{
    public static KapiciManager Instance;

    public Transform leftWindows, rightWindows, makara, makaraBack, rope, sepet, movingSut, movingEkmek, suBorusu, thanksSounds;

    public RectTransform tutorialBackground, tutorialArrow, zilButtons;

    public GameObject tutorialDevamButton, tutorialsParent, tutorialAnimsParent, finishScene, winParticle;

    public Text tutorialText;

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

    Touch touch;

    Ray screenRay;

    RaycastHit hit;

    Transform onHoldMovingObject;

    public int tutorialId = 0;

    int touchCount, onSepetTouchId, onMovingObjectTouchId, leftEkmekCount = 6, leftSutCount = 6, zilWaitTime = 3, tutorialAnimId;

    Vector3 initialMovingEkmekPos, initialMovingSutPos, tempMovingObjectPos;

    public AudioSource dingDongSound, takeFromSepetSound, putMovingObjectSound, putMovingObjectToWrongPlaceSound, finishSound, buttonSound, openLightSound, makaraRotateSound;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
            
	void Start ()
    {
        if (RestrictionMap.findDifficulty(makara.transform.position) <= RestrictionMap.findDifficulty(new Vector2(-makara.localPosition.x, makara.localPosition.y)))
            isLeftSided = true;
        else
            isLeftSided = false;
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
        if (isLeftSided)
        {
            activeWindows = leftWindowList;
        }
        else
        {
            activeWindows = rightWindowList;
            zilButtons.localPosition = new Vector3(-zilButtons.localPosition.x, zilButtons.localPosition.y, 0);
            makara.localEulerAngles = new Vector3(0, 180, 0);
            makara.localPosition = new Vector3(-makara.localPosition.x, makara.localPosition.y, 0);
            makaraBack.localEulerAngles = new Vector3(0, 180, 0);
            makaraBack.localPosition = new Vector3(-makaraBack.localPosition.x, makaraBack.localPosition.y, 0);
            suBorusu.localEulerAngles = new Vector3(0, 180, 0);
            suBorusu.localPosition = new Vector3(-suBorusu.localPosition.x, suBorusu.localPosition.y, 0);
            sepet.localEulerAngles = new Vector3(0, 180, 0);
            sepet.localPosition = new Vector3(-sepet.localPosition.x, sepet.localPosition.y, 0);
            rope.localPosition = new Vector3(-rope.localPosition.x, rope.localPosition.y, 0);
        }
        initialMovingEkmekPos = movingEkmek.localPosition;
        initialMovingSutPos = movingSut.localPosition;
        onMovingObjectTouchId = -1;
        isOnTutorial = true;
        tutorialId = -1;
        tutorialsParent.SetActive(true);
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
                                    if (isOnTutorial && tutorialId == 4)
                                        removeTutorialAnim();
                                }
                                else
                                {
                                    sepetOnHold = true;
                                    onSepetTouchId = i;
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
                                        Debug.Log("Order Taken");
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
                                            thanksSounds.GetChild(Random.Range(0, thanksSounds.childCount)).GetComponent<AudioSource>().Play();
                                            requestingFloorIds.Remove(tempRequestId);
                                            getNextOrder();
                                            Sepet.Instance.sepetCollider.enabled = false;
                                            putMovingObjectSound.Play();
                                        }
                                    }
                                    else
                                    {
                                        Debug.Log("Not Ready To Take Order");
                                        putMovingObjectToWrongPlaceSound.Play();
                                        if (ekmekOnHold)
                                            releaseEkmekOrSut(true);
                                        if (sutOnHold)
                                            releaseEkmekOrSut(false);
                                    }
                                }
                                else
                                {
                                    Debug.Log("Order Put To Wrong Area");
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
        }
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
                if (isOnTutorial && tutorialId == 1)
                    getNextTutorial();
                openLightSound.Play();
            }
            if (isOnTutorial && tutorialId == 1)
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
            Debug.Log("Game Is Over");
            finishScene.SetActive(true);
            winParticle.SetActive(true);
            isGameOver = true;
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
            if (isLeftSided)
                tutorialBackground.localPosition = new Vector3(179f, 229f, 0);
            else
                tutorialBackground.localPosition = new Vector3(480f, 229f, 0);
            tutorialArrow.gameObject.SetActive(true);
            getSpecificOrder(4, true);
        }
        else if (tutorialId == 1)
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
        }
        else if (tutorialId == 2)
        {
            if (isLeftSided)
                tutorialBackground.localPosition = new Vector3(-420f, 214f, 0);
            else
            {
                tutorialBackground.localPosition = new Vector3(-118f, 214f, 0);
                tutorialArrow.eulerAngles = new Vector3(0, 180, 0);
                tutorialArrow.localPosition = new Vector3(212.2f, 23.2f, 0);
            }
            tutorialBackground.gameObject.SetActive(true);
            tutorialDevamButton.SetActive(true);
        }
        else if (tutorialId == 3)
        {
            tutorialBackground.gameObject.SetActive(true);
            tutorialAnimId++;
            if (isLeftSided)
            {
                tutorialBackground.localPosition = new Vector3(-726f, 386f, 0);
            }
            else
            {
                tutorialBackground.localPosition = new Vector3(144f, 386f, 0);
                tutorialArrow.eulerAngles = new Vector3(0, 180, 0);
                tutorialArrow.localPosition = new Vector3(210.6f, 23.2f, 0);
                tutorialAnimsParent.transform.GetChild(tutorialAnimId).transform.eulerAngles = new Vector3(0, 180f, 0);
                tutorialAnimsParent.transform.GetChild(tutorialAnimId).transform.localPosition = new Vector3(4.8f, 2.96f, 0);
            }
            tutorialAnimsParent.transform.GetChild(tutorialAnimId).GetComponent<SkeletonAnimation>().enabled = true;
            tutorialAnimsParent.transform.GetChild(tutorialAnimId).gameObject.SetActive(true);
            tutorialDevamButton.SetActive(false);
            MakaraRotater.Instance.thisCollider.enabled = true;
        }
        else if (tutorialId == 4)
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
        else if (tutorialId == 5)
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

    public void playButtonSound()
    {
        buttonSound.Play();
    }

    public void returnToMainMenu()
    {
        PlayerPrefs.SetInt("Babuş", 1);
        SceneManager.LoadSceneAsync("MainMenu");
    }

    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
