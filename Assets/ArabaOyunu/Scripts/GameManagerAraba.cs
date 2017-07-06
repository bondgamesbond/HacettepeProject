using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using DG.Tweening;

public class GameManagerAraba : MonoBehaviour
{
    public static float INITIAL_ALPHA = 1920 * 1080; 

    public InputField minScoreField, pointDurationField, minPointsField, maxPointsField, dragResetIntervalField;

    public float totalAlphaValueToChangeLevel, totalAlphaCheckInterval;

    private float defaultValueToChangeLevel;

    private float lastTotalAlphaCheckTime;

    public float textCheckInterval = 1f;

    public Text progressText;

    public DragDetectorAraba dragDetector;

    public Image levelChangeGlowImage;

    public GameObject winParticles, finishDialog, dragCanvas;

    public RCC_CarControllerV3 cilaCarController;

    public GameObject outroCamera, mainCamera, raceTrack, reflectionProbe;

    public SpriteRenderer cilaCarSprite;

    public string[] feedbackTexts;
    public string[] wrongFeedbackTexts;
    /* Different mask cameras for erasing the texture */
    public GameObject[] maskCameras;

    /* Singleton instance */
    public static GameManagerAraba Instance;

    /* Current mask camera and the index at maskCameras array */
    [HideInInspector]
    public GameObject currentMaskCameraObject;
    private int currentMaskCameraIndex;

    MaskCamera currentMaskCamera;

    /* Level information of the game */
    public int level;

	/* Feedback text */
	public Text feedbackText;

    private float defaultScoreToAccept, defaultPointUsageDuration, defaultDragTypeResetInterval;
    private int defaultMinPointsCount, defaultMaxPointsCount;

    private float resetScoreToAccept, resetPointUsageDuration, resetDragTypeResetInterval;
    private int resetMinPointsCount, resetMaxPointsCount;

    private float initialTotalAlpha = INITIAL_ALPHA;

    [HideInInspector]
    public float currentTotalAlpha;

    [HideInInspector]
    public bool introFinished, totalAlphaCalculated, calculating;

    private float alphaDifferenceToChangeLevel;

    private bool levelChanging;

    void Awake()
    {
        /* Provide Singleton functionality */
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        defaultValueToChangeLevel = totalAlphaValueToChangeLevel;
        alphaDifferenceToChangeLevel = initialTotalAlpha - totalAlphaValueToChangeLevel;

        defaultScoreToAccept = dragDetector.scoreToAccept;
        defaultPointUsageDuration = dragDetector.pointUsageDuration;
        defaultMinPointsCount = dragDetector.minPointsCount;
        defaultMaxPointsCount = dragDetector.maxPointsCount;
        defaultDragTypeResetInterval = dragDetector.dragTypeResetInterval;

        SaveDragResetValues();
        PrintDragValues();
    }

    void Start()
    {
        levelChangeGlowImage.material.color = new Color(1f, 1f, 1f, 0f);

        /* Initialize the mask camera properties */
        currentMaskCameraIndex = 1;
        currentMaskCameraObject = maskCameras[currentMaskCameraIndex - 1];

        SetCurrentMaskCamera();
    }

    float progress;
    void Update()
    {
        if (currentMaskCameraObject.activeSelf && currentMaskCamera.firstTextureApplyPassed && !totalAlphaCalculated && lastTotalAlphaCheckTime + totalAlphaCheckInterval < Time.time)
        {
            if (!calculating)
            {
                calculating = true;
                currentMaskCamera.StartCalculatingTotalAlpha();
            }  
        }

        if (totalAlphaCalculated)
        {
            progress = ((initialTotalAlpha - currentTotalAlpha) / alphaDifferenceToChangeLevel) * 100;
            //Debug.Log("progress: " + (initialTotalAlpha - currentTotalAlpha) + " / " + alphaDifferenceToChangeLevel);
            if (progress > 100f)
            {
                progress = 100f;
            }
            progressText.text = progress.ToString("F1") + "%";

            if (currentTotalAlpha <= totalAlphaValueToChangeLevel)
            {
                if (!levelChanging)
                {
                    levelChanging = true;
                    currentMaskCamera.dragDetector.enabled = false;
                    levelChangeGlowImage.material.DOFade(0.75f, 1).OnComplete(() =>
                    {
                        ChangeToNextMaskCamera();
                        levelChangeGlowImage.material.DOFade(0f, 1).OnComplete(() =>
                        {
                            if (currentMaskCameraIndex <= maskCameras.Length)
                            {
                                if (currentMaskCamera.recognizer != null)
                                {
                                    currentMaskCamera.dragDetector.enabled = true;
                                }
                                else
                                {
                                    currentMaskCamera.dragDetector.enabled = false;
                                }
                                levelChanging = false;
                            }
                            else
                            {
                                feedbackText.text = "Tebrikler!";
                                winParticles.SetActive(true);
                                finishDialog.SetActive(true);
                                cilaCarController.gasInput = 1f;
                            }
                        });
                    });
                }
            }

            lastTotalAlphaCheckTime = Time.time;
            totalAlphaCalculated = false;
        }
    }

    public void PrintDragValues()
    {
        minScoreField.text = dragDetector.scoreToAccept.ToString();
        pointDurationField.text = dragDetector.pointUsageDuration.ToString();
        minPointsField.text = dragDetector.minPointsCount.ToString();
        maxPointsField.text = dragDetector.maxPointsCount.ToString();
        dragResetIntervalField.text = dragDetector.dragTypeResetInterval.ToString();
    }

    public void ClearDragValuesFields()
    {
        minScoreField.text = "";
        pointDurationField.text = "";
        minPointsField.text = "";
        maxPointsField.text = "";
        dragResetIntervalField.text = "";
    }

    public void SetDragValues()
    {
        dragDetector.scoreToAccept = float.Parse(minScoreField.text);
        dragDetector.pointUsageDuration = float.Parse(pointDurationField.text);
        dragDetector.minPointsCount = int.Parse(minPointsField.text);
        dragDetector.maxPointsCount = int.Parse(maxPointsField.text);
        dragDetector.dragTypeResetInterval = float.Parse(dragResetIntervalField.text);
    }

    void SaveDragResetValues()
    {
        resetScoreToAccept = dragDetector.scoreToAccept;
        resetPointUsageDuration = dragDetector.pointUsageDuration;
        resetMinPointsCount = dragDetector.minPointsCount;
        resetMaxPointsCount =  dragDetector.maxPointsCount;
        resetDragTypeResetInterval = dragDetector.dragTypeResetInterval;
    }

    public void ResetDragValues()
    {
        dragDetector.scoreToAccept = resetScoreToAccept;
        dragDetector.pointUsageDuration = resetPointUsageDuration;
        dragDetector.minPointsCount = resetMinPointsCount;
        dragDetector.maxPointsCount = resetMaxPointsCount;
        dragDetector.dragTypeResetInterval = resetDragTypeResetInterval;

        PrintDragValues();
    }

    void SetCurrentMaskCamera()
    {
        if (currentMaskCamera != null)
        {
            currentMaskCamera.StopCalculatingTotalAlpha();
        }

        currentMaskCamera = currentMaskCameraObject.GetComponent<MaskCamera>();
        currentMaskCamera.dragDetector = dragDetector;
        if (currentMaskCamera.recognizer != null)
        {
            currentMaskCamera.dragDetector.recognizer = currentMaskCamera.recognizer;

            if (level == 2)
            {
                defaultScoreToAccept = dragDetector.scoreToAccept;
                defaultPointUsageDuration = dragDetector.pointUsageDuration;
                defaultMinPointsCount = dragDetector.minPointsCount;
                defaultMaxPointsCount = dragDetector.maxPointsCount;
                defaultDragTypeResetInterval = dragDetector.dragTypeResetInterval;

                dragDetector.scoreToAccept = 0.5f;
                dragDetector.pointUsageDuration = 2.5f;
                dragDetector.minPointsCount = 4;
                dragDetector.maxPointsCount = 0;
                dragDetector.dragTypeResetInterval = 2f;
                dragDetector.allMotionsAllowed = false;

                SaveDragResetValues();
                PrintDragValues();
            }
            else
            {
                dragDetector.scoreToAccept = defaultScoreToAccept;
                dragDetector.pointUsageDuration = defaultPointUsageDuration;
                dragDetector.minPointsCount = defaultMinPointsCount;
                dragDetector.maxPointsCount = defaultMaxPointsCount;
                dragDetector.dragTypeResetInterval = defaultDragTypeResetInterval;
                if (level == 3)
                {
                    dragDetector.allMotionsAllowed = true;
                }
                else
                {
                    dragDetector.allMotionsAllowed = false;
                }

                SaveDragResetValues();
                PrintDragValues();
            }
        }

        initialTotalAlpha = INITIAL_ALPHA;
        currentTotalAlpha = INITIAL_ALPHA;
        //if (level == 2)
        //{
        //    totalAlphaValueToChangeLevel = 800000f;
        //}
        //else
        //{
            totalAlphaValueToChangeLevel = defaultValueToChangeLevel;
        //}
        alphaDifferenceToChangeLevel = initialTotalAlpha - totalAlphaValueToChangeLevel;

        progressText.text = "0.0%";
        totalAlphaCalculated = false;
        calculating = false;
    }

    /// <summary>
    /// Change the current mask camera to the next one in the scene.
    /// </summary>
    public void ChangeToNextMaskCamera()
    {
        /* Deactivate the current one */
        currentMaskCameraObject.SetActive(false);

        /* Increment the index of the mask camera */
        currentMaskCameraIndex++;

        /* Increase the level */
        level++;

        /* If there are mask cameras left get the next camera */
        if (currentMaskCameraIndex <= maskCameras.Length)
        {
            currentMaskCameraObject.GetComponent<MaskCamera>().objectToErase.SetActive(false);
            currentMaskCameraObject = maskCameras[currentMaskCameraIndex - 1];
            currentMaskCameraObject.SetActive(true);

            SetCurrentMaskCamera();

            switch (level)
            {
                case 0:
                    feedbackText.text = feedbackTexts[0];
                    break;
                case 1:
                    feedbackText.text = feedbackTexts[1];
                    break;
                case 2:
                    feedbackText.text = feedbackTexts[2];
                    break;
                case 3:
                    //feedbackText.text = "Lütfen istediðiniz bir hareketle aracý kurulayýnýz!";
                    feedbackText.text = feedbackTexts[3];
                    break;
                default:
                    feedbackText.text = "Lütfen istediðiniz bir hareketle araca cila atýnýz!";
                    break;
            }
        }
        else
        {
            currentMaskCamera.dragDetector.enabled = false;
            currentMaskCamera.StopCalculatingTotalAlpha();
            totalAlphaCalculated = false;
            currentMaskCameraObject.GetComponent<MaskCamera>().objectToErase.SetActive(false);
            currentMaskCameraObject.SetActive(false);
            cilaCarSprite.gameObject.SetActive(false);
            dragCanvas.SetActive(false);
            cilaCarController.gameObject.SetActive(true);
            raceTrack.SetActive(true);
            reflectionProbe.SetActive(true);
            mainCamera.SetActive(false);
            outroCamera.SetActive(true);
        }
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
