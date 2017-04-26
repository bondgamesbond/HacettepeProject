using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class MoveRecognizer : MonoBehaviour
{
    public Recognizer recognizer;

    public UILineRenderer line;

    List<Vector2> points = new List<Vector2>();

    [Range(0f, 1f)]
    public float scoreToAccept = 0.65f;

    void Start()
    {
        UpdateLine();
    }


    IEnumerator Blink(GameObject go)
    {
        var seconds = new WaitForSeconds(0.1f);
        for (int i = 0; i <= 20; i++)
        {
            go.SetActive(i % 2 == 0);
            yield return seconds;
        }
    }

    public void UpdateLine()
    {
        if (line)
        {
            line.Points = points.ToArray();
            line.SetAllDirty();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        points.Clear();
        points.Add(eventData.position);
        UpdateLine();
    }

    public void OnDrag(PointerEventData eventData)
    {
        points.Add(eventData.position);
        UpdateLine();
    }



    public void OnEndDrag(PointerEventData eventData)
    {
        points.Add(eventData.position);
        UpdateLine();
        Recognizer.RecognitionResult result = recognizer.Recognize(points);

        StopAllCoroutines();

        if (result.score.score >= scoreToAccept)
        {
            Debug.Log("True");
        }
        else
        {
            Debug.Log("False");
        }

    }
}
