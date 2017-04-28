using UnityEngine;
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

    public GameObject snowParticle, tutorial, popUp, finishMenu, winParticles;

    public SkeletonAnimation thumbsUp1, thumbsUp2;

    public SpriteRenderer snowBackgroundSprite;

    public Text popUpText;

    public List<Transform> activePiecesList;

    List<Renderer> activePiecesRenderersList;

    List<PairGamePiece> activePieceScriptList;

    public float maxXPos, minXPos, maxYPos, minYPos;

    int circlePieceCount, squarePieceCount, tempPieceId;

    bool isOverLap, isGameOver, isReadyToPlay, isPaused;

    RaycastHit2D hit;

    Vector2 worldPoint;

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
    }

    void Start ()
    {
        if (RestrictionMap.redRatio <= 15)
        {
            while (circlePieceCount < 4)
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
            while (squarePieceCount < 4)
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
                            }
                        }
                    }
                }
                else if (Input.GetMouseButton(0))
                {

                }
                else if (Input.GetMouseButtonUp(0))
                {
                    worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    hit = Physics2D.Raycast(worldPoint, Vector2.zero);
                    if (hit.collider != null)
                    {
                        if (hit.transform.tag == "PairGamePiece")
                        {
                            if (onClearPiece != null && onClearPiece.name == hit.transform.name)
                            {
                                activePieceScriptList[activePiecesList.IndexOf(hit.transform)].clearPiece();
                            }
                            onClearPiece = null;
                        }
                    }
                }
            }
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
    }

    public void resumeGame()
    {
        isPaused = false;
        popUp.SetActive(false);
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
            if(activePiecesList.Count==0)
            {
                isGameOver = true;
                StartCoroutine(finishMenuGetter());
            }
        }
        else
        {
            firtPiece.resetPiece();
            secondPiece.resetPiece();
        }
        firtPiece = null;
        secondPiece = null;
    }

    IEnumerator finishMenuGetter()
    {
        yield return new WaitForSeconds(0.7f);
        winParticles.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        finishMenu.SetActive(true);
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
