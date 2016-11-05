﻿using UnityEngine;
using System.Collections;

public class Piece : MonoBehaviour
{
    public Vector3 targetPosition, initialPosition;

    Vector3 firstPosition;

    Quaternion shadowRotation;

    Renderer thisRenderer;

    Collider thisCollider;

    public Transform shadow;

    GameObject numberText;

    SkeletonAnimation whiteAnimation;

    Shadow shadowScript;

    float positionSensivity = 0.3f, rotationSensivity, maxRandomX = 7.5f, minRandomX = -7.5f, maxRandomY = 4.0f, minRandomY = -1.0f, tempScale;

    bool isOverlap, isStraightLineup, isWhiteAnimAuto, resultFlag = false;

    public int laneId;

    public bool isPlaced, isOnHold, isOnTrueScale, isOnTrueRotation, isMultiplying, isCollided, isWhiteAnimFinished;

    void Awake ()
    {
        thisCollider = GetComponent<Collider>();
        shadow = transform.FindChild("shadow");
        shadowScript = shadow.GetComponent<Shadow>();
        if (transform.FindChild("whiteAnim") != null)
            whiteAnimation = transform.FindChild("whiteAnim").GetComponent<SkeletonAnimation>();
        numberText = transform.FindChild("pieceText").gameObject;
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
            gameObject.SetActive(true);
            firstPosition = transform.position;
            shadowScript.position = targetPosition;
            shadowScript.rotation = shadowRotation;
            Vector2 pos = Camera.main.WorldToScreenPoint(transform.position);
            pos.x = ((pos.x - 50) / 550) * 1600;
            pos.y = ((pos.y - 150) / 190) * 900;
            if (pos.x > 1600)
                pos.x = 1599;
            if (pos.y > 900)
                pos.y = 899;
            //.Log(transform.name + ",x:" + pos.x  + ",y:" + ((int)(900 - pos.y)) + ",Value:" + PlayerPrefsX.GetIntArray("CurrentDifficultyArea" + ((int)(900 - pos.y) / 4))[(int)(pos.x / 4)]);
        }
    }


    public bool checkTargetPosition() //return true if piece was put on right place
    {
        if (Mathf.Abs(transform.position.x - targetPosition.x) <= positionSensivity && Mathf.Abs(transform.position.y - targetPosition.y) <= positionSensivity && isOnTrueScale && isOnTrueRotation)
        {
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
        transform.position = firstPosition;
        isOnHold = false;
    }

    public void resetPiece()
    {
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
