using UnityEngine;
using System.Collections;

public class Piece : MonoBehaviour
{
    public Vector3 targetPosition, initialPosition;

    Vector3 firstPosition;

    Quaternion shadowRotation;

    Renderer thisRenderer;

    Collider thisCollider;

    public Transform shadow;

    GameObject numberText, moveParticle;

    SkeletonAnimation whiteAnimation;

    Shadow shadowScript;

    float positionSensivity = 0.3f, rotationSensivity, maxRandomX = 7.5f, minRandomX = -7.5f, maxRandomY = 4.0f, minRandomY = -1.0f, tempScale;

    bool isOverlap, isStraightLineup, isWhiteAnimAuto, resultFlag = false;

    public int laneId, difficulty;

    int tempDifficulty;

    public bool isPlaced, isOnHold, isOnTrueScale, isOnTrueRotation, isMultiplying, isCollided, isWhiteAnimFinished;

    void Awake ()
    {
        thisCollider = GetComponent<Collider>();
        shadow = transform.FindChild("shadow");
        shadowScript = shadow.GetComponent<Shadow>();
        if (transform.FindChild("whiteAnim") != null)
            whiteAnimation = transform.FindChild("whiteAnim").GetComponent<SkeletonAnimation>();
        numberText = transform.FindChild("pieceText").gameObject;
        moveParticle = transform.FindChild("moveParticle").gameObject;
        moveParticle.GetComponent<Renderer>().sortingLayerName = GetComponent<Renderer>().sortingLayerName;
        moveParticle.GetComponent<Renderer>().sortingOrder = GetComponent<Renderer>().sortingOrder - 2;
    }
	
	void Update ()
    {
        if (whiteAnimation != null)
        {
            if (whiteAnimation.gameObject.activeSelf)
            {
                if (whiteAnimation.AnimationName == "turnToWhite")
                {
                    if (whiteAnimation.state.GetCurrent(0).time >= whiteAnimation.state.GetCurrent(0).endTime)
                    {
                        if (isWhiteAnimAuto)
                        {
                            whiteAnimation.AnimationName = "turnToNormal";
                            numberText.SetActive(false);
                        }
                        else
                        {
                            whiteAnimation.loop = true;
                            whiteAnimation.AnimationName = "loop";
                        }
                    }
                }
                if (whiteAnimation.AnimationName == "turnToNormal")
                {
                    if (whiteAnimation.state.GetCurrent(0).time >= whiteAnimation.state.GetCurrent(0).endTime * 0.2f && !resultFlag)
                    {
                        resultFlag = true;
                        PieceGameMath.Instance.getResultingPiece();
                    }
                    if (whiteAnimation.state.GetCurrent(0).time >= whiteAnimation.state.GetCurrent(0).endTime)
                    {
                        if (PieceGameMath.Instance.currentOperation == PieceGameMath.Instance.tempOperation)
                            resetPiece();
                    }
                }
            }
        }
	}

    public void shufflePosition(bool isStraightLine, bool isRotationChanging, bool isScaleChanging, bool isMathPiece, bool isAlone)
    {
        isStraightLineup = isStraightLine;
        if (targetPosition == Vector3.zero)
        {
            targetPosition = transform.position;
            shadowRotation = transform.rotation;
            initialPosition = targetPosition;
        }
        isOverlap = false;
        if (isScaleChanging)
        {
            if (Random.Range(0, 5) < 0.5f)
                tempScale = Random.Range(0.6f, 0.9f);
            else
                tempScale = Random.Range(1.1f, 1.4f);
            transform.localScale = new Vector3(tempScale, tempScale, 1);
        }
        else
            isOnTrueScale = true;
        if (isRotationChanging)
        {
            transform.localEulerAngles = new Vector3(0, 0, Random.Range(100, 260));
        }
        else
            isOnTrueRotation = true;
        if (!isStraightLine)
        {
            if (!isMathPiece)
                transform.localPosition = new Vector3(Random.Range(minRandomX, maxRandomX), Random.Range(minRandomY, maxRandomY), 0);
            else
                transform.localPosition = new Vector3(Random.Range(-6.0f, 6.0f), Random.Range(-2.5f, 2.5f), 0);
        }
        else
        {
            //StraightLine random position
            PieceGame.Instance.pieceSlider.slidingPiecesList.Add(transform);
        }
        if (thisRenderer == null)
            thisRenderer = transform.GetComponent<Renderer>();
        if (!isAlone)
        {
            foreach (Renderer child in PieceGame.Instance.pieceList.GetComponentsInChildren<Renderer>())
            {
                if (child.gameObject.activeSelf && (child.name != transform.name) && child.name != "pieceText")
                {
                    if (child.bounds.Intersects(thisRenderer.bounds))
                        isOverlap = true;
                }
            }
        }
        if (isOverlap)
            shufflePosition(isStraightLineup, isRotationChanging, isScaleChanging, isMathPiece, isAlone);
        else
        {
            if (!isStraightLineup && !PieceGame.Instance.isTooMuchRedArea && RestrictionMap.findDifficulty(transform.position) >= 4)
                shufflePosition(isStraightLineup, isRotationChanging, isScaleChanging, isMathPiece, isAlone);
            else
            {
                gameObject.SetActive(true);
                firstPosition = transform.position;
                shadowScript.position = targetPosition;
                shadowScript.rotation = shadowRotation;
            }
        }
        //if (isOverlap)
        //    shufflePosition(isStraightLineup, isRotationChanging, isScaleChanging, isMathPiece, isAlone);
        //else
        //{
        //    if (!isStraightLineup)
        //    {
        //        tempDifficulty = RestrictionMap.findDifficulty(transform.position);
        //        if (tempDifficulty >= 4)
        //        {
        //            if (PieceGame.Instance.redPieceCount < PieceGame.Instance.redPieceSlot)
        //            {
        //                PieceGame.Instance.redPieceCount++;
        //                gameObject.SetActive(true);
        //                firstPosition = transform.position;
        //                shadowScript.position = targetPosition;
        //                shadowScript.rotation = shadowRotation;
        //                difficulty = 4;
        //            }
        //            else
        //                shufflePosition(isStraightLineup, isRotationChanging, isScaleChanging, isMathPiece, isAlone);
        //        }
        //        else if (tempDifficulty == 3)
        //        {
        //            if (PieceGame.Instance.bluePieceCount < PieceGame.Instance.bluePieceSlot)
        //            {
        //                PieceGame.Instance.bluePieceCount++;
        //                gameObject.SetActive(true);
        //                firstPosition = transform.position;
        //                shadowScript.position = targetPosition;
        //                shadowScript.rotation = shadowRotation;
        //                difficulty = 3;
        //            }
        //            else
        //                shufflePosition(isStraightLineup, isRotationChanging, isScaleChanging, isMathPiece, isAlone);
        //        }
        //        else if (tempDifficulty == 2)
        //        {
        //            if (PieceGame.Instance.cyanPieceCount < PieceGame.Instance.cyanPieceSlot)
        //            {
        //                PieceGame.Instance.cyanPieceCount++;
        //                gameObject.SetActive(true);
        //                firstPosition = transform.position;
        //                shadowScript.position = targetPosition;
        //                shadowScript.rotation = shadowRotation;
        //                difficulty = 2;
        //            }
        //            else
        //                shufflePosition(isStraightLineup, isRotationChanging, isScaleChanging, isMathPiece, isAlone);
        //        }
        //        else if (tempDifficulty == 1)
        //        {
        //            if (PieceGame.Instance.greenPieceCount < PieceGame.Instance.greenPieceSlot)
        //            {
        //                PieceGame.Instance.greenPieceCount++;
        //                gameObject.SetActive(true);
        //                firstPosition = transform.position;
        //                shadowScript.position = targetPosition;
        //                shadowScript.rotation = shadowRotation;
        //                difficulty = 1;
        //            }
        //            else
        //                shufflePosition(isStraightLineup, isRotationChanging, isScaleChanging, isMathPiece, isAlone);
        //        }
        //        else if (tempDifficulty == 0)
        //        {
        //            if (PieceGame.Instance.yellowPieceCount < PieceGame.Instance.yellowPieceSlot)
        //            {
        //                PieceGame.Instance.yellowPieceCount++;
        //                gameObject.SetActive(true);
        //                firstPosition = transform.position;
        //                shadowScript.position = targetPosition;
        //                shadowScript.rotation = shadowRotation;
        //                difficulty = 0;
        //            }
        //            else
        //                shufflePosition(isStraightLineup, isRotationChanging, isScaleChanging, isMathPiece, isAlone);
        //        }
        //    }
        //    else
        //    {
        //        gameObject.SetActive(true);
        //        firstPosition = transform.position;
        //        shadowScript.position = targetPosition;
        //        shadowScript.rotation = shadowRotation;
        //    }
        //}
    }


    public bool checkTargetPosition() //return true if piece was put on right place
    {
        if (Mathf.Abs(transform.position.x - targetPosition.x) <= positionSensivity && Mathf.Abs(transform.position.y - targetPosition.y) <= positionSensivity && isOnTrueScale && isOnTrueRotation)
        {
            if (moveParticle.activeSelf)
                moveParticle.SetActive(false);
            isPlaced = true;
            thisCollider.enabled = false;
            transform.position = targetPosition;
            shadow.gameObject.SetActive(false);
            isOnHold = false;
            if (!PieceGame.Instance.hasReversePlay)
                PieceGame.Instance.levelCompleteScore += 5;
            else
                PieceGame.Instance.levelCompleteScore += 2.5f;
            return true;
        }
        else
        {
            if (!isStraightLineup)
                transform.position = firstPosition;
            isOnHold = false;
            return false;
        }
    }

    public bool isOnTargetPosition()
    {
        if (Mathf.Abs(transform.position.x - targetPosition.x) <= positionSensivity && Mathf.Abs(transform.position.y - targetPosition.y) <= positionSensivity && isOnTrueScale && isOnTrueRotation)
            return true;
        else
            return false;
    }

    public void returnPiece()
    {
        if (moveParticle.activeSelf)
            moveParticle.SetActive(false);
        transform.position = firstPosition;
        isOnHold = false;
    }

    public void resetPiece()
    {
        if (moveParticle.activeSelf)
            moveParticle.SetActive(false);
        thisRenderer.enabled = true;
        isMultiplying = false;
        isCollided = false;
        isPlaced = false;
        transform.position = initialPosition;
        targetPosition = Vector3.zero;
        firstPosition = Vector3.zero;
        gameObject.SetActive(false);
        thisCollider.enabled = true;
        isOverlap = false;
        isOnHold = false;
        if (whiteAnimation != null)
        {
            whiteAnimation.AnimationName = "";
            whiteAnimation.gameObject.SetActive(false);
        }
        if (transform.FindChild("underline") != null)
            transform.FindChild("underline").gameObject.SetActive(true);
        numberText.SetActive(true);
        resultFlag = false;
    }

    public void refreshPiece()
    {
        if (moveParticle.activeSelf)
            moveParticle.SetActive(false);
        gameObject.SetActive(true);
        thisRenderer.enabled = true;
        isMultiplying = false;
        isCollided = false;
        isPlaced = false;
        thisCollider.enabled = true;
        isOverlap = false;
        if (whiteAnimation != null)
        {
            whiteAnimation.AnimationName = "";
            whiteAnimation.gameObject.SetActive(false);
        }
        numberText.SetActive(true);
        resultFlag = false;
        if (transform.FindChild("underline") != null)
            transform.FindChild("underline").gameObject.SetActive(true);
        returnPiece();
    }

    public void reverse()
    {
        if (moveParticle.activeSelf)
            moveParticle.SetActive(false);
        isPlaced = false;
        isOnHold = false;
        thisCollider.enabled = true;
        shadowScript.position = firstPosition;
        Vector3 tempPosition = firstPosition;
        firstPosition = targetPosition;
        targetPosition = tempPosition;
    }

    void OnTriggerEnter(Collider other)
    {
        if (isMultiplying)
        {
            if (other.transform.tag == "piece")
                isCollided = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (isMultiplying)
        {
            if (other.transform.tag == "piece")
                isCollided = false;
        }
    }

    public void getWhiteAnim(bool isAuto)
    {
        if (moveParticle.activeSelf)
            moveParticle.SetActive(false);
        whiteAnimation.gameObject.SetActive(true);
        whiteAnimation.loop = false;
        whiteAnimation.AnimationName = "turnToWhite";
        isWhiteAnimAuto = isAuto;
        thisRenderer.enabled = false;
        if (transform.FindChild("underline") != null)
            transform.FindChild("underline").gameObject.SetActive(false);
    }

    public void endWhiteAnim()
    {
        whiteAnimation.loop = false;
        whiteAnimation.AnimationName = "turnToNormal";
        numberText.SetActive(false);
    }
}
