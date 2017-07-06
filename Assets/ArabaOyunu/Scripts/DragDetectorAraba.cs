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

public class DragDetectorAraba : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public enum DragTypeAraba
    {
        None,
        Horizontal,
        Vertical,
        Circular,
        AllMotions
    }

    public Recognizer recognizer { get; set; }

    List<Vector2> points = new List<Vector2>();
    List<float> pointTimes = new List<float>();
    int removedPointsCount;

    [Range(0f, 1f)]
    public float scoreToAccept = 0.65f;

    public float pointUsageDuration = 0.1f;

    public int minPointsCount, maxPointsCount;

    public float dragTypeResetInterval = 0.5f;
    float dragTypeResetTime;

    public bool isOnCar { get; set; }

    [HideInInspector]
    public bool allMotionsAllowed;

    public DragTypeAraba currentDragType { get; set; }

    public float lastRightDragTypeTime;

    void Start()
    {
        lastRightDragTypeTime = Time.time;
    }

    void Update()
    {
        if (!allMotionsAllowed)
        {
            dragTypeResetTime += Time.deltaTime;
            if (dragTypeResetTime > dragTypeResetInterval)
            {
                dragTypeResetTime = 0;
                points.Clear();
                pointTimes.Clear();
                currentDragType = DragTypeAraba.None;
            }
            //else
            //{
            //    lastRightDragTypeTime = Time.time;
            //}

            for (int i = 0; i < points.Count; i++)
            {
                if (pointTimes[i] + pointUsageDuration < Time.time)
                {
                    removedPointsCount++;
                }
                else
                {
                    break;
                }
            }
            if (removedPointsCount > 0)
            {
                points.RemoveRange(0, removedPointsCount);
                pointTimes.RemoveRange(0, removedPointsCount);
                removedPointsCount = 0;

                //if (points.Count == 0)
                //{
                //    currentDragType = DragTypeAraba.AllMotions;
                //}
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log("BeginDrag");

        if (allMotionsAllowed)
        {
            currentDragType = DragTypeAraba.AllMotions;
            //lastRightDragTypeTime = Time.time;
        }
        //else
        //{
        //    points.Add(eventData.position);
        //    pointTimes.Add(Time.time);
        //}
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!allMotionsAllowed)
        {
            points.Add(eventData.position);
            pointTimes.Add(Time.time);
            if (maxPointsCount > 0 && points.Count > maxPointsCount)
            {
                points.RemoveRange(0, points.Count - maxPointsCount);
                pointTimes.RemoveRange(0, pointTimes.Count - maxPointsCount);
            }

            if (isOnCar && points.Count >= minPointsCount)
            {
                Recognizer.RecognitionResult result = recognizer.Recognize(points);

                if (result.score.score >= scoreToAccept)
                {
                    dragTypeResetTime = 0;
                    switch (result.pattern.id)
                    {
                        case "horizontal":
                            currentDragType = DragTypeAraba.Horizontal;
                            break;
                        case "vertical":
                            currentDragType = DragTypeAraba.Vertical;
                            break;
                        case "circular":
                            currentDragType = DragTypeAraba.Circular;
                            break;
                        default:
                            currentDragType = DragTypeAraba.AllMotions;
                            break;
                    }
                    //lastRightDragTypeTime = Time.time;
                }
                else
                {
                    //currentDragType = DragTypeAraba.AllMotions;
                }
                //Debug.Log("score: " + result.score.score + " - " + currentDragType.ToString() + " - points: " + points.Count);
            }
            else
            {
                //Debug.Log("score: none - " + currentDragType.ToString() + " - points: " + points.Count);

                //currentDragType = DragTypeAraba.AllMotions;
            }
        }
        //else
        //{
        //    lastRightDragTypeTime = Time.time;
        //}
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //Debug.Log("EndDrag");

        if (!allMotionsAllowed)
        {
            points.Clear();
            pointTimes.Clear();
        }
        currentDragType = DragTypeAraba.None;
    }
}
