using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PieceRotater : MonoBehaviour
{
    Touch touch;

    Ray screenRay;

    RaycastHit hit;

    int touchCount, onPieceTouchCount;

    float rotateSensivity = 17.5f;

    Transform onHolPiece, redGlow, greenGlow, whiteGlow;

    public Text testText;

    private float baseAngle = 0.0f;

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
                                if (!hit.transform.GetComponent<Piece>().isOnTrueRotation && onPieceTouchCount < 2)
                                {
                                    onPieceTouchCount++;
                                    if (onPieceTouchCount == 2)
                                    {
                                        onHolPiece = hit.transform;
                                        redGlow = onHolPiece.FindChild("redGlow");
                                        greenGlow = onHolPiece.FindChild("greenGlow");
                                        whiteGlow = onHolPiece.FindChild("whiteGlow");
                                        redGlow.gameObject.SetActive(true);
                                        Vector2 pos = Camera.main.WorldToScreenPoint(onHolPiece.position);
                                        pos = Input.GetTouch(0).position - pos;
                                        baseAngle = Mathf.Atan2(pos.y, pos.x) * Mathf.Rad2Deg;
                                        baseAngle -= Mathf.Atan2(onHolPiece.right.y, onHolPiece.right.x) * Mathf.Rad2Deg;
                                    }
                                }
                            }
                        }
                    }
                    if (touch.phase == TouchPhase.Moved)
                    {
                        if (onHolPiece != null)
                            checkForRotate();
                    }
                    if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                    {
                        if (onHolPiece != null)
                        {
                            if (onHolPiece.localEulerAngles.z > 0 - rotateSensivity && onHolPiece.localEulerAngles.z < 0 + rotateSensivity)
                            {
                                onHolPiece.localEulerAngles = new Vector3(0, 0, 0);
                                onHolPiece.GetComponent<Piece>().isOnTrueRotation = true;
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

    private void checkForRotate()
    {
        if (Input.touchCount == 2 && (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved))
        {
            Vector2 pos = Camera.main.WorldToScreenPoint(onHolPiece.position);
            pos = Input.GetTouch(0).position - pos;
            float ang = Mathf.Atan2(pos.y, pos.x) * Mathf.Rad2Deg - baseAngle;
            onHolPiece.rotation = Quaternion.AngleAxis(ang, Vector3.forward);

            if (onHolPiece.localEulerAngles.z > 0 - rotateSensivity && onHolPiece.localEulerAngles.z < 0 + rotateSensivity)
            {
                redGlow.gameObject.SetActive(false);
                greenGlow.gameObject.SetActive(true);
                whiteGlow.gameObject.SetActive(true);
            }
            else
            {
                if (greenGlow.gameObject.activeSelf)
                {
                    greenGlow.gameObject.SetActive(false);
                    redGlow.gameObject.SetActive(true);
                    whiteGlow.gameObject.SetActive(false);
                }
            }
        }
    }

    //void OnMouseDown()
    //{
    //    Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
    //    pos = Input.mousePosition - pos;
    //    baseAngle = Mathf.Atan2(pos.y, pos.x) * Mathf.Rad2Deg;
    //    baseAngle -= Mathf.Atan2(transform.right.y, transform.right.x) * Mathf.Rad2Deg;
    //}

    //void OnMouseDrag()
    //{
    //    Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
    //    pos = Input.mousePosition - pos;
    //    float ang = Mathf.Atan2(pos.y, pos.x) * Mathf.Rad2Deg - baseAngle;
    //    transform.rotation = Quaternion.AngleAxis(ang, Vector3.forward);
    //}
}
