using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class PieceRescaler : MonoBehaviour
{
    int touchCount, onPieceTouchCount;

    float scaleSensivity=0.175f;

    Transform onHolPiece, redGlow, greenGlow, whiteGlow;

    public Text testText;

    Touch touch;

    Ray screenRay;

    RaycastHit hit;

    private float scale_factor = 0.06f;
    private float MAXSCALE = 2.0f, MIN_SCALE = 0.6f; // zoom-in and zoom-out limits
    private Vector2 prevDist = new Vector2(0, 0);
    private Vector2 curDist = new Vector2(0, 0);
    private Vector2 midPoint = new Vector2(0, 0);
    private Vector3 originalPos;


    void Start ()
    {
	
	}

    void Update()
    {
        if (PieceGame.Instance.gameState == GameState.isActivePlay)
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
                            if (hit.transform.tag == "piece" && (onHolPiece == null || onHolPiece.name == hit.transform.name))
                            {
                                if (!hit.transform.GetComponent<Piece>().isOnTrueScale && onPieceTouchCount < 2)
                                {
                                    onPieceTouchCount++;
                                    if (onPieceTouchCount == 2)
                                    {
                                        onHolPiece = hit.transform;
                                        redGlow = onHolPiece.FindChild("redGlow");
                                        greenGlow = onHolPiece.FindChild("greenGlow");
                                        whiteGlow = onHolPiece.FindChild("whiteGlow");
                                        redGlow.gameObject.SetActive(true);
                                    }
                                }
                            }
                        }
                    }
                    if (touch.phase == TouchPhase.Moved)
                    {
                        if (onHolPiece != null)
                            checkForScale();
                    }
                    if(touch.phase== TouchPhase.Ended || touch.phase==TouchPhase.Canceled)
                    {
                        if (onHolPiece != null)
                        {
                            if (onHolPiece.localScale.x > 1 - scaleSensivity && onHolPiece.localScale.x < 1 + scaleSensivity)
                            {
                                onHolPiece.localScale = new Vector3(1, 1, 1);
                                onHolPiece.GetComponent<Piece>().isOnTrueScale = true;
                                PieceGame.Instance.piecePlaceSound.Play();
                            }
                            onHolPiece = null;
                            if (redGlow != null)
                            {
                                redGlow.gameObject.SetActive(false);
                                whiteGlow.gameObject.SetActive(false);
                                redGlow = null;
                                greenGlow = null;
                            }
                        }
                        onPieceTouchCount = 0;
                    }
                }
            }
        }
    }
    
    private void checkForScale()
    {
        // These lines of code will take the distance between two touches and zoom in - zoom out at middle point between them
        if (Input.touchCount == 2 && (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved))
        {
            midPoint = new Vector2(((Input.GetTouch(0).position.x + Input.GetTouch(1).position.x) / 2), ((Input.GetTouch(0).position.y + Input.GetTouch(1).position.y) / 2));
            midPoint = Camera.main.ScreenToWorldPoint(midPoint);

            curDist = Input.GetTouch(0).position - Input.GetTouch(1).position; //current distance between finger touches
            prevDist = ((Input.GetTouch(0).position - Input.GetTouch(0).deltaPosition) - (Input.GetTouch(1).position - Input.GetTouch(1).deltaPosition)); //difference in previous locations using delta positions
            float touchDelta = curDist.magnitude - prevDist.magnitude;
            // Zoom out
            if (touchDelta > 0)
            {
                if (onHolPiece.transform.localScale.x < MAXSCALE && onHolPiece.transform.localScale.y < MAXSCALE)
                {
                    Vector3 scale = new Vector3(onHolPiece.transform.localScale.x + scale_factor, onHolPiece.transform.localScale.y + scale_factor, 1);
                    scale.x = (scale.x > MAXSCALE) ? MAXSCALE : scale.x;
                    scale.y = (scale.y > MAXSCALE) ? MAXSCALE : scale.y;
                    scaleFromPosition(scale, midPoint);
                }
            }
            //Zoom in
            else if (touchDelta < 0)
            {
                if (onHolPiece.transform.localScale.x > MIN_SCALE && onHolPiece.transform.localScale.y > MIN_SCALE)
                {
                    Vector3 scale = new Vector3(onHolPiece.transform.localScale.x + scale_factor * -1, onHolPiece.transform.localScale.y + scale_factor * -1, 1);
                    scale.x = (scale.x < MIN_SCALE) ? MIN_SCALE : scale.x;
                    scale.y = (scale.y < MIN_SCALE) ? MIN_SCALE : scale.y;
                    scaleFromPosition(scale, midPoint);
                }
            }
            if (onHolPiece.localScale.x > 1 - scaleSensivity && onHolPiece.localScale.x < 1 + scaleSensivity)
            {
                redGlow.gameObject.SetActive(false);
                greenGlow.gameObject.SetActive(true);
                whiteGlow.gameObject.SetActive(true);
            }
            else
            {
                if(greenGlow.gameObject.activeSelf)
                {
                    greenGlow.gameObject.SetActive(false);
                    whiteGlow.gameObject.SetActive(false);
                    redGlow.gameObject.SetActive(true);
                }
            }
        }
    }
    //Following method scales the gameobject from particular position
    static Vector3 prevPos = Vector3.zero;
    private void scaleFromPosition(Vector3 scale, Vector3 fromPos)
    {
        if (!fromPos.Equals(prevPos))
        {
            Vector3 prevParentPos = onHolPiece.transform.position;
            onHolPiece.transform.position = fromPos;
            Vector3 diff = onHolPiece.transform.position - prevParentPos;
            Vector3 pos = new Vector3(diff.x / onHolPiece.transform.localScale.x * -1, diff.y / onHolPiece.transform.localScale.y * -1, onHolPiece.position.z);
            //onHolPiece.localPosition = new Vector3(onHolPiece.localPosition.x + pos.x, onHolPiece.localPosition.y + pos.y, pos.z);
        }
        onHolPiece.transform.localScale = scale;
        prevPos = fromPos;
    }
}
