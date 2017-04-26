using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PairGamePiece : MonoBehaviour
{
    GameObject snowPoofParticle;

    BoxCollider2D thisBoxCollider;

    CircleCollider2D thisCircleCollider;

    Transform snowsParent;

    List<GameObject> snowList;

    int clearCount;

    public SpriteRenderer firstSnow;

	void Awake ()
    {
        snowPoofParticle = transform.FindChild("snowPoof").gameObject;
        snowsParent = transform.FindChild("Snows");
        thisBoxCollider = GetComponent<BoxCollider2D>();
        if (thisBoxCollider != null)
            thisBoxCollider.enabled = false;
        thisCircleCollider = GetComponent<CircleCollider2D>();
        if (thisCircleCollider != null)
            thisCircleCollider.enabled = false;
        snowList = new List<GameObject>();
    }
	
    public void enableCollider()
    {
        if (thisBoxCollider != null)
            thisBoxCollider.enabled = true;
        if (thisCircleCollider != null)
            thisCircleCollider.enabled = true;
    }

    public void setPieceActive(int clearNumber)
    {
        clearCount = clearNumber;
        if (clearCount == 1)
        {
            snowList.Add(snowsParent.GetChild(0).gameObject);
        }
        else if (clearCount == 2)
        {
            snowList.Add(snowsParent.GetChild(0).gameObject);
            snowList.Add(snowsParent.GetChild(2).gameObject);
        }
        else if (clearCount == 3)
        {
            snowList.Add(snowsParent.GetChild(0).gameObject);
            snowList.Add(snowsParent.GetChild(1).gameObject);
            snowList.Add(snowsParent.GetChild(3).gameObject);
        }
        else if (clearCount == 4)
        {
            snowList.Add(snowsParent.GetChild(0).gameObject);
            snowList.Add(snowsParent.GetChild(1).gameObject);
            snowList.Add(snowsParent.GetChild(2).gameObject);
            snowList.Add(snowsParent.GetChild(3).gameObject);
        }
        firstSnow = snowList[0].GetComponent<SpriteRenderer>();
    }

    public void clearPiece()
    {
        clearCount--;
        snowPoofParticle.SetActive(false);
        snowPoofParticle.SetActive(true);
        snowList[0].SetActive(false);
        snowList.Remove(snowList[0]);
        if (snowList.Count > 0)
            snowList[0].SetActive(true);
        if (clearCount == 0)
        {
            if (thisBoxCollider != null)
                thisBoxCollider.enabled = false;
            if (thisCircleCollider != null)
                thisCircleCollider.enabled = false;
        }
    }
}
