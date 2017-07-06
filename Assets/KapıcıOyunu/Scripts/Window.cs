using UnityEngine;
using System.Collections;

public class Window : MonoBehaviour
{
    GameObject light, Bars, sutBalloon, ekmekBalloon;
    Transform tentesParent;
    public bool isReadyToGetOrder, isRequesting;

    void Awake()
    {
        light = transform.Find("light").gameObject;
        Bars = transform.Find("bars").gameObject;
        sutBalloon = transform.Find("sutBalloon").gameObject;
        ekmekBalloon = transform.Find("ekmekBalloon").gameObject;
        tentesParent = transform.Find("Tents");
        StartCoroutine(getRandomSkin((float)(Random.Range(0, 10)) / 10));
        isReadyToGetOrder = false;
    }

    IEnumerator getRandomSkin(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        int tempRandom = Random.Range(0, 10);
        if (transform.GetSiblingIndex() < 2)
        {
            if (tempRandom < 6)
                Bars.SetActive(true);
            else if (tempRandom >= 6 && tempRandom < 8)
                tentesParent.GetChild(Random.Range(0, tentesParent.childCount)).gameObject.SetActive(true);
        }
        else
        {
            if (tempRandom < 5)
                tentesParent.GetChild(Random.Range(0, tentesParent.childCount)).gameObject.SetActive(true);
        }
    }

    public void setReadyToTakeOrder()
    {
        light.SetActive(true);
        isReadyToGetOrder = true;
    }

    public void setNotReadyToTakeOrder()
    {
        light.SetActive(false);
        isReadyToGetOrder = false;
    }

    public void requestOrder(bool isEkmek)
    {
        if (isEkmek)
            ekmekBalloon.SetActive(true);
        else
            sutBalloon.SetActive(true);
    }

    public void completeOrder()
    {
        if (ekmekBalloon.activeSelf)
            ekmekBalloon.SetActive(false);
        if (sutBalloon.activeSelf)
            sutBalloon.SetActive(false);
        light.SetActive(false);
        isReadyToGetOrder = false;
    }
}
