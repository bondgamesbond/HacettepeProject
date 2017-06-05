using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI.Extensions;
using System.Linq;
using UnityEngine.UI;

public class MoveRecognizer : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Recognizer recognizer;

    public UILineRenderer line;

    List<Vector2> points = new List<Vector2>();

    public Text labelResult;

    public Transform referenceRoot;

    Dictionary<GesturePattern, GameObject> references;

    [Range(0f, 1f)]
    public float scoreToAccept = 0.65f;

    float checkCoolDown, clearLineTimer;

    bool canCheck, isClearing;

    void Start()
    {
        canCheck = false;
        UpdateLine();
    }

    void Update()
    {
        if (!canCheck)
        {
            checkCoolDown += Time.deltaTime;
            if (checkCoolDown >= 0.5f)
            {
                checkCoolDown = 0;
                canCheck = true;
            }
        }
        if (isClearing)
        {
            clearLineTimer += Time.deltaTime;
            if (clearLineTimer >= 1.5f && canCheck)
            {
                clearLineTimer = 0;
                points.Clear();
            }
        }
    }

    void ActiveAllDrawings()
    {
        foreach (var line in references.Values)
        {
            line.SetActive(true);
        }
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
        labelResult.text = "";
        isClearing = true;
        Debug.Log("Here");
    }

    public void OnDrag(PointerEventData eventData)
    {
        points.Add(eventData.position);
        UpdateLine();
        if (canCheck)
        {
            canCheck = false;
            Recognizer.RecognitionResult result = recognizer.Recognize(points);

            StopAllCoroutines();
            ActiveAllDrawings();

            if (result.score.score >= scoreToAccept)
            {
                labelResult.text = Mathf.RoundToInt(result.score.score * 100) + "%";
                StartCoroutine(Blink(references[result.pattern]));
                clearLineTimer = 0;
                points.Clear();
            }
            else
            {
                labelResult.text = "?";
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isClearing = false;
        clearLineTimer = 0;
        //    points.Add(eventData.position);
        //    UpdateLine();
        //    Recognizer.RecognitionResult result = recognizer.Recognize(points);

        //    StopAllCoroutines();
        //    ActiveAllDrawings();

        //    if (result.score.score >= scoreToAccept)
        //    {
        //        labelResult.text = Mathf.RoundToInt(result.score.score * 100) + "%";
        //        StartCoroutine(Blink(references[result.pattern]));
        //    }
        //    else
        //    {
        //        labelResult.text = "?";
        //    }

    }
}
