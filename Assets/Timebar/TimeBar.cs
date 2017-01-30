using UnityEngine;
using System.Collections;

public class TimeBar : MonoBehaviour
{
    private static TimeBar myInstance = null;

    Renderer renderer;
    float totalTime, tempTimer;
    public bool timeOver;
    bool isActive = false;

    public static TimeBar Instance
    {
        get
        {
            if (myInstance == null)
            {
                myInstance = FindObjectOfType(typeof(TimeBar)) as TimeBar;
            }
            if (myInstance == null)
            {
                GameObject obj = new GameObject("TimeBar");
                myInstance = obj.AddComponent(typeof(TimeBar)) as TimeBar;
            }
            return myInstance;
        }
    }

    void Start ()
    {
        renderer = GetComponent<Renderer>();
    }

    public void startTimer(float time)
    {
        gameObject.SetActive(true);
        tempTimer = 0;
        totalTime = time;
        timeOver = false;
        isActive = true;
    }

    void Update()
    {
        if (isActive)
        {
            if (tempTimer < totalTime)
            {
                tempTimer += Time.deltaTime;
                if (tempTimer >= totalTime - 5.0f)
                {
                    if (renderer.material.GetColor("_Color") != Color.red)
                        renderer.material.SetColor("_Color", Color.red);
                }
                if (tempTimer / totalTime > 0)
                    renderer.material.SetFloat("_Cutoff", tempTimer / totalTime);
                else
                    renderer.material.SetFloat("_Cutoff", 1.05f);
            }
            else
            {
                renderer.material.SetFloat("_Cutoff", 1.05f);
                isActive = false;
                timeOver = true;
            }
        }
    }
}
