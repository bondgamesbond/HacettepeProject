using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class PairGameManager : MonoBehaviour
{
    public static PairGameManager Instance;

    public Transform circlePiecesHead, squarePiecesHead;

    public List<Transform> activePiecesList;

    List<Renderer> activePiecesRenderersList;

    public float maxXPos, minXPos, maxYPos, minYPos;

    int circlePieceCount, squarePieceCount, tempPieceId;

    bool isOverLap, isGameOver;

    Ray screenRay;

    RaycastHit hit;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        activePiecesList = new List<Transform>();
        activePiecesRenderersList = new List<Renderer>();
        RestrictionMap.getDifficultyRatios();
        if(RestrictionMap.redRatio <= 15)
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
                    squarePieceCount++;
                }
            }
        }
        for (int i = 0; i < activePiecesList.Count; i++)
        {
            shufflePiece(activePiecesList[i], activePiecesRenderersList[i]);
        }
    }

    void Start ()
    {
	
	}
	
	void Update ()
    {
        if (!isGameOver)
        {
            if (Input.GetMouseButtonDown(0))
            {
                screenRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(screenRay, out hit))
                {
                    if (hit.transform.tag == "PairGamePiece")
                    {

                    }
                }
            }
        }
	}

    void shufflePiece(Transform piece, Renderer pieceRanderer)
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
            shufflePiece(piece, pieceRanderer);
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
