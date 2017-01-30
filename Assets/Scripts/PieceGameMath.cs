using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PieceGameMath : MonoBehaviour
{
    private static PieceGameMath myInstance = null;

    public string currentOperation, tempOperation;

    public int targetResult, value1, value2;

    public bool showingSymbols;

    bool isNumbersAdding, isNumbersMultiplying, divideTiming, isResulting, isResultWaiting;

    int touchCount, operationCount, tempValue;

    float divideTimer, resultTimer;

    public Text testText;

    public tk2dTextMesh resultingPieceText;

    Touch touch;

    Ray screenRay;

    RaycastHit hit;

    Transform greenGlow1, greenGlow2, whiteGlow1, whiteGlow2;

    GameObject moveParticle1, moveParticle2;

    public GameObject resultingPiece;

    Transform onHoldPiece1, onHoldPiece2;

    Vector3 offset1, offset2, startinHitPoint, endingPHitoint;

    public AudioSource addSound, multiplySound, divideSound, errorSound;

    public static PieceGameMath Instance
    {
        get
        {
            if (myInstance == null)
            {
                myInstance = FindObjectOfType(typeof(PieceGameMath)) as PieceGameMath;
            }
            if (myInstance == null)
            {
                GameObject obj = new GameObject("PieceGameMath");
                myInstance = obj.AddComponent(typeof(PieceGameMath)) as PieceGameMath;
            }
            return myInstance;
        }
    }

    void Update()
    {
        if (PieceGame.Instance.gameState == GameState.isActivePlay)
        {
            if (!isResulting)
            {
                touchCount = Input.touchCount;

                if (touchCount <= 2)
                {
                    for (int i = 0; i < touchCount; i++)
                    {
                        touch = Input.GetTouch(i);
                        if (touch.phase == TouchPhase.Began)  //On Touch Down
                        {
                            screenRay = Camera.main.ScreenPointToRay(touch.position);
                            if (Physics.Raycast(screenRay, out hit))
                            {
                                if (hit.transform.tag == "piece")
                                {
                                    if (touch.fingerId == 0)
                                    {
                                        onHoldPiece1 = hit.transform;
                                        greenGlow1 = onHoldPiece1.FindChild("greenGlow");
                                        whiteGlow1 = onHoldPiece1.FindChild("whiteGlow");
                                        moveParticle1 = onHoldPiece1.FindChild("moveParticle").gameObject;
                                        whiteGlow1.gameObject.SetActive(true);
                                        moveParticle1.SetActive(true);
                                        offset1 = hit.transform.position - Camera.main.ScreenToWorldPoint(new Vector2(touch.position.x, touch.position.y));
                                    }
                                    else if (touch.fingerId == 1)
                                    {
                                        onHoldPiece2 = hit.transform;
                                        greenGlow2 = onHoldPiece2.FindChild("greenGlow");
                                        whiteGlow2 = onHoldPiece2.FindChild("whiteGlow");
                                        moveParticle2 = onHoldPiece2.FindChild("moveParticle").gameObject;
                                        whiteGlow2.gameObject.SetActive(true);
                                        moveParticle2.SetActive(true);
                                        offset2 = hit.transform.position - Camera.main.ScreenToWorldPoint(new Vector2(touch.position.x, touch.position.y));
                                    }
                                }
                            }
                        }
                        if (touch.phase == TouchPhase.Moved)
                        {
                            if (!isNumbersMultiplying && !isNumbersAdding && currentOperation != "Divide")
                            {
                                if (onHoldPiece1 != null)
                                    onHoldPiece1.position = (Vector2)(Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position) + offset1);
                                if (onHoldPiece2 != null && Input.touchCount == 2)
                                    onHoldPiece2.position = (Vector2)(Camera.main.ScreenToWorldPoint(Input.GetTouch(1).position) + offset2);
                            }

                            if (onHoldPiece1 != null && onHoldPiece2 != null)
                            {
                                if (!onHoldPiece1.GetComponent<Piece>().isMultiplying)
                                    onHoldPiece1.GetComponent<Piece>().isMultiplying = true;
                                if (onHoldPiece1.GetComponent<Piece>().isCollided && isOnMultiplyPositions() && !isNumbersMultiplying)
                                {
                                    isNumbersMultiplying = true;
                                    tempOperation = "Multiply";
                                    if (tempOperation == currentOperation)
                                        multiplySound.Play();
                                    else
                                        errorSound.Play();
                                    isResulting = true;
                                    onHoldPiece1.GetComponent<Piece>().getWhiteAnim(false);
                                    onHoldPiece2.GetComponent<Piece>().getWhiteAnim(false);
                                    if (greenGlow1 != null && greenGlow1.gameObject.activeSelf)
                                        greenGlow1.gameObject.SetActive(false);
                                    if (greenGlow2 != null && greenGlow2.gameObject.activeSelf)
                                        greenGlow2.gameObject.SetActive(false);
                                    if (whiteGlow1 != null && whiteGlow1.gameObject.activeSelf)
                                        whiteGlow1.gameObject.SetActive(false);
                                    if (whiteGlow2 != null && whiteGlow2.gameObject.activeSelf)
                                        whiteGlow2.gameObject.SetActive(false);
                                    if (moveParticle1 != null && moveParticle1.activeSelf)
                                        moveParticle1.SetActive(false);
                                    if (moveParticle2 != null && moveParticle2.activeSelf)
                                        moveParticle2.SetActive(false);
                                }
                                if (isOnAddPositions())
                                {
                                    greenGlow1.gameObject.SetActive(true);
                                    greenGlow2.gameObject.SetActive(true);
                                }
                                else
                                {
                                    greenGlow1.gameObject.SetActive(false);
                                    greenGlow2.gameObject.SetActive(false);
                                }
                            }
                            if (onHoldPiece1 != null)
                            {
                                onHoldPiece1.position = (Vector2)(Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position) + offset1);
                                if (touchCount >= 2 && touch.fingerId == 1)
                                {
                                    screenRay = Camera.main.ScreenPointToRay(touch.position);
                                    if (Physics.Raycast(screenRay, out hit))
                                    {
                                        if (hit.transform.tag == "piece")
                                        {
                                            if (startinHitPoint == Vector3.zero)
                                            {
                                                startinHitPoint = hit.point;
                                                divideTiming = true;
                                            }
                                            endingPHitoint = hit.point;
                                        }
                                    }
                                    else
                                    {
                                        if (startinHitPoint != Vector3.zero)
                                        {
                                            if (Vector3.Distance(startinHitPoint, endingPHitoint) > 0.8f && divideTimer < 0.8f)
                                            {
                                                onHoldPiece1.GetComponent<Piece>().getWhiteAnim(true);
                                                isResulting = true;
                                                tempOperation = "Divide";
                                                if (tempOperation == currentOperation)
                                                    divideSound.Play();
                                                else
                                                    errorSound.Play();
                                                if (whiteGlow1 != null && whiteGlow1.gameObject.activeSelf)
                                                    whiteGlow1.gameObject.SetActive(false);
                                                if (moveParticle1 != null && moveParticle1.activeSelf)
                                                    moveParticle1.SetActive(false);
                                                tempValue = int.Parse(onHoldPiece1.FindChild("pieceText").GetComponent<tk2dTextMesh>().text);
                                                if (currentOperation != tempOperation)
                                                {
                                                    if (onHoldPiece1 != null)
                                                        onHoldPiece1.GetComponent<Piece>().refreshPiece();
                                                    if (onHoldPiece2 != null)
                                                        onHoldPiece2.GetComponent<Piece>().refreshPiece();
                                                    if (greenGlow1 != null && greenGlow1.gameObject.activeSelf)
                                                        greenGlow1.gameObject.SetActive(false);
                                                    if (greenGlow2 != null && greenGlow2.gameObject.activeSelf)
                                                        greenGlow2.gameObject.SetActive(false);
                                                    if (whiteGlow1 != null && whiteGlow1.gameObject.activeSelf)
                                                        whiteGlow1.gameObject.SetActive(false);
                                                    if (whiteGlow2 != null && whiteGlow2.gameObject.activeSelf)
                                                        whiteGlow2.gameObject.SetActive(false);
                                                    if (moveParticle1 != null && moveParticle1.activeSelf)
                                                        moveParticle1.SetActive(false);
                                                    if (moveParticle2 != null && moveParticle2.activeSelf)
                                                        moveParticle2.SetActive(false);
                                                    onHoldPiece1 = null;
                                                    onHoldPiece2 = null;
                                                    isResulting = false;
                                                }
                                            }
                                            divideTiming = false;
                                            divideTimer = 0;
                                            startinHitPoint = Vector3.zero;
                                            endingPHitoint = Vector3.zero;
                                        }
                                    }
                                }
                            }
                        }
                        if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                        {
                            if (onHoldPiece1 != null && onHoldPiece2 != null)
                            {
                                if (!checkAddOperation() && !isNumbersMultiplying)
                                {
                                    PieceGame.Instance.pieceReturnSound.Play();
                                    onHoldPiece1.GetComponent<Piece>().returnPiece();
                                    onHoldPiece1 = null;
                                    onHoldPiece2.GetComponent<Piece>().returnPiece();
                                    onHoldPiece2 = null;
                                    if (greenGlow1 != null && greenGlow1.gameObject.activeSelf)
                                        greenGlow1.gameObject.SetActive(false);
                                    if (greenGlow2 != null && greenGlow2.gameObject.activeSelf)
                                        greenGlow2.gameObject.SetActive(false);
                                    if (whiteGlow1 != null && whiteGlow1.gameObject.activeSelf)
                                        whiteGlow1.gameObject.SetActive(false);
                                    if (whiteGlow2 != null && whiteGlow2.gameObject.activeSelf)
                                        whiteGlow2.gameObject.SetActive(false);
                                    if (moveParticle1 != null && moveParticle1.activeSelf)
                                        moveParticle1.SetActive(false);
                                    if (moveParticle2 != null && moveParticle2.activeSelf)
                                        moveParticle2.SetActive(false);
                                }
                            }
                            else
                            {
                                if (onHoldPiece1 != null)
                                {
                                    if (touch.fingerId == 0)
                                    {
                                        PieceGame.Instance.pieceReturnSound.Play();
                                        onHoldPiece1.GetComponent<Piece>().returnPiece();
                                        onHoldPiece1 = null;
                                        if (greenGlow1 != null && greenGlow1.gameObject.activeSelf)
                                            greenGlow1.gameObject.SetActive(false);
                                        if (whiteGlow1 != null && whiteGlow1.gameObject.activeSelf)
                                            whiteGlow1.gameObject.SetActive(false);
                                        if (moveParticle1 != null && moveParticle1.activeSelf)
                                            moveParticle1.SetActive(false);

                                    }
                                }
                                if (onHoldPiece2 != null)
                                {
                                    PieceGame.Instance.pieceReturnSound.Play();
                                    onHoldPiece2.GetComponent<Piece>().returnPiece();
                                    onHoldPiece2 = null;
                                    if (greenGlow2 != null && greenGlow2.gameObject.activeSelf)
                                        greenGlow2.gameObject.SetActive(false);
                                    if (whiteGlow2 != null && whiteGlow2.gameObject.activeSelf)
                                        whiteGlow2.gameObject.SetActive(false);
                                    if (moveParticle2 != null && moveParticle2.activeSelf)
                                        moveParticle2.SetActive(false);
                                }
                            }
                            startinHitPoint = Vector3.zero;
                            endingPHitoint = Vector3.zero;
                            divideTimer = 0;
                        }
                    }
                }
            }

            if (divideTiming)
                divideTimer += Time.deltaTime;

            if (isNumbersAdding)
            {
                onHoldPiece1.position = Vector2.MoveTowards(onHoldPiece1.position, onHoldPiece2.position, 0.05f);
                onHoldPiece2.position = Vector2.MoveTowards(onHoldPiece2.position, onHoldPiece1.position, 0.05f);
                if (currentOperation == tempOperation)
                {
                    if (onHoldPiece1.position == onHoldPiece2.position)
                    {
                        isNumbersAdding = false;
                        onHoldPiece1.GetComponent<Piece>().endWhiteAnim();
                        if (currentOperation == tempOperation)
                            onHoldPiece2.GetComponent<Piece>().resetPiece();
                        else
                            onHoldPiece2.gameObject.SetActive(false);
                    }
                }
                else
                {
                    isNumbersAdding = false;
                    if (onHoldPiece1 != null)
                        onHoldPiece1.GetComponent<Piece>().refreshPiece();
                    if (onHoldPiece2 != null)
                        onHoldPiece2.GetComponent<Piece>().refreshPiece();
                    if (greenGlow1 != null && greenGlow1.gameObject.activeSelf)
                        greenGlow1.gameObject.SetActive(false);
                    if (greenGlow2 != null && greenGlow2.gameObject.activeSelf)
                        greenGlow2.gameObject.SetActive(false);
                    if (whiteGlow1 != null && whiteGlow1.gameObject.activeSelf)
                        whiteGlow1.gameObject.SetActive(false);
                    if (whiteGlow2 != null && whiteGlow2.gameObject.activeSelf)
                        whiteGlow2.gameObject.SetActive(false);
                    if (moveParticle1 != null && moveParticle1.activeSelf)
                        moveParticle1.SetActive(false);
                    if (moveParticle2 != null && moveParticle2.activeSelf)
                        moveParticle2.SetActive(false);
                    onHoldPiece1 = null;
                    onHoldPiece2 = null;
                    isResulting = false;
                }
            }

            if (isNumbersMultiplying)
            {
                onHoldPiece1.position = Vector2.MoveTowards(onHoldPiece1.position, onHoldPiece2.position, 0.05f);
                onHoldPiece2.position = Vector2.MoveTowards(onHoldPiece2.position, onHoldPiece1.position, 0.05f);
                if (currentOperation == tempOperation)
                {
                    if (onHoldPiece1.position == onHoldPiece2.position)
                    {
                        isNumbersMultiplying = false;
                        onHoldPiece1.GetComponent<Piece>().endWhiteAnim();
                        if (currentOperation == tempOperation)
                            onHoldPiece2.GetComponent<Piece>().resetPiece();
                        else
                            onHoldPiece2.gameObject.SetActive(false);
                    }
                }
                else
                {
                    isNumbersMultiplying = false;
                    if (onHoldPiece1 != null)
                        onHoldPiece1.GetComponent<Piece>().refreshPiece();
                    if (onHoldPiece2 != null)
                        onHoldPiece2.GetComponent<Piece>().refreshPiece();
                    if (greenGlow1 != null && greenGlow1.gameObject.activeSelf)
                        greenGlow1.gameObject.SetActive(false);
                    if (greenGlow2 != null && greenGlow2.gameObject.activeSelf)
                        greenGlow2.gameObject.SetActive(false);
                    if (whiteGlow1 != null && whiteGlow1.gameObject.activeSelf)
                        whiteGlow1.gameObject.SetActive(false);
                    if (whiteGlow2 != null && whiteGlow2.gameObject.activeSelf)
                        whiteGlow2.gameObject.SetActive(false);
                    if (moveParticle1 != null && moveParticle1.activeSelf)
                        moveParticle1.SetActive(false);
                    if (moveParticle2 != null && moveParticle2.activeSelf)
                        moveParticle2.SetActive(false);
                    onHoldPiece1 = null;
                    onHoldPiece2 = null;
                    isResulting = false;
                }
            }

            if(isResultWaiting)
            {
                resultTimer += Time.deltaTime;
                if(resultTimer>=1.0f)
                {
                    resultTimer = 0;
                    isResultWaiting = false;
                    resultingPiece.SetActive(false);
                    if (currentOperation == tempOperation)
                    {
                        if (greenGlow1 != null && greenGlow1.gameObject.activeSelf)
                            greenGlow1.gameObject.SetActive(false);
                        if (greenGlow2 != null && greenGlow2.gameObject.activeSelf)
                            greenGlow2.gameObject.SetActive(false);
                        if (whiteGlow1 != null && whiteGlow1.gameObject.activeSelf)
                            whiteGlow1.gameObject.SetActive(false);
                        if (whiteGlow2 != null && whiteGlow2.gameObject.activeSelf)
                            whiteGlow2.gameObject.SetActive(false);
                        if (moveParticle1 != null && moveParticle1.activeSelf)
                            moveParticle1.SetActive(false);
                        if (moveParticle2 != null && moveParticle2.activeSelf)
                            moveParticle2.SetActive(false);
                        onHoldPiece1 = null;
                        onHoldPiece2 = null;
                        getNextOperation();
                    }
                    else
                    {
                        if (onHoldPiece1 != null)
                            onHoldPiece1.GetComponent<Piece>().refreshPiece();
                        if (onHoldPiece2 != null)
                            onHoldPiece2.GetComponent<Piece>().refreshPiece();
                        if (greenGlow1 != null && greenGlow1.gameObject.activeSelf)
                            greenGlow1.gameObject.SetActive(false);
                        if (greenGlow2 != null && greenGlow2.gameObject.activeSelf)
                            greenGlow2.gameObject.SetActive(false);
                        if (whiteGlow1 != null && whiteGlow1.gameObject.activeSelf)
                            whiteGlow1.gameObject.SetActive(false);
                        if (whiteGlow2 != null && whiteGlow2.gameObject.activeSelf)
                            whiteGlow2.gameObject.SetActive(false);
                        if (moveParticle1 != null && moveParticle1.activeSelf)
                            moveParticle1.SetActive(false);
                        if (moveParticle2 != null && moveParticle2.activeSelf)
                            moveParticle2.SetActive(false);
                        onHoldPiece1 = null;
                        onHoldPiece2 = null;
                        isResulting = false;
                    }
                }
            }
        }
    }

    public void getResultingPiece()
    {
        resultingPiece.transform.position = onHoldPiece1.position;
        if (currentOperation == tempOperation)
            resultingPieceText.text = targetResult.ToString();
        else
        {
            if (tempOperation == "Add")
                resultingPieceText.text = (value1 + value2).ToString();
            else if (tempOperation == "Multiply")
                resultingPieceText.text = (value1 * value2).ToString();
            else if (tempOperation == "Divide")
                resultingPieceText.text = (value1 / 2).ToString();
        }
        resultingPiece.SetActive(true);
        isResultWaiting = true;
    }
    
    bool checkAddOperation()
    {
        if (Mathf.Abs(onHoldPiece1.position.x - onHoldPiece2.position.x) <= 0.3f && Mathf.Abs(onHoldPiece1.position.y - onHoldPiece2.position.y) >= 0.9f && Mathf.Abs(onHoldPiece1.position.y - onHoldPiece2.position.y) <= 1.5f)
        {
            onHoldPiece2.transform.position = new Vector3(onHoldPiece1.transform.position.x, onHoldPiece2.transform.position.y, 0);
            isNumbersAdding = true;
            isResulting = true;
            onHoldPiece1.GetComponent<Piece>().getWhiteAnim(false);
            onHoldPiece2.GetComponent<Piece>().getWhiteAnim(false);
            tempOperation = "Add";
            if (tempOperation == currentOperation)
                addSound.Play();
            else
                errorSound.Play();
            if (greenGlow1 != null && greenGlow1.gameObject.activeSelf)
                greenGlow1.gameObject.SetActive(false);
            if (greenGlow2 != null && greenGlow2.gameObject.activeSelf)
                greenGlow2.gameObject.SetActive(false);
            if (whiteGlow1 != null && whiteGlow1.gameObject.activeSelf)
                whiteGlow1.gameObject.SetActive(false);
            if (whiteGlow2 != null && whiteGlow2.gameObject.activeSelf)
                whiteGlow2.gameObject.SetActive(false);
            if (moveParticle1 != null && moveParticle1.activeSelf)
                moveParticle1.SetActive(false);
            if (moveParticle2 != null && moveParticle2.activeSelf)
                moveParticle2.SetActive(false);
            return true;
        }
        else
            return false;
    }

    bool isOnAddPositions()
    {
        if (Mathf.Abs(onHoldPiece1.position.x - onHoldPiece2.position.x) <= 0.3f && Mathf.Abs(onHoldPiece1.position.y - onHoldPiece2.position.y) >= 0.9f && Mathf.Abs(onHoldPiece1.position.y - onHoldPiece2.position.y) <= 1.5f)
            return true;
        else
            return false;
    }
    bool isOnMultiplyPositions()
    {
        if (Mathf.Abs(onHoldPiece1.position.y - onHoldPiece2.position.y) <= 0.25f)
            return true;
        else
            return false;
    }

    void getNextOperation()
    {
        operationCount++;
        PieceGame.Instance.levelCompleteScore += 5;
        if (operationCount < 20)
        {
            isResulting = false;
            if (showingSymbols)
            {
                if (operationCount == 1)
                {
                    PieceGame.Instance.getTutorialScene(8, false);
                }
                else if (operationCount == 2)
                {
                    PieceGame.Instance.getTutorialScene(9, false);
                }
                else
                    PieceGame.Instance.getMathOperation("", PieceGame.Instance.numbersList.GetChild(Random.Range(0, 10)), PieceGame.Instance.numbersList.GetChild(Random.Range(0, 10)), true);
            }
            else
                PieceGame.Instance.getMathOperation("", PieceGame.Instance.numbersList.GetChild(Random.Range(0, 10)), PieceGame.Instance.numbersList.GetChild(Random.Range(0, 10)), false);
        }
        else
            PieceGame.Instance.finishGame(false, false, false);
    }
}
