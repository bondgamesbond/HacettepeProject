﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class PairGameManager : MonoBehaviour
{
    public static PairGameManager Instance;

    public Transform circlePiecesHead, squarePiecesHead;

    public PairGamePiece firtPiece, secondPiece;

    Transform onClearPiece;

    public RectTransform warningPopUp;

    public GameObject snowParticle, tutorial, popUp, finishMenu, winParticles, motionCanvas;

    public SkeletonAnimation thumbsUp1, thumbsUp2;

    public SpriteRenderer snowBackgroundSprite;

    public Text popUpText, warningText;

    public AudioSource buttonSound, wrongPairSound, correctPairSound, winSound, clearSnowSound;

    public List<Transform> activePiecesList;

    List<Renderer> activePiecesRenderersList;

    List<PairGamePiece> activePieceScriptList;

    public float maxXPos, minXPos, maxYPos, minYPos;

    int circlePieceCount, squarePieceCount, tempPieceId;

    bool isOverLap, isGameOver, isReadyToPlay, isPaused, isFirstSquareMoveCompleted;

    RaycastHit2D hit;

    Vector2 worldPoint, squarePieceFirstPos, squarePieceTempPos;

    public DragDetector motionDetecter;

    public Image warningPopUpImage;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        activePiecesList = new List<Transform>();
        activePiecesRenderersList = new List<Renderer>();
        activePieceScriptList = new List<PairGamePiece>();
        RestrictionMap.getDifficultyRatios();
        motionCanvas.SetActive(false);
    }

    void Start ()
    {
        if (RestrictionMap.redRatio <= 15)
        {
            while (circlePieceCount < 3)
            {
                tempPieceId = Random.Range(0, circlePiecesHead.childCount);
                if (tempPieceId % 2 == 0 && !activePiecesList.Contains(circlePiecesHead.GetChild(tempPieceId)))
                {
                    activePiecesList.Add(circlePiecesHead.GetChild(tempPieceId));
                    activePiecesList.Add(circlePiecesHead.GetChild(tempPieceId + 1));
                    activePiecesRenderersList.Add(circlePiecesHead.GetChild(tempPieceId).GetComponent<Renderer>());
                    activePiecesRenderersList.Add(circlePiecesHead.GetChild(tempPieceId + 1).GetComponent<Renderer>());
                    activePieceScriptList.Add(circlePiecesHead.GetChild(tempPieceId).GetComponent<PairGamePiece>());
                    activePieceScriptList.Add(circlePiecesHead.GetChild(tempPieceId + 1).GetComponent<PairGamePiece>());
                    circlePieceCount++;
                }
            }
            while (squarePieceCount < 3)
            {
                tempPieceId = Random.Range(0, squarePiecesHead.childCount);
                if (tempPieceId % 2 == 0 && !activePiecesList.Contains(squarePiecesHead.GetChild(tempPieceId)))
                {
                    activePiecesList.Add(squarePiecesHead.GetChild(tempPieceId));
                    activePiecesList.Add(squarePiecesHead.GetChild(tempPieceId + 1));
                    activePiecesRenderersList.Add(squarePiecesHead.GetChild(tempPieceId).GetComponent<Renderer>());
                    activePiecesRenderersList.Add(squarePiecesHead.GetChild(tempPieceId + 1).GetComponent<Renderer>());
                    activePieceScriptList.Add(squarePiecesHead.GetChild(tempPieceId).GetComponent<PairGamePiece>());
                    activePieceScriptList.Add(squarePiecesHead.GetChild(tempPieceId + 1).GetComponent<PairGamePiece>());
                    squarePieceCount++;
                }
            }
        }
        else
        {
            while (circlePieceCount < 2)
            {
                tempPieceId = Random.Range(0, circlePiecesHead.childCount);
                if (tempPieceId % 2 == 0 && !activePiecesList.Contains(circlePiecesHead.GetChild(tempPieceId)))
                {
                    activePiecesList.Add(circlePiecesHead.GetChild(tempPieceId));
                    activePiecesList.Add(circlePiecesHead.GetChild(tempPieceId + 1));
                    activePiecesRenderersList.Add(circlePiecesHead.GetChild(tempPieceId).GetComponent<Renderer>());
                    activePiecesRenderersList.Add(circlePiecesHead.GetChild(tempPieceId + 1).GetComponent<Renderer>());
                    activePieceScriptList.Add(circlePiecesHead.GetChild(tempPieceId).GetComponent<PairGamePiece>());
                    activePieceScriptList.Add(circlePiecesHead.GetChild(tempPieceId + 1).GetComponent<PairGamePiece>());
                    circlePieceCount++;
                }
            }
            while (squarePieceCount < 2)
            {
                tempPieceId = Random.Range(0, squarePiecesHead.childCount);
                if (tempPieceId % 2 == 0 && !activePiecesList.Contains(squarePiecesHead.GetChild(tempPieceId)))
                {
                    activePiecesList.Add(squarePiecesHead.GetChild(tempPieceId));
                    activePiecesList.Add(squarePiecesHead.GetChild(tempPieceId + 1));
                    activePiecesRenderersList.Add(squarePiecesHead.GetChild(tempPieceId).GetComponent<Renderer>());
                    activePiecesRenderersList.Add(squarePiecesHead.GetChild(tempPieceId + 1).GetComponent<Renderer>());
                    activePieceScriptList.Add(squarePiecesHead.GetChild(tempPieceId).GetComponent<PairGamePiece>());
                    activePieceScriptList.Add(squarePiecesHead.GetChild(tempPieceId + 1).GetComponent<PairGamePiece>());
                    squarePieceCount++;
                }
            }
        }
        for (int i = 0; i < activePiecesList.Count; i++)
        {
            shufflePiece(activePiecesList[i], activePiecesRenderersList[i], activePieceScriptList[i]);
        }
        isPaused = true;
        tutorial.SetActive(true);
    }
	
	void Update ()
    {
        if (!isGameOver && !isPaused)
        {
            if (isReadyToPlay)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    hit = Physics2D.Raycast(worldPoint, Vector2.zero);
                    if (hit.collider != null)
                    {
                        if (hit.transform.tag == "PairGamePiece")
                        {
                            if (firtPiece == null || (firtPiece != null && !firtPiece.isClear && firtPiece.name == hit.transform.name)
                                || (firtPiece != null && firtPiece.isClear && secondPiece == null) || (firtPiece != null && secondPiece != null && !secondPiece.isClear && secondPiece.name == hit.transform.name))
                            {
                                onClearPiece = hit.transform;
                                if (hit.transform.name.Contains("Circle"))
                                    motionDetecter.isOnObject = true;
                                else
                                {
                                    squarePieceFirstPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                                    isFirstSquareMoveCompleted = false;
                                }
                            }
                        }
                    }
                }
                else if (Input.GetMouseButton(0))
                {
                    worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    hit = Physics2D.Raycast(worldPoint, Vector2.zero);
                    if (hit.collider != null)
                    {
                        if (firtPiece == null || (firtPiece != null && !firtPiece.isClear && firtPiece.name == hit.transform.name)
                                   || (firtPiece != null && firtPiece.isClear && secondPiece == null) || (firtPiece != null && secondPiece != null && !secondPiece.isClear && secondPiece.name == hit.transform.name))
                        {
                            if (onClearPiece != hit.transform)
                                onClearPiece = hit.transform;
                            if (hit.transform.name.Contains("Circle"))
                                motionDetecter.isOnObject = true;
                            else
                            {
                                squarePieceTempPos= Camera.main.ScreenToWorldPoint(Input.mousePosition);
                                if (squarePieceFirstPos == Vector2.zero)
                                    squarePieceFirstPos = squarePieceTempPos;
                                if (Mathf.Abs(squarePieceFirstPos.x - squarePieceTempPos.x) >= 1.5f && Mathf.Abs(squarePieceFirstPos.y - squarePieceTempPos.y) <= 0.4f)
                                {
                                    if(!isFirstSquareMoveCompleted)
                                    {
                                        isFirstSquareMoveCompleted = true;
                                        squarePieceFirstPos = squarePieceTempPos;
                                    }
                                    else
                                    {
                                        squarePieceFirstPos = squarePieceTempPos;
                                        isFirstSquareMoveCompleted = false;
                                        clearActivePiece();
                                    }
                                }
                            }
                        }
                        else
                        {
                            onClearPiece = null;
                            motionDetecter.isOnObject = false;
                            squarePieceTempPos = Vector2.zero;
                            squarePieceFirstPos = Vector2.zero;
                        }
                    }
                    else
                    {
                        if (onClearPiece != null)
                        {
                            onClearPiece = null;
                            motionDetecter.isOnObject = false;
                            squarePieceTempPos = Vector2.zero;
                            squarePieceFirstPos = Vector2.zero;
                        }
                    }
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    hit = Physics2D.Raycast(worldPoint, Vector2.zero);
                    if (hit.collider != null)
                    {
                        if (hit.transform.tag == "PairGamePiece")
                        {
                            onClearPiece = null;
                            motionDetecter.isOnObject = false;
                            squarePieceTempPos = Vector2.zero;
                            squarePieceFirstPos = Vector2.zero;
                        }
                    }
                }
            }

            if (warningPopUpActive)
            {
                tempWarningColor.a -= Time.deltaTime * 1.0f;
                tempColorBlack.a -= Time.deltaTime * 1.0f;
                if (tempWarningColor.a <= 0)
                {
                    warningPopUp.gameObject.SetActive(false);
                    warningPopUpActive = false;
                }
                warningPopUpImage.color = tempWarningColor;
                warningText.color = tempColorBlack;
            }
        }
	}

    public void clearActivePiece()
    {
        if (onClearPiece != null && !onClearPiece.GetComponent<PairGamePiece>().isClear)
        {
            activePieceScriptList[activePiecesList.IndexOf(onClearPiece)].clearPiece();
            clearSnowSound.Play();
        }
    }

    int tempDifficulty;
    void shufflePiece(Transform piece, Renderer pieceRanderer, PairGamePiece pieceScript)
    {
        isOverLap = false;
        piece.gameObject.SetActive(true);
        piece.localPosition = new Vector2(Random.Range(minXPos, maxXPos), Random.Range(minYPos, maxYPos));
        for (int i = 0; i < activePiecesList.Count; i++)
        {
            if (piece.name != activePiecesList[i].name && activePiecesList[i].gameObject.activeSelf)
            {
                if (pieceRanderer.bounds.Intersects(activePiecesRenderersList[i].bounds))
                    isOverLap = true;
            }
        }

        if (isOverLap)
            shufflePiece(piece, pieceRanderer, pieceScript);
        else
        {
            tempDifficulty = RestrictionMap.findDifficulty(piece.position);
            if(tempDifficulty == 0)
                pieceScript.setPieceActive(2);
            else if (tempDifficulty == 1)
                pieceScript.setPieceActive(3);
            else if (tempDifficulty == 2)
                pieceScript.setPieceActive(4);
            else if (tempDifficulty == 3 || tempDifficulty == 4 || tempDifficulty == 5)
                pieceScript.setPieceActive(1);

        }
    }

    public void startGame()
    {
        isPaused = false;
        tutorial.SetActive(false);
        StartCoroutine(gameStarter());
        buttonSound.Play();
    }

    public void resumeGame()
    {
        isPaused = false;
        popUp.SetActive(false);
        buttonSound.Play();
    }

    public void buttonSoundPlayer()
    {
        buttonSound.Play();
    }

    public void getPopUp(int id)
    {
        if (id == 0)
        {
            popUpText.text = "";
        }
        else if (id == 1)
        {
            popUpText.text = "";
        }
        else if (id == 2)
        {
            popUpText.text = "";
        }
        popUp.SetActive(true);
        isPaused = true;
    }

    Color tempColor;
    IEnumerator gameStarter()
    {
        yield return new WaitForSeconds(2f);
        snowParticle.SetActive(true);
        yield return new WaitForSeconds(6.0f);
        tempColor = Color.white;
        tempColor.a = 0;
        for (int i = 0; i < activePieceScriptList.Count; i++)
        {
            activePieceScriptList[i].firstSnow.color = tempColor;
            activePieceScriptList[i].firstSnow.gameObject.SetActive(true);
        }
        snowBackgroundSprite.color = tempColor;
        snowBackgroundSprite.gameObject.SetActive(true);
        while (tempColor.a < 1)
        {
            tempColor.a += 0.04f;
            yield return new WaitForSeconds(Time.deltaTime * 4.0f);
            snowBackgroundSprite.color = tempColor;
            for (int i = 0; i < activePieceScriptList.Count; i++)
                activePieceScriptList[i].firstSnow.color = tempColor;
        }
        for (int i = 0; i < activePieceScriptList.Count; i++)
            activePieceScriptList[i].enableCollider();
        isReadyToPlay = true;
        motionCanvas.SetActive(true);
    }

    public void comparePieces()
    {
        if (firtPiece.name.Substring(0, 7) == secondPiece.name.Substring(0, 7))
        {
            firtPiece.completePiece();
            secondPiece.completePiece();
            thumbsUp1.transform.position = new Vector3(firtPiece.transform.position.x - 0.3f, firtPiece.transform.position.y - 0.5f, 0);
            thumbsUp2.transform.position = new Vector3(secondPiece.transform.position.x - 0.3f, secondPiece.transform.position.y - 0.5f, 0);
            thumbsUp1.gameObject.SetActive(true);
            thumbsUp2.gameObject.SetActive(true);
            thumbsUp1.state.SetAnimation(0, "animation", false);
            thumbsUp2.state.SetAnimation(0, "animation", false);
            activePieceScriptList.Remove(firtPiece);
            activePieceScriptList.Remove(secondPiece);
            activePiecesList.Remove(firtPiece.transform);
            activePiecesList.Remove(secondPiece.transform);
            correctPairSound.Play();
            if (activePiecesList.Count==0)
            {
                isGameOver = true;
                StartCoroutine(finishMenuGetter());
            }
        }
        else
        {
            wrongPairSound.Play();
            StartCoroutine(warningPopUpGetter());
            firtPiece.resetPiece();
            secondPiece.resetPiece();
        }
        firtPiece = null;
        secondPiece = null;
    }

    Color tempWarningColor, tempColorBlack;
    bool warningPopUpActive;

    IEnumerator warningPopUpGetter()
    {
        tempWarningColor = Color.white;
        tempColorBlack = Color.black;
        warningPopUpImage.color = tempWarningColor;
        warningText.color = tempColorBlack;
        warningPopUp.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        warningPopUpActive = true;
    }

    IEnumerator finishMenuGetter()
    {
        yield return new WaitForSeconds(0.7f);
        winParticles.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        winSound.Play();
        finishMenu.SetActive(true);
    }

    public void returnToMainMenu()
    {
        buttonSound.Play();
        PlayerPrefs.SetInt("Babuş", 1);
        SceneManager.LoadSceneAsync("MainMenu");
    }

    public void restart()
    {
        buttonSound.Play();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
