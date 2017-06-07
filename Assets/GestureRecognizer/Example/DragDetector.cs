using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI.Extensions;
using System.Linq;
using UnityEngine.UI;

/// <summary>
/// Example class that captures player drawing and call the Recognizer to discover which gesture player did.
/// </summary>

public class DragDetector : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    public Recognizer recognizer;

    public UILineRenderer line;

    List<Vector2> points = new List<Vector2>();

    [Range(0f, 1f)]
    public float scoreToAccept;

    float checkCoolDown, clearLineTimer;

    bool canCheck, isClearing;

    public bool isOnObject;

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
            if (checkCoolDown >= 0.4f)
            {
                checkCoolDown = 0;
                canCheck = true;
            }
        }
        if (isClearing)
        {
            clearLineTimer += Time.deltaTime;
            if (clearLineTimer >= 1f && canCheck)
            {
                clearLineTimer = 0;
                points.Clear();
            }
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
        isClearing = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isOnObject)
        {
            points.Add(eventData.position);
            UpdateLine();
            if (canCheck)
            {
                canCheck = false;
                Recognizer.RecognitionResult result = recognizer.Recognize(points);

                StopAllCoroutines();

                if (result.score.score >= scoreToAccept)
                {
                    clearLineTimer = 0;
                    points.Clear();
                    PairGameManager.Instance.clearActivePiece();
                }
                else
                {
                }
            }
        }
        else
        {
            points.Clear();
            UpdateLine();
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
