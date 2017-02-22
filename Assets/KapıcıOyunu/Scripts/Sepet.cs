using UnityEngine;
using System.Collections;

public class Sepet : MonoBehaviour
{
    public GameObject ekmek, sut;
    public Transform arrow;
    public SpriteRenderer arrowSprite;
    public float arrowRotateSpeed;
    float closestRequestedFloorValue;
    public Transform closestTargetFloor;
    Vector3 tempArrowRotation;

	void Start ()
    {
	
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
                    closestTargetFloor = KapiciManager.Instance.activeWindows.GetChild(KapiciManager.Instance.requestingFloorIds[i]);
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
                    arrowSprite.color = Color.green;
                else
                    arrowSprite.color = Color.red;
            }
            else
            {
                arrow.eulerAngles = Vector3.zero;
                arrowSprite.color = Color.red;
                closestTargetFloor = null;
            }
        }
        else
        {
            arrow.eulerAngles = Vector3.zero;
            arrowSprite.color = Color.red;
            closestTargetFloor = null;
        }
	}
}
