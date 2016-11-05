using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ValveRotater : MonoBehaviour
{
    public Slider correctBar;
    public AudioSource openSound, closeSound;

    Touch touch;

    Vector2 screenRay;

    RaycastHit2D hit;

    int touchCount, onPieceTouchCount;

    float rotateSensivity = 10.0f;

    Transform onHolPiece; //, redGlow, greenGlow;

    private float baseAngle = 0.0f;

    void Start()
    {
        openSound = GameObject.Find("Sounds").transform.GetChild(3).GetComponent<AudioSource>();
        closeSound = GameObject.Find("Sounds").transform.GetChild(4).GetComponent<AudioSource>();
        correctBar = GameManager.Instance.game.GetComponentInChildren<Slider>();
    }

    void Update()
    {
        touchCount = Input.touchCount;

        if (touchCount > 0)
        {
            for (int i = 0; i < touchCount; i++)
            {
                touch = Input.GetTouch(i);
                
                if (touch.phase == TouchPhase.Began)  //On Touch Down
                {
                    screenRay = Camera.main.ScreenToWorldPoint(touch.position);
                    hit = Physics2D.Raycast(screenRay, Vector3.forward);
                    if (hit.collider != null)
                    {
                        if (hit.transform.tag == "Valve" && (onHolPiece == null || onHolPiece.name == hit.transform.name))
                        {
                            if ((!hit.transform.GetComponent<Valve>().Opened && onPieceTouchCount < 2) || GameManager.Instance.State == GameStateBoru.Leaking)
                            {
                                onPieceTouchCount++;
                                if (onPieceTouchCount == 2)
                                {
                                    onHolPiece = hit.transform;
                                    //redGlow = onHolPiece.FindChild("redGlow");
                                    //greenGlow = onHolPiece.FindChild("greenGlow");
                                    //redGlow.gameObject.SetActive(true);
                                    Vector2 pos = Camera.main.WorldToScreenPoint(onHolPiece.position);
                                    if (Input.touchCount == 3)
                                    {
                                        pos = Input.GetTouch(1).position - pos;
                                    }
                                    else
                                    {
                                        pos = Input.GetTouch(0).position - pos;
                                    }
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
                        if (GetComponent<Valve>().Opened)
                        {
                            if (onHolPiece.localEulerAngles.z > 360 - rotateSensivity)
                            {
                                onHolPiece.localEulerAngles = new Vector3(0, 0, 355);
                                onHolPiece.GetComponent<Valve>().Closed = true;
                                onHolPiece.GetComponent<Valve>().Opened = false;
                                GameManager.Instance.State = GameStateBoru.FillWater;
                                GameManager.Instance.leakingPipe.transform.GetChild(4).gameObject.SetActive(false);
                                // PieceGame.Instance.piecePlaceSound.Play();
                            }
                        }
                        else if (GetComponent<Valve>().Closed)
                        {
                            if (onHolPiece.localEulerAngles.z < 0 + rotateSensivity)
                            {
                                onHolPiece.localEulerAngles = new Vector3(0, 0, 5);
                                onHolPiece.GetComponent<Valve>().Opened = true;
                                onHolPiece.GetComponent<Valve>().Closed = false;
                                // PieceGame.Instance.piecePlaceSound.Play();
                            }
                        }
                        onHolPiece = null;
                        //if (redGlow != null)
                        //{
                        //    redGlow.gameObject.SetActive(false);
                        //    greenGlow.gameObject.SetActive(false);
                        //    redGlow = null;
                        //    greenGlow = null;
                        //}
                    }
                    onPieceTouchCount = 0;
                    // testText.text = "Cikti";
                }
            }
        }
    }

    private void checkForRotate()
    {
        if ((Input.touchCount == 2 && (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved)) ||
             Input.touchCount == 3 && (Input.GetTouch(1).phase == TouchPhase.Moved || Input.GetTouch(2).phase == TouchPhase.Moved))
        {
            Vector2 pos = Camera.main.WorldToScreenPoint(onHolPiece.position);
            if (Input.touchCount == 3)
            {
                pos = Input.GetTouch(1).position - pos;
            }
            else
            {
                pos = Input.GetTouch(0).position - pos;
            }
            float ang = Mathf.Atan2(pos.y, pos.x) * Mathf.Rad2Deg - baseAngle;

            correctBar.value = 1 - onHolPiece.localEulerAngles.z / 360; 

            if (GetComponent<Valve>().Closed)
            {
                if (onHolPiece.localEulerAngles.z > Quaternion.AngleAxis(ang, Vector3.forward).eulerAngles.z)
                {
                    if (onHolPiece.localEulerAngles.z - Quaternion.AngleAxis(ang, Vector3.forward).eulerAngles.z > 100)
                    {
                        return;
                    }
                    if(!openSound.isPlaying)
                    {
                        openSound.Play();
                    }

                    onHolPiece.rotation = Quaternion.AngleAxis(ang, Vector3.forward);
                }
            }
            else if (GetComponent<Valve>().Opened)
            {
                if (GameManager.Instance.valveCanTurn)
                {
                    if (onHolPiece.localEulerAngles.z < Quaternion.AngleAxis(ang, Vector3.forward).eulerAngles.z)
                    {
                        if (-onHolPiece.localEulerAngles.z + Quaternion.AngleAxis(ang, Vector3.forward).eulerAngles.z > 100)
                        {
                            return;
                        }
                        if (!closeSound.isPlaying)
                        {
                            closeSound.Play();
                        }

                        GameManager.Instance.leakingSound.volume = 1 - onHolPiece.localEulerAngles.z / 360;
                        onHolPiece.rotation = Quaternion.AngleAxis(ang, Vector3.forward);
                    }
                }
            }

            //if (onHolPiece.localEulerAngles.z > 0 - rotateSensivity && onHolPiece.localEulerAngles.z < 0 + rotateSensivity)
            //{
            //    redGlow.gameObject.SetActive(false);
            //    greenGlow.gameObject.SetActive(true);
            //}
            //else
            //{
            //    if (greenGlow.gameObject.activeSelf)
            //    {
            //        greenGlow.gameObject.SetActive(false);
            //        redGlow.gameObject.SetActive(true);
            //    }
            //}
        }
    }
}
