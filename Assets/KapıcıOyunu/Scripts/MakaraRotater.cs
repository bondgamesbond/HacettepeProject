using UnityEngine;
using System.Collections;

public class MakaraRotater : MonoBehaviour
{
    public static MakaraRotater Instance = null;

    Touch touch;

    Ray screenRay;

    RaycastHit hit;

    public BoxCollider thisCollider;

    public Transform sepet, rope;

    int touchCount, onPieceTouchCount;

    float rotateSensivity = 17.5f, baseAngle = 0.0f, moveSpeedDivider = 100f;

    public float maxSepetPos, minSepetPos, maxRopeScale, minRopeScale;

    public bool onHold;

    Vector3 tempPos, tempScale;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start ()
    {
        onHold = false;
        thisCollider = GetComponent<BoxCollider>();
        thisCollider.enabled = false;
	}
	
	void Update ()
    {
        if (!KapiciManager.Instance.isGameOver)
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
                            if (hit.transform.tag == transform.tag && !onHold)
                            {
                                if (KapiciManager.Instance.isOnTutorial && KapiciManager.Instance.tutorialId == 3)
                                    KapiciManager.Instance.removeTutorialAnim();
                                if (onPieceTouchCount < 2)
                                {
                                    onPieceTouchCount++;
                                    if (onPieceTouchCount == 2)
                                    {
                                        onHold = true;
                                        Vector2 pos = Camera.main.WorldToScreenPoint(transform.position);
                                        pos = Input.GetTouch(0).position - pos;
                                        baseAngle = Mathf.Atan2(pos.y, pos.x) * Mathf.Rad2Deg;
                                        baseAngle -= Mathf.Atan2(transform.right.y, transform.right.x) * Mathf.Rad2Deg;
                                    }
                                }
                            }
                        }
                    }
                    if (touch.phase == TouchPhase.Moved)
                    {
                        if (onHold)
                            checkForRotate();
                    }
                    if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                    {
                        if (onHold)
                        {
                            //min ve max koşulları
                            onHold = false;
                        }
                        onPieceTouchCount = 0;
                    }
                }
            }
        }
    }

    Vector3 tempRotation;
    private void checkForRotate()
    {
        if (Input.touchCount == 2 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            float previousAng = transform.eulerAngles.z;
            Vector2 pos = Camera.main.WorldToScreenPoint(transform.position);
            pos = Input.GetTouch(0).position - pos;
            float ang = Mathf.Atan2(pos.y, pos.x) * Mathf.Rad2Deg - baseAngle;
            if (ang < 0)
                ang += 360;
            //Debug.Log("Prev: " + previousAng + " Ang: " + ang + " BaseAng: " + baseAngle);
            if (Mathf.Abs(ang - previousAng) < 30 || (ang > 345 && previousAng < 15) || (previousAng > 345 && ang < 15))
            {
                transform.rotation = Quaternion.AngleAxis(ang, Vector3.forward);
                if (previousAng < 0)
                    previousAng += 360;
                if (transform.eulerAngles.z < 0)
                {
                    tempRotation = transform.eulerAngles;
                    tempRotation.z += 360;
                    transform.eulerAngles = tempRotation;
                }
                //Debug.Log("Previous: " + previousAng + " " + "Last: " + transform.eulerAngles.z);
                if ((previousAng > 0 && previousAng < 100 && transform.eulerAngles.z > 300) || (previousAng > 300 && transform.eulerAngles.z < 0 && transform.eulerAngles.z < 100))
                {
                    if ((KapiciManager.Instance.isOnTutorial && !Sepet.Instance.sepetCollider.enabled) || !KapiciManager.Instance.isOnTutorial)
                        moveSepet(360 - Mathf.Abs(transform.eulerAngles.z - previousAng));
                }
                else
                {
                    if ((KapiciManager.Instance.isOnTutorial && !Sepet.Instance.sepetCollider.enabled) || !KapiciManager.Instance.isOnTutorial)
                        moveSepet(transform.eulerAngles.z - previousAng);
                }
            }
        }
        else
        {
            Vector2 pos = Camera.main.WorldToScreenPoint(transform.position);
            pos = Input.GetTouch(0).position - pos;
            baseAngle = Mathf.Atan2(pos.y, pos.x) * Mathf.Rad2Deg;
            baseAngle -= Mathf.Atan2(transform.right.y, transform.right.x) * Mathf.Rad2Deg;
        }
    }

    void moveSepet(float amount)
    {
        //Debug.Log(amount);
        if (amount < 20 && amount > -20)
        {
            tempPos = sepet.localPosition;
            //Debug.Log(tempPos.y + "," + amount);
            if (KapiciManager.Instance.isLeftSided)
            {
                if (tempPos.y + (amount / moveSpeedDivider) < maxSepetPos && tempPos.y + (amount / moveSpeedDivider) > minSepetPos)
                {
                    tempPos.y += (amount / moveSpeedDivider);
                    sepet.localPosition = tempPos;
                    tempScale = rope.localScale;
                    tempScale.y = Mathf.Lerp(minRopeScale, maxRopeScale, Mathf.Abs(maxSepetPos - sepet.localPosition.y) / (maxSepetPos - minSepetPos));
                    rope.localScale = tempScale;
                    if (amount != 0)
                    {
                        if (!KapiciManager.Instance.makaraRotateSound.isPlaying)
                            KapiciManager.Instance.makaraRotateSound.Play();
                    }
                }
            }
            else
            {
                if (tempPos.y - (amount / moveSpeedDivider) < maxSepetPos && tempPos.y - (amount / moveSpeedDivider) > minSepetPos)
                {
                    tempPos.y -= (amount / moveSpeedDivider);
                    sepet.localPosition = tempPos;
                    tempScale = rope.localScale;
                    tempScale.y = Mathf.Lerp(minRopeScale, maxRopeScale, Mathf.Abs(maxSepetPos - sepet.localPosition.y) / (maxSepetPos - minSepetPos));
                    rope.localScale = tempScale;
                    if (amount != 0)
                    {
                        if (!KapiciManager.Instance.makaraRotateSound.isPlaying)
                            KapiciManager.Instance.makaraRotateSound.Play();
                    }
                }
            }
            //Debug.Log(sepet.localPosition.y);
        }
    }
}
