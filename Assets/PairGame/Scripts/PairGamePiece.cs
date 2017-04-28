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

    int clearCount, initialClearCount;

    public bool isClear;

    public SpriteRenderer firstSnow;

    SpriteRenderer glow;

    Color tempColor;

	void Awake ()
    {
        snowPoofParticle = transform.FindChild("snowPoof").gameObject;
        snowsParent = transform.FindChild("Snows");
        glow = transform.FindChild("glow").GetComponent<SpriteRenderer>();
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
        initialClearCount = clearNumber;
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
        if (PairGameManager.Instance.firtPiece == null)
            PairGameManager.Instance.firtPiece = this;
        else if (PairGameManager.Instance.secondPiece == null && transform.name != PairGameManager.Instance.firtPiece.name)
            PairGameManager.Instance.secondPiece = this;
        if (clearCount == 0)
        {
            isClear = true;
            if (thisBoxCollider != null)
                thisBoxCollider.enabled = false;
            if (thisCircleCollider != null)
                thisCircleCollider.enabled = false;
            if (PairGameManager.Instance.secondPiece == null)
            {
                StartCoroutine(shine());
            }
            else
            {
                PairGameManager.Instance.comparePieces();
            }
        }
    }

    IEnumerator shine()
    {
        tempColor = Color.white;
        tempColor.a = 0;
        glow.color = tempColor;
        while (tempColor.a < 1)
        {
            tempColor.a += 0.04f;
            yield return new WaitForSeconds(Time.deltaTime * 0.8f);
            glow.color = tempColor;
        }
        while (tempColor.a > 0)
        {
            tempColor.a -= 0.025f;
            yield return new WaitForSeconds(Time.deltaTime * 1.0f);
            glow.color = tempColor;
        }
    }

    IEnumerator pieceResetter()
    {
        yield return new WaitForSeconds(1f);
        tempColor = Color.white;
        tempColor.a = 0;
        firstSnow.color = tempColor;
        firstSnow.gameObject.SetActive(true);
        while (tempColor.a < 1)
        {
            tempColor.a += 0.04f;
            yield return new WaitForSeconds(Time.deltaTime * 1.0f);
            firstSnow.color = tempColor;
        }
        if (thisBoxCollider != null)
            thisBoxCollider.enabled = true;
        if (thisCircleCollider != null)
            thisCircleCollider.enabled = true;
    }

    public void completePiece()
    {
        StartCoroutine(shine());
        // thumbs Up Anim
    }

    public void resetPiece()
    {
        isClear = false;
        clearCount = initialClearCount;
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
        StartCoroutine(pieceResetter());
    }
}
