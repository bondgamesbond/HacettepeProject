using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WarningText : MonoBehaviour
{
    public Vector2 targetPosition;
    float stayTime = 3.0f, moveSpeed=400.0f, fadeSpeed=2.0f;
    Color tempColor;
    bool fadingOut;
    Text text;
    
	void Start ()
    {
        text = GetComponent<Text>();
        tempColor = text.color;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (stayTime > 0)
            stayTime -= Time.deltaTime;
        else
        {
            if (text.rectTransform.anchoredPosition != targetPosition)
                text.rectTransform.anchoredPosition = Vector2.MoveTowards(text.rectTransform.anchoredPosition, (targetPosition), Time.deltaTime * moveSpeed);
        }
        if (fadingOut)
            tempColor.a -= Time.deltaTime * fadeSpeed;
        else
            tempColor.a += Time.deltaTime * fadeSpeed;
        if (fadingOut && tempColor.a <= 0.3f)
        {
            tempColor.a = 0.3f;
            fadingOut = false;
        }
        if (!fadingOut && tempColor.a > 1)
        {
            tempColor.a = 1;
            fadingOut = true;
        }
        text.color = tempColor;
	}
}
