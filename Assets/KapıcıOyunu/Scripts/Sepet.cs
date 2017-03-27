using UnityEngine;
using System.Collections;

public class Sepet : MonoBehaviour
{
    public static Sepet Instance;
    public GameObject ekmek, sut;
    public BoxCollider sepetCollider;
    public Transform arrow;
    public SpriteRenderer arrowSprite, sepetGlowSprite;
    public float arrowRotateSpeed;
    float closestRequestedFloorValue;
    public Transform closestTargetFloor;
    Vector3 tempArrowRotation;

	void Start ()
    {
        if (Instance != null)
            Destroy(gameObject);
        else
            Instance = this;
        sepetCollider = GetComponent<BoxCollider>();
        sepetCollider.enabled = false;
    }
	
	void Update ()
    {
        if (KapiciManager.Instance.requestingFloorIds.Count > 0)
        {
            closestRequestedFloorValue = 100;
            for (int i = 0; i < KapiciManager.Instance.requestingFloorIds.Count; i++)
            {
                if (Mathf.Abs(transform.position.y - KapiciManager.Instance.floarYValues[KapiciManager.Instance.requestingFloorIds[i]]) <= closestRequestedFloorValue)
                {
                    closestRequestedFloorValue = Mathf.Abs(transform.position.y - KapiciManager.Instance.floarYValues[KapiciManager.Instance.requestingFloorIds[i]]);
                    closestTargetFloor = KapiciManager.Instance.activeWindows[KapiciManager.Instance.requestingFloorIds[i]].transform;
                }
            }
            if (closestTargetFloor != null)
            {
                //arrow.LookAt(closestTargetFloor.position);
                //tempArrowRotation = arrow.eulerAngles;
                //tempArrowRotation.z = tempArrowRotation.x;
                //tempArrowRotation.x = 0;
                //tempArrowRotation.y = 0;
                //arrow.eulerAngles = tempArrowRotation;
                if (Mathf.Abs(transform.position.y - closestTargetFloor.position.y + 0.24f) <= KapiciManager.Instance.floarThreshold)
                {
                    arrowSprite.color = Color.green;
                    sepetCollider.enabled = true;
                    sepetGlowSprite.enabled = true;
                    if (KapiciManager.Instance.isOnTutorial && MakaraRotater.Instance.thisCollider.enabled)
                    {
                        MakaraRotater.Instance.thisCollider.enabled = false;
                        KapiciManager.Instance.getNextTutorial();
                    }
                }
                else
                { 
                    arrowSprite.color = Color.red;
                    sepetCollider.enabled = false;
                    sepetGlowSprite.enabled = false;
                }
            }
            else
            {
                arrowSprite.color = Color.red;
                sepetGlowSprite.enabled = false;
                closestTargetFloor = null;
                sepetCollider.enabled = false;
            }
        }
        else
        {
            arrowSprite.color = Color.red;
            sepetGlowSprite.enabled = false;
            closestTargetFloor = null;
        }
	}

    public void swapSutAndEkmek()
    {
        Vector3 tempVector;
        tempVector = sut.transform.localPosition;
        sut.transform.localPosition = ekmek.transform.localPosition;
        ekmek.transform.localPosition = tempVector;
        tempVector = ekmek.transform.eulerAngles;
        tempVector.z = (-tempVector.z);
        ekmek.transform.eulerAngles = tempVector;
        tempVector = sut.transform.eulerAngles;
        tempVector.z = (-tempVector.z);
        sut.transform.eulerAngles = tempVector;
    }
}
