using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class PieceGame : MonoBehaviour
{
    private static PieceGame myInstance = null;

    public PieceSlider pieceSlider;

    public PieceRescaler pieceRescaler;

    public PieceRotater pieceRotater;

    public PieceGameMath pieceGameMath;

    public Transform pieceList, board, levels, currentPiece, mathSymbols;

    public GameObject inGameUi, pauseMenu, tutorial, tutorialButton, winParticles, menuArea, notPlayableText;

    public Text timeText, pauseText;

    public tk2dTextMesh tutorialText;

    public string[] tutorialTexts, tutorialAnimNames;

    public Transform numbersList, lettersList, onHoldPiece;

    bool multiTouchActive, isPieceOnHold, isInReverseMode, timeSaveLevel, canHoldAnyPiece, isMathMode, isTimeScore, isFinishing, countdownFlag;

    public bool hasReversePlay, isTooMuchRedArea, isMidRangeRedArea, isLittleRedArea;

    int touchCount, levelId, currentPieceIndex, timeToBeat;

    public int playablePieceCount, redPieceCount, redPieceSlot, bluePieceCount, bluePieceSlot, cyanPieceCount, cyanPieceSlot, greenPieceCount, greenPieceSlot, yellowPieceCount, yellowPieceSlot;

    Transform redGlow, greenGlow, whiteGlow;

    List<int> tutorialIds = new List<int>();

    Vector2 offset;

    public float timer, levelCompleteScore;

    float finishTimer;

    string currentLevelName;

    Touch touch;

    Ray screenRay;

    RaycastHit hit;

    List<Transform> activePieces = new List<Transform>();

    public AudioSource piecePlaceSound, pieceReturnSound, pieceRetakeSound, finishSound, gameOverSound, countDownSound;

    public GameState gameState;

    public string mostDifficultyArea;

    public float redRatio, blueRatio, cyanRatio, greenRatio, yellowRatio;

    public static PieceGame Instance
    {
        get
        {
            if (myInstance == null)
            {
                myInstance = FindObjectOfType(typeof(PieceGame)) as PieceGame;
            }
            if (myInstance == null)
            {
                GameObject obj = new GameObject("PieceGame");
                myInstance = obj.AddComponent(typeof(PieceGame)) as PieceGame;
            }
            return myInstance;
        }
    }

    void Awake()
    {
        RestrictionMap.getDifficultyRatios();
        redRatio = RestrictionMap.redRatio;
        blueRatio = RestrictionMap.blueRatio;
        cyanRatio = RestrictionMap.cyanRatio;
        greenRatio = RestrictionMap.greenRatio;
        yellowRatio = RestrictionMap.yellowRatio;
        mostDifficultyArea = RestrictionMap.mostAvailableArea;
    }

    void Start ()
    {
        //PlayerPrefs.SetInt("PieceGameLevelCount", 12);
        numbersList = pieceList.FindChild("Numbers");
        lettersList = pieceList.FindChild("Letters");
        activateLevels();
        gameState = GameState.isOnTutorial;
        //redAreaRatio = 55;
        playablePieceCount = 20;
        if (redRatio >= 50)
            isTooMuchRedArea = true;
        else if (redRatio > 20 && redRatio < 50)
        {
            playablePieceCount = 16 - (int)(((redRatio - 20) / 30) * 6.5f);
            isMidRangeRedArea = true;
        }
        else if (redRatio <= 20)
            isLittleRedArea = true;

        //if (isLittleRedArea)
        //{
        //    redPieceSlot = Mathf.RoundToInt(playablePieceCount * redRatio / 100);
        //    bluePieceSlot = Mathf.RoundToInt(playablePieceCount * blueRatio / 100);
        //    cyanPieceSlot = Mathf.RoundToInt(playablePieceCount * cyanRatio / 100);
        //    greenPieceSlot = Mathf.RoundToInt(playablePieceCount * greenRatio / 100);
        //    yellowPieceSlot = Mathf.RoundToInt(playablePieceCount * yellowRatio / 100);
        //    if (redRatio < 5f)
        //    {
        //        increaseMostAreaSlot(redPieceSlot);
        //        redPieceSlot = 0;
        //    }
        //    if (blueRatio < 5f)
        //    {
        //        increaseMostAreaSlot(bluePieceSlot);
        //        bluePieceSlot = 0;
        //    }
        //    if (cyanRatio < 5f)
        //    {
        //        increaseMostAreaSlot(cyanPieceSlot);
        //        cyanPieceSlot = 0;
        //    }
        //    if (greenRatio < 5f)
        //    {
        //        increaseMostAreaSlot(greenPieceSlot);
        //        greenPieceSlot = 0;
        //    }
        //    if (yellowRatio < 5f)
        //    {
        //        increaseMostAreaSlot(yellowPieceSlot);
        //        yellowPieceSlot = 0;
        //    }
        //}
        //else if(isMidRangeRedArea)
        //{
        //    redPieceSlot = 1;
        //    bluePieceSlot = (int)((playablePieceCount) * 0.3f);
        //    cyanPieceSlot = (int)((playablePieceCount) * 0.4f);
        //    greenPieceSlot = (int)((playablePieceCount) * 0.15f);
        //    yellowPieceSlot = (int)((playablePieceCount) * 0.1f);
        //    if (blueRatio < 7.5f)
        //    {
        //        increaseMostAreaSlot(bluePieceSlot);
        //        bluePieceSlot = 0;
        //    }
        //    if (cyanRatio < 7.5f)
        //    {
        //        increaseMostAreaSlot(cyanPieceSlot);
        //        cyanPieceSlot = 0;
        //    }
        //    if (greenRatio < 7.5f)
        //    {
        //        increaseMostAreaSlot(greenPieceSlot);
        //        greenPieceSlot = 0;
        //    }
        //    if (yellowRatio < 7.5f)
        //    {
        //        increaseMostAreaSlot(yellowPieceSlot);
        //        yellowPieceSlot = 0;
        //    }
        //    int tempTotal = redPieceSlot + bluePieceSlot + cyanPieceSlot + greenPieceSlot + yellowPieceSlot;
        //    if (tempTotal < playablePieceCount)
        //        increaseMostAreaSlot(playablePieceCount - tempTotal);
        //}
        //else if(isTooMuchRedArea)
        //{
        //    redPieceSlot = playablePieceCount;
        //    bluePieceSlot = playablePieceCount;
        //    cyanPieceSlot = playablePieceCount;
        //    greenPieceSlot = playablePieceCount;
        //    yellowPieceSlot = playablePieceCount;
        //}

        foreach (Renderer child in winParticles.GetComponentsInChildren<Renderer>())
            child.sortingOrder = 8;
        
        if (PlayerPrefs.GetString("RestartLevelId") != "")
        {
            getLevel(PlayerPrefs.GetString("RestartLevelId"));
            menuArea.SetActive(false);
            PlayerPrefs.SetString("RestartLevelId", "");
        }
    }

    void increaseMostAreaSlot(int count)
    {
        if (mostDifficultyArea == "Red")
            redPieceSlot += count;
        else if (mostDifficultyArea == "Blue")
            bluePieceSlot += count;
        else if (mostDifficultyArea == "Cyan")
            cyanPieceSlot += count;
        else if (mostDifficultyArea == "Green")
            greenPieceSlot += count;
        else if (mostDifficultyArea == "Yellow")
            yellowPieceSlot += count;
    }
	
	void Update ()
    {
        if (gameState == GameState.isActivePlay)
        {
            if ((!multiTouchActive || Input.touchCount == 1) && !isMathMode)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    screenRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(screenRay, out hit))
                    {
                        if (hit.transform.tag == "piece")
                        {
                            redGlow = hit.transform.FindChild("redGlow");
                            greenGlow = hit.transform.FindChild("greenGlow");
                            whiteGlow = hit.transform.FindChild("whiteGlow");
                            if (hit.transform.name == currentPiece.name)
                            {
                                isPieceOnHold = true;
                                onHoldPiece = hit.transform;
                                hit.transform.GetComponent<Piece>().isOnHold = true;
                                offset = hit.transform.position - Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
                                whiteGlow.gameObject.SetActive(true);
                            }
                            else
                            {
                                redGlow.gameObject.SetActive(true);
                            }
                        }
                        else if(canHoldAnyPiece && hit.transform.tag == "piece")
                        {
                            isPieceOnHold = true;
                            onHoldPiece = hit.transform;
                            hit.transform.GetComponent<Piece>().isOnHold = true;
                            offset = hit.transform.position - Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
                        }
                    }
                }
                if (Input.GetMouseButton(0)) //Move
                {
                    if (isPieceOnHold)
                    {
                        if (onHoldPiece != null)
                        {
                            onHoldPiece.position = (Vector2)(Camera.main.ScreenToWorldPoint(Input.mousePosition)) + offset;
                            //Debug.Log("x: " + Camera.main.WorldToScreenPoint(onHoldPiece.position).x + " y: " + Camera.main.WorldToScreenPoint(onHoldPiece.position).y);
                            if (onHoldPiece.name == currentPiece.name)
                            {
                                if (currentPiece.transform.GetComponent<Piece>().isOnTargetPosition())
                                {
                                    if (!greenGlow.gameObject.activeSelf)
                                        greenGlow.gameObject.SetActive(true);
                                }
                                else
                                {
                                    if (greenGlow.gameObject.activeSelf)
                                        greenGlow.gameObject.SetActive(false);
                                }
                            }
                        }
                        else
                        {
                            if (redGlow != null && redGlow.gameObject.activeSelf)
                                redGlow.gameObject.SetActive(false);
                        }
                    }
                }
                if (Input.GetMouseButtonUp(0))
                {
                    isPieceOnHold = false;
                    if (onHoldPiece != null)
                    {
                        if (onHoldPiece.name != currentPiece.name)
                            onHoldPiece.GetComponent<Piece>().returnPiece();
                        else
                        {
                            if (currentPiece.transform.GetComponent<Piece>().checkTargetPosition())
                            {
                                if (pieceSlider.enabled)
                                    pieceSlider.removePiece(currentPiece);
                                currentPieceIndex++;
                                if (currentPieceIndex < activePieces.Count)
                                {
                                    currentPiece = activePieces[currentPieceIndex];
                                    if (pieceSlider.enabled)
                                        pieceSlider.currentPiece = activePieces[currentPieceIndex];
                                    currentPiece.GetComponent<Piece>().shadow.gameObject.SetActive(true);
                                    if (pieceSlider.enabled)
                                        pieceSlider.currentPiece = currentPiece;
                                }
                                else
                                {
                                    if (checkFinishStatue())
                                    {
                                        if (hasReversePlay && !isInReverseMode)
                                            reverseGamePlay();
                                        else
                                            finishGame(false, timeSaveLevel, !timeSaveLevel);
                                    }
                                }
                                if (!isInReverseMode)
                                    piecePlaceSound.Play();
                                else
                                    pieceRetakeSound.Play();
                            }
                            else
                                pieceReturnSound.Play();
                        }
                        onHoldPiece = null;
                    }
                    if (!pieceRotater.enabled && !pieceRescaler.enabled)
                    {
                        if (greenGlow != null && greenGlow.gameObject.activeSelf)
                            greenGlow.gameObject.SetActive(false);
                    }
                    else
                    {
                        if (pieceRescaler.enabled)
                        {
                            if (greenGlow != null)
                            {
                                if (greenGlow.parent.GetComponent<Piece>().isOnTrueScale && !greenGlow.parent.GetComponent<Piece>().isPlaced)
                                    greenGlow.gameObject.SetActive(true);
                                else
                                    greenGlow.gameObject.SetActive(false);
                            }
                        }
                        else if (pieceRotater.enabled)
                        {
                            if (greenGlow != null)
                            {
                                if (greenGlow.parent.GetComponent<Piece>().isOnTrueRotation && !greenGlow.parent.GetComponent<Piece>().isPlaced)
                                    greenGlow.gameObject.SetActive(true);
                                else
                                    greenGlow.gameObject.SetActive(false);
                            }
                        }
                    }
                    if (redGlow != null && redGlow.gameObject.activeSelf)
                        redGlow.gameObject.SetActive(false);
                    if (whiteGlow != null && whiteGlow.gameObject.activeSelf)
                        whiteGlow.gameObject.SetActive(false);
                }
            }
            if (!isMathMode)
            {
                timer += Time.deltaTime;
                if (timeText.gameObject.activeSelf)
                {
                    timeText.text = ((int)(timeToBeat + 1 - timer)).ToString();
                    if((timeToBeat - timer) <= 3.1f && !countdownFlag)
                    {
                        countdownFlag = true;
                        countDownSound.Play();
                    }
                    if ((timeToBeat - timer) <= 0)
                    {
                        finishGame(true, timeSaveLevel, true);
                    }
                }
            }
        }
        if(isFinishing)
        {
            finishTimer += Time.deltaTime;
            if(finishTimer>=2.0f)
            {
                finishTimer = 0;
                isFinishing = false;
                pauseGame(false);
            }
        }
    }

    public void getLevel(string levelName)
    {
        levelId = int.Parse(levelName);
        currentLevelName = "PieceGameLevel" + levelId;
        if (levelName=="1")
        {
            tutorialIds.Add(0);
            tutorialIds.Add(1);
            getLevelReady(playablePieceCount, false, false, false, false, false, true, true, false, false, false);
        }
        else if (levelName == "2")
        {
            tutorialIds.Add(0);
            tutorialIds.Add(1);
            getLevelReady(playablePieceCount, false, false, false, false, false, true, false, true, false, false);
        }
        else if (levelName == "3")
        {
            tutorialIds.Add(2);
            tutorialIds.Add(3);
            getLevelReady(playablePieceCount, true, false, false, false, false, true, true, false, false, false);
        }
        else if (levelName == "4")
        {
            tutorialIds.Add(2);
            tutorialIds.Add(3);
            getLevelReady(playablePieceCount, true, false, false, false, false, true, false, true, false, false);
        }
        else if (levelName == "5")
        {
            tutorialIds.Add(6);
            getLevelReady(20, false, true, false, false, false, false, true, false, false, false);
            canHoldAnyPiece = true;
        }
        else if (levelName == "6")
        {
            tutorialIds.Add(6);
            getLevelReady(20, false, true, false, false, false, false, false, true, false, false);
            canHoldAnyPiece = true;
        }
        else if (levelName == "7")
        {
            tutorialIds.Add(4);
            getLevelReady(playablePieceCount, false, false, true, false, true, false, true, false, false, false);
        }
        else if (levelName == "8")
        {
            tutorialIds.Add(4);
            getLevelReady(playablePieceCount, false, false, true, false, true, false, false, true, false, false);
        }
        else if (levelName == "9")
        {
            tutorialIds.Add(5);
            getLevelReady(playablePieceCount, false, false, false, true, true, false, true, false, false, false);
        }
        else if (levelName == "10")
        {
            tutorialIds.Add(5);
            getLevelReady(playablePieceCount, false, false, false, true, true, false, false, true, false, false);
        }
        else if (levelName == "11")
        {
            tutorialIds.Add(7);
            tutorialIds.Add(8);
            tutorialIds.Add(9);
            getLevelReady(2, false, false, false, false, true, false, false, false, true, true);
            canHoldAnyPiece = true;
        }
        else if (levelName == "12")
        {
            tutorialIds.Add(10);
            getLevelReady(2, false, false, false, false, true, false, false, false, true, false);
            canHoldAnyPiece = true;
        }
    }

    void getLevelReady(int pieceCount, bool isMixedWithLetters, bool isStandinInLine, bool isRotationChanged, bool isScaleChanged, bool isMultiTouch, bool isReverse, bool timeSaver, bool showTime, bool mathLevel, bool showingMathSymbols)
    {
        isMathMode = mathLevel;
        if (showTime)
        {
            timeToBeat = PlayerPrefs.GetInt(currentLevelName + "Time");
            timeText.text = ((int)(timeToBeat - timer)).ToString();
            timeText.gameObject.SetActive(true);
        }
        if (!mathLevel)
        {
            if (!isMixedWithLetters)
            {
                for (int i = 0; i < pieceCount; i++)
                {
                    activePieces.Add(numbersList.GetChild(i));
                    numbersList.GetChild(i).GetComponent<Piece>().shufflePosition(isStandinInLine, isRotationChanged, isScaleChanged, mathLevel, false);
                }
            }
            else
            {
                for (int i = 0; i < pieceCount / 4; i++)
                {
                    activePieces.Add(numbersList.GetChild(i));
                    numbersList.GetChild(i).GetComponent<Piece>().shufflePosition(isStandinInLine, isRotationChanged, isScaleChanged, mathLevel, false);
                }
                for (int i = 0; i < pieceCount / 4; i++)
                {
                    activePieces.Add(lettersList.GetChild(i));
                    lettersList.GetChild(i).GetComponent<Piece>().shufflePosition(isStandinInLine, isRotationChanged, isScaleChanged, mathLevel, false);
                }
                for (int i = pieceCount / 4; i < pieceCount / 2; i++)
                {
                    numbersList.GetChild(i).position = new Vector2(numbersList.GetChild(i).position.x - 6.75f, -4.21f);
                    activePieces.Add(numbersList.GetChild(i));
                    numbersList.GetChild(i).GetComponent<Piece>().shufflePosition(isStandinInLine, isRotationChanged, isScaleChanged, mathLevel, false);
                }
                for (int i = pieceCount / 4; i < pieceCount / 2; i++)
                {
                    activePieces.Add(lettersList.GetChild(i));
                    lettersList.GetChild(i).GetComponent<Piece>().shufflePosition(isStandinInLine, isRotationChanged, isScaleChanged, mathLevel, false);
                }
            }
            currentPiece = activePieces[0];
            currentPieceIndex = 0;
            currentPiece.GetComponent<Piece>().shadow.gameObject.SetActive(true);
            board.gameObject.SetActive(true);
        }
        else
        {
            pieceGameMath.enabled = true;
            pieceGameMath.showingSymbols = showingMathSymbols;
            getMathOperation("Add", numbersList.GetChild(3), numbersList.GetChild(5), showingMathSymbols);
        }
        if (isStandinInLine)
        {
            pieceSlider.enabled = true;
            pieceSlider.shufflePieces();
            pieceSlider.currentPiece = activePieces[0];
        }
        if(isScaleChanged)
        {
            pieceRescaler.enabled = true;
        }
        if(isRotationChanged)
        {
            pieceRotater.enabled = true;
        }
        isTimeScore = showTime;
        multiTouchActive = isMultiTouch;
        hasReversePlay = isReverse;
        timeSaveLevel = timeSaver;
        inGameUi.SetActive(true);
        getTutorialScene(tutorialIds[0], showTime);
        if (isTooMuchRedArea)
            notPlayableText.SetActive(true); 
    }

    bool checkFinishStatue()
    {
        bool finishStatue = true;
        for (int i = 0; i < activePieces.Count; i++)
        {
            if (!activePieces[i].GetComponent<Piece>().isPlaced)
                finishStatue = false;
        }
        return finishStatue;
    }

    void reverseGamePlay()
    {
        for (int i = 0; i < activePieces.Count; i++)
        {
            activePieces[i].GetComponent<Piece>().reverse();
        }
        currentPieceIndex = 0;
        currentPiece = activePieces[0];
        currentPiece.GetComponent<Piece>().shadow.gameObject.SetActive(true);
        isInReverseMode = true;
        getTutorialScene(tutorialIds[1], timeText.gameObject.activeSelf);
    }

    public void finishGame(bool isFail, bool saveTime, bool isTimeScore)
    {
        gameState = GameState.isFinished;
        if (saveTime)
        {
            if (!isFail)
            {
                if ((int)(timer * 0.9f) < PlayerPrefs.GetInt("PieceGameLevel" + (levelId + 1) + "Time") || PlayerPrefs.GetInt("PieceGameLevel" + (levelId + 1) + "Time") == 0)
                    PlayerPrefs.SetInt("PieceGameLevel" + (levelId + 1) + "Time", (int)(timer * 0.9f));
            }
            if (PlayerPrefs.GetFloat("Level" + levelId + "Score", 0) == 0 || PlayerPrefs.GetFloat("Level" + levelId + "Score", 0) < levelCompleteScore)
                PlayerPrefs.SetFloat("Level" + levelId + "Score", levelCompleteScore);
        }
        else
        {
            if (PlayerPrefs.GetFloat("Level" + levelId + "Score", 0) == 0 || PlayerPrefs.GetFloat("Level" + levelId + "Score", 0) < levelCompleteScore)
                PlayerPrefs.SetFloat("Level" + levelId + "Score", levelCompleteScore);
        }
        if (isFail)
        {
            gameOverSound.Play();
            //Get Game Over Scene
            pauseText.text = "Üzgünüm\nSüreniz Bitti !";
            pauseGame(false);
        }
        else
        {
            finishSound.Play();
            //Get Finish Scene
            if (PlayerPrefs.GetInt("PieceGameLevelCount", 1) == levelId && PlayerPrefs.GetInt("PieceGameLevelCount", 1) < levels.transform.childCount)
                PlayerPrefs.SetInt("PieceGameLevelCount", PlayerPrefs.GetInt("PieceGameLevelCount", 1) + 1);
            pauseText.text = "Tebrikler\nBölümü Tamamladın !";
            winParticles.SetActive(false);
            winParticles.SetActive(true);
            isFinishing = true;
        }
        timer = 0;
        timeText.gameObject.SetActive(false);
        if(mathSymbols.gameObject.activeSelf)
            mathSymbols.gameObject.SetActive(false);
    }

    public void restartGame()
    {
        for (int i = 0; i < activePieces.Count; i++)
        {
            activePieces[i].GetComponent<Piece>().resetPiece();
        }
        activePieces.Clear();
        timer = 0;
        timeText.gameObject.SetActive(false);
        if (PlayerPrefs.GetFloat("Level" + levelId + "Score", 0) == 0 || PlayerPrefs.GetFloat("Level" + levelId + "Score", 0) < levelCompleteScore)
            PlayerPrefs.SetFloat("Level" + levelId + "Score", levelCompleteScore);
        PlayerPrefs.SetString("RestartLevelId", levelId.ToString());
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
    }

    public void returnToLevelSelect()
    {
        if (PlayerPrefs.GetFloat("Level" + levelId + "Score", 0) == 0 || PlayerPrefs.GetFloat("Level" + levelId + "Score", 0) < levelCompleteScore)
                PlayerPrefs.SetFloat("Level" + levelId + "Score", levelCompleteScore);
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
    }

    public void returnToMainMenu()
    {
        PlayerPrefs.SetInt("Babuş", 1);
        SceneManager.LoadSceneAsync("MainMenu");
    }

    public void pauseGame(bool isPause)
    {
        pauseMenu.SetActive(true);
        if (isPause)
        {
            gameState = GameState.isOnPause;
            pauseText.text = "Oyun Durduruldu !";
            pauseMenu.transform.FindChild("Resume").gameObject.SetActive(true);
        }
        else
        {
            gameState = GameState.isFinished;
            pauseMenu.transform.FindChild("Resume").gameObject.SetActive(false);
        }
    }

    public void resumeGame()
    {
        gameState = GameState.isActivePlay;
        pauseMenu.SetActive(false);
    }

    void activateLevels()
    {
        for (int i = 0; i < PlayerPrefs.GetInt("PieceGameLevelCount", 1); i++)
        {
            levels.transform.GetChild(i).GetComponent<Button>().enabled = true;
            levels.transform.GetChild(i).GetComponent<Image>().enabled = true;
            levels.transform.GetChild(i).FindChild("LevelNumber").gameObject.SetActive(true);
            levels.transform.GetChild(i).FindChild("Score").gameObject.SetActive(true);
            levels.transform.GetChild(i).FindChild("Score").GetComponent<Text>().text = "%" + PlayerPrefs.GetFloat("Level" + (i + 1) + "Score", 0) + "\ntamamlandı";
            levels.transform.GetChild(i).FindChild("Locked").gameObject.SetActive(false);
        }
    }

    public void getTutorialScene(int tutorialId, bool timeTextActive)
    {
        pieceList.gameObject.SetActive(false);
        gameState = GameState.isOnTutorial;
        inGameUi.SetActive(false);
        tutorial.SetActive(true);
        tutorial.GetComponent<SkeletonAnimation>().AnimationName = tutorialAnimNames[tutorialId];
        tutorialText.text = tutorialTexts[tutorialId];
        tutorialButton.SetActive(true);
        if (timeTextActive)
            tutorial.transform.FindChild("timeText").gameObject.SetActive(true);
        if (isMathMode)
            mathSymbols.gameObject.SetActive(false);
    }
    public void removeTutorialScene()
    {
        pieceList.gameObject.SetActive(true);
        tutorial.SetActive(false);
        inGameUi.SetActive(true);
        gameState = GameState.isActivePlay;
        tutorialButton.SetActive(false);
        if(isMathMode)
        {
            mathSymbols.gameObject.SetActive(true);
            if (tutorial.GetComponent<SkeletonAnimation>().AnimationName == "tutorial_09_multiply")
            {
                getMathOperation("Multiply", numbersList.GetChild(2), numbersList.GetChild(4), pieceGameMath.showingSymbols);
            }
            else if (tutorial.GetComponent<SkeletonAnimation>().AnimationName == "tutorial_10_divide")
            {
                getMathOperation("Divide", numbersList.GetChild(3), null, pieceGameMath.showingSymbols);
            }
        }
    }

    public void getMathOperation(string type, Transform piece1, Transform piece2, bool isShowingSymbol)
    {
        mathSymbols.FindChild("Add").gameObject.SetActive(false);
        mathSymbols.FindChild("Divide").gameObject.SetActive(false);
        mathSymbols.FindChild("Multiply").gameObject.SetActive(false);
        mathSymbols.FindChild("Result").gameObject.SetActive(false);
        int value1 = 0, value2 = 0;

        piece1.GetComponent<Piece>().shufflePosition(false, false, false, true, true);
        value1 = int.Parse(piece1.FindChild("pieceText").GetComponent<tk2dTextMesh>().text);
        PieceGameMath.Instance.value1 = value1;
        if (piece2 != null)
        {
            while (piece1 == piece2)
                piece2 = numbersList.GetChild(Random.Range(0, 10));
            value2 = int.Parse(piece2.FindChild("pieceText").GetComponent<tk2dTextMesh>().text);
            piece2.GetComponent<Piece>().shufflePosition(false, false, false, true, false);
            PieceGameMath.Instance.value2 = value2;
        }
        if (type != "")
        {
            if (type == "Add")
            {
                pieceGameMath.targetResult = value1 + value2;
            }
            else if (type == "Multiply")
            {
                pieceGameMath.targetResult = value1 * value2;
            }
            else if (type == "Divide")
            {
                pieceGameMath.targetResult = value1 / 2;
            }

            pieceGameMath.currentOperation = type;
            if (isShowingSymbol)
                mathSymbols.FindChild(type).gameObject.SetActive(true);
            else
            {
                mathSymbols.FindChild("Result").GetComponent<Text>().text = pieceGameMath.targetResult.ToString();
                mathSymbols.FindChild("Result").gameObject.SetActive(true);
            }
        }
        else
        {
            int typeRandom = Random.Range(0, 3);
            if(typeRandom==0)
            {
                pieceGameMath.targetResult = value1 + value2;
                pieceGameMath.currentOperation = "Add";
            }
            else if (typeRandom == 1)
            {
                pieceGameMath.targetResult = value1 * value2;
                pieceGameMath.currentOperation = "Multiply";
            }
            else if (typeRandom == 2)
            {
                if (value1 % 2 == 0)
                {
                    pieceGameMath.targetResult = value1 / 2;
                    piece2.GetComponent<Piece>().resetPiece();
                }
                else if (value2 % 2 == 0)
                {
                    pieceGameMath.targetResult = value2 / 2;
                    piece1.GetComponent<Piece>().resetPiece();
                }
                else
                {
                    int random = Random.Range(1, 10);
                    while ((random + 1) % 2 != 0)
                        random = Random.Range(0, 10);
                    piece1.GetComponent<Piece>().resetPiece();
                    piece2.GetComponent<Piece>().resetPiece();
                    piece1 = numbersList.GetChild(random);
                    piece1.GetComponent<Piece>().shufflePosition(false, false, false, true, true);
                    value1 = int.Parse(piece1.FindChild("pieceText").GetComponent<tk2dTextMesh>().text);
                    pieceGameMath.targetResult = value1 / 2;
                }
                pieceGameMath.currentOperation = "Divide";
            }
            if (isShowingSymbol)
                mathSymbols.FindChild(pieceGameMath.currentOperation).gameObject.SetActive(true);
            else
            {
                mathSymbols.FindChild("Result").GetComponent<Text>().text = pieceGameMath.targetResult.ToString();
                mathSymbols.FindChild("Result").gameObject.SetActive(true);
            }
        }
    }
}
public enum GameState
{
    isOnTutorial,
    isActivePlay,
    isOnPause,
    isFinished
}
