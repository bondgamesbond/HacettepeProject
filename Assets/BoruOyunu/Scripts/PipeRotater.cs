using UnityEngine;
using System.Collections;

public class PipeRotater : MonoBehaviour
{
    public AudioSource rotateSound;
    public GameObject pipe3, pipe4;

    Touch touch;

    Vector2 screenRay;

    RaycastHit2D hit;

    int touchCount, onPieceTouchCount;

    float rotateSensivity = 10.0f;

    Transform onHolPiece, redGlow, greenGlow;

    private float baseAngle = 0.0f;

    void Start()
    {
        rotateSound = GameObject.Find("Sounds").transform.GetChild(5).GetComponent<AudioSource>();
    }

    void Update()
    {
        if (onHolPiece != null)
        {
            onHolPiece.transform.position = onHolPiece.GetComponent<Pipe>().actualPos;
            onHolPiece.GetComponent<Pipe>().rotating = true;
        }

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

                        if (hit.transform.tag == "LPipe" && (onHolPiece == null || onHolPiece.name == hit.transform.name))
                        {
                            if (!hit.transform.GetComponent<Pipe>().isOnTrueRotation && hit.transform.GetComponent<Pipe>().canRotate && onPieceTouchCount < 2)
                            {
                                onPieceTouchCount++;
                                if (onPieceTouchCount == 2)
                                {
                                    onHolPiece = hit.transform;
                                    redGlow = onHolPiece.GetChild(2);
                                    greenGlow = onHolPiece.GetChild(3);
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
                    {
                        checkForRotate();
                    }
                }
                if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    if (onHolPiece != null)
                    {
                        onHolPiece.transform.position = onHolPiece.GetComponent<Pipe>().actualPos;

                        if (onHolPiece.localEulerAngles.z < 45 || onHolPiece.localEulerAngles.z > 315)
                        {
                            onHolPiece.localEulerAngles = Vector3.Scale(onHolPiece.localEulerAngles, new Vector3(1, 1, 0));
                            onHolPiece.GetComponent<Pipe>().type = 2;

                            int gridX = (int)(onHolPiece.GetComponent<Pipe>().pipePos[0]);
                            int gridY = (int)(onHolPiece.GetComponent<Pipe>().pipePos[1]);

                            if (GameManager.Instance.CheckGrid(gridY, gridX) == 2)
                            {
                                GameManager.Instance.PlaceToGrid(gridY, gridX);
                                GameManager.Instance.Place++;
                                onHolPiece.GetComponent<Pipe>().isOnTrueRotation = true;
                            }
                        }
                        else if (onHolPiece.localEulerAngles.z < 135)
                        {
                            onHolPiece.localEulerAngles = Vector3.Scale(onHolPiece.localEulerAngles, new Vector3(1, 1, 0)) + new Vector3(0, 0, 90);
                            onHolPiece.GetComponent<Pipe>().type = 3;

                            int gridX = (int)(onHolPiece.GetComponent<Pipe>().pipePos[0]);
                            int gridY = (int)(onHolPiece.GetComponent<Pipe>().pipePos[1]);

                            if (GameManager.Instance.CheckGrid(gridY, gridX) == 3)
                            {
                                int place = GameManager.Instance.CheckPlace(gridY, gridX);
                                GameObject obj = Instantiate(pipe3, onHolPiece.GetComponent<Pipe>().actualPos, Quaternion.identity) as GameObject;
                                obj.transform.parent = transform;
                                GameManager.Instance.AddPipe(place, obj.GetComponent<Pipe>());

                                obj.GetComponent<Pipe>().isOnTrueRotation = true;
                                obj.GetComponent<Pipe>().canRotate = true;
                                obj.GetComponent<Pipe>().Placed = true;
                                onHolPiece.gameObject.SetActive(false);

                                GameManager.Instance.PlaceToGrid(gridY, gridX);
                                GameManager.Instance.Place++;
                                onHolPiece.GetComponent<Pipe>().isOnTrueRotation = true;
                            }
                        }
                        else if (onHolPiece.localEulerAngles.z < 225)
                        {
                            onHolPiece.localEulerAngles = Vector3.Scale(onHolPiece.localEulerAngles, new Vector3(1, 1, 0)) + new Vector3(0, 0, 180);
                            onHolPiece.GetComponent<Pipe>().type = 4;

                            int gridX = (int)(onHolPiece.GetComponent<Pipe>().pipePos[0]);
                            int gridY = (int)(onHolPiece.GetComponent<Pipe>().pipePos[1]);

                            if (GameManager.Instance.CheckGrid(gridY, gridX) == 4)
                            {
                                int place = GameManager.Instance.CheckPlace(gridY, gridX);
                                GameObject obj = Instantiate(pipe4, onHolPiece.GetComponent<Pipe>().actualPos, Quaternion.identity) as GameObject;
                                obj.transform.parent = transform;
                                GameManager.Instance.AddPipe(place, obj.GetComponent<Pipe>());

                                obj.GetComponent<Pipe>().isOnTrueRotation = true;
                                obj.GetComponent<Pipe>().canRotate = true;
                                obj.GetComponent<Pipe>().Placed = true;
                                onHolPiece.gameObject.SetActive(false);

                                GameManager.Instance.PlaceToGrid(gridY, gridX);
                                GameManager.Instance.Place++;
                                onHolPiece.GetComponent<Pipe>().isOnTrueRotation = true;
                            }
                        }
                        else
                        {
                            onHolPiece.localEulerAngles = Vector3.Scale(onHolPiece.localEulerAngles, new Vector3(1, 1, 0)) + new Vector3(0, 0, 270);
                            onHolPiece.GetComponent<Pipe>().type = 5;

                            int gridX = (int)(onHolPiece.GetComponent<Pipe>().pipePos[0]);
                            int gridY = (int)(onHolPiece.GetComponent<Pipe>().pipePos[1]);

                            if (GameManager.Instance.CheckGrid(gridY, gridX) == 5)
                            {
                                GameManager.Instance.PlaceToGrid(gridY, gridX);
                                GameManager.Instance.Place++;
                                onHolPiece.GetComponent<Pipe>().isOnTrueRotation = true;
                            }
                        }

                        if (onHolPiece.localEulerAngles.z > 0 - rotateSensivity && onHolPiece.localEulerAngles.z < 0 + rotateSensivity)
                        {
                            onHolPiece.localEulerAngles = new Vector3(0, 0, 0);
                            // PieceGame.Instance.piecePlaceSound.Play();
                        }
                        onHolPiece.GetComponent<Pipe>().rotating = false;
                        onHolPiece = null;
                        if (redGlow != null)
                        {
                            redGlow.gameObject.SetActive(false);
                            greenGlow.gameObject.SetActive(false);
                            redGlow = null;
                            greenGlow = null;
                        }
                    }
                    onPieceTouchCount = 0;
                    // testText.text = "Cikti";
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

            int gridX = (int)(onHolPiece.GetComponent<Pipe>().pipePos[0]);
            int gridY = (int)(onHolPiece.GetComponent<Pipe>().pipePos[1]);

            if (!rotateSound.isPlaying)
            {
                rotateSound.Play();
            }
            

            if (onHolPiece.localEulerAngles.z < 45 || onHolPiece.localEulerAngles.z > 315)
            {
                if (GameManager.Instance.CheckGrid(gridY, gridX) == 2)
                {
                    redGlow.gameObject.SetActive(false);
                    greenGlow.gameObject.SetActive(true);
                }
                else
                {
                    redGlow.gameObject.SetActive(true);
                    greenGlow.gameObject.SetActive(false);
                }
            }
            else if (onHolPiece.localEulerAngles.z < 135)
            {
                if (GameManager.Instance.CheckGrid(gridY, gridX) == 3)
                {
                    redGlow.gameObject.SetActive(false);
                    greenGlow.gameObject.SetActive(true);
                }
                else
                {
                    redGlow.gameObject.SetActive(true);
                    greenGlow.gameObject.SetActive(false);
                }
            }
            else if (onHolPiece.localEulerAngles.z < 225)
            {
                if (GameManager.Instance.CheckGrid(gridY, gridX) == 4)
                {
                    redGlow.gameObject.SetActive(false);
                    greenGlow.gameObject.SetActive(true);
                }
                else
                {
                    redGlow.gameObject.SetActive(true);
                    greenGlow.gameObject.SetActive(false);
                }
            }
            else
            {
                if (GameManager.Instance.CheckGrid(gridY, gridX) == 5)
                {
                    redGlow.gameObject.SetActive(false);
                    greenGlow.gameObject.SetActive(true);
                }
                else
                {
                    redGlow.gameObject.SetActive(true);
                    greenGlow.gameObject.SetActive(false);
                }
            }
        }
    }
}
