using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class KapiciManager : MonoBehaviour
{
    public static KapiciManager Instance;
    public Transform leftWindows, rightWindows, makara, rope, sepet, activeWindows;
    List<Window> leftWindowList, rightWindowList;
    public float[] floarYValues;
    public float floarThreshold;
    public List<int> requestingFloorIds;
    bool isLeftSided;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
            
	void Start ()
    {
        isLeftSided = true;
        requestingFloorIds = new List<int>();
        leftWindowList = new List<Window>();
        rightWindowList = new List<Window>();
        for (int i = 0; i < leftWindows.childCount; i++)
        {
            leftWindowList.Add(leftWindows.GetChild(i).GetComponent<Window>());
            rightWindowList.Add(rightWindows.GetChild(i).GetComponent<Window>());
        }
        if (isLeftSided)
            activeWindows = leftWindows;
        else
            activeWindows = rightWindows;
	}
	
	void Update ()
    {
	
	}
}
