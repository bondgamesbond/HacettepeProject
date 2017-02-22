using UnityEngine;
using System.Collections;

public class Window : MonoBehaviour
{
    public GameObject light, Bars, sütBalloon, ekmekBalloon;
    public Transform tentesParent;

    void Awake()
    {
        light = transform.FindChild("light").gameObject;
        Bars = transform.FindChild("bars").gameObject;
        sütBalloon = transform.FindChild("sutBalloon").gameObject;
        ekmekBalloon = transform.FindChild("ekmekBalloon").gameObject;
        tentesParent = transform.FindChild("Tents");
        StartCoroutine(getRandomSkin((float)(Random.Range(0, 10)) / 10));
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
}
