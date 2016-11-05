using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PieceSlider : MonoBehaviour
{
    public List<Transform> slidingPiecesList = new List<Transform>();

    public Transform[] activePieces;

    public float rightScreenLimit, leftScreenLimit, pieceDistance, positionY, speed;

    public Transform currentPiece;

    Vector3[] activePiecesPositions;

    Vector3 tempPos;

    int enteringPieceIndex;

    bool isMoving;

    public Transform minPiece, maxPiece, enteringPiece;

	void Start ()
    {
	
	}
	
	void FixedUpdate ()
    {
        if (PieceGame.Instance.gameState == GameState.isActivePlay)
        {
            if (isMoving)
            {
                for (int i = 0; i < activePieces.Length; i++)
                {
                    if (activePieces[i] != null)
                    {
                        if (minPiece == null || activePiecesPositions[i].x < minPiece.localPosition.x)
                            minPiece = activePieces[i];
                        if (maxPiece == null || activePiecesPositions[i].x > maxPiece.localPosition.x)
                            maxPiece = activePieces[i];
                        if (enteringPiece == null || ((activePiecesPositions[i].x > rightScreenLimit + pieceDistance) && (activePiecesPositions[i].x < rightScreenLimit + (pieceDistance * 2.0f))))
                        {
                            enteringPiece = activePieces[i];
                            enteringPieceIndex = i;
                        }
                    }
                }

                for (int i = 0; i < activePieces.Length; i++)
                {
                    if (activePieces[i] != null)
                    {
                        activePiecesPositions[i].x -= Time.deltaTime * speed;
                        if (activePieces[i].name == currentPiece.name)
                        {
                            if (currentPiece.localPosition.x > rightScreenLimit + (pieceDistance * 2.0f))
                            {
                                tempPos = enteringPiece.localPosition;
                                activePiecesPositions[enteringPieceIndex].x = maxPiece.localPosition.x + pieceDistance;
                                activePiecesPositions[i] = tempPos;
                            }
                        }
                        if (activePiecesPositions[i].x < leftScreenLimit - pieceDistance)
                        {
                            if (activePieces[i].name == currentPiece.name)
                            {
                                if (enteringPiece == null || enteringPiece.localPosition.x < rightScreenLimit)
                                    activePiecesPositions[i].x = rightScreenLimit + pieceDistance;
                                else
                                {
                                    tempPos = enteringPiece.localPosition;
                                    activePiecesPositions[enteringPieceIndex].x = maxPiece.localPosition.x + pieceDistance;
                                    activePiecesPositions[i] = tempPos;
                                }
                            }
                            else
                            {
                                if (maxPiece.localPosition.x >= rightScreenLimit)
                                    activePiecesPositions[i].x = maxPiece.localPosition.x + pieceDistance;
                                else
                                    activePiecesPositions[i].x = rightScreenLimit + pieceDistance;
                            }
                            minPiece = null;
                        }
                        if (!activePieces[i].GetComponent<Piece>().isOnHold)
                            activePieces[i].localPosition = activePiecesPositions[i];
                    }
                }
            }
        }
	}

    public void shufflePieces()
    {
        int random;
        bool isEmpty;
        activePieces = new Transform[slidingPiecesList.Count];
        activePiecesPositions = new Vector3[slidingPiecesList.Count];
        for (int i = 0; i < slidingPiecesList.Count; i++)
        {
            isEmpty = true;
            while (isEmpty)
            {
                if (i < 4)
                    random = Random.Range(4, slidingPiecesList.Count - 4);
                else
                    random = Random.Range(0, slidingPiecesList.Count);
                if (activePieces[random] == null)
                {
                    activePieces[random] = slidingPiecesList[i];
                    activePieces[random].localPosition = new Vector3(leftScreenLimit + (pieceDistance * random), positionY, 0);
                    activePieces[random].GetComponent<Piece>().laneId = random;
                    activePiecesPositions[random] = activePieces[random].localPosition;
                    isEmpty = false;
                }
            }
        }
        isMoving = true;
    }

    public void removePiece(Transform removingPiece)
    {
        activePieces[removingPiece.GetComponent<Piece>().laneId] = null;
        activePiecesPositions[removingPiece.GetComponent<Piece>().laneId] = Vector3.zero;
    }
}
