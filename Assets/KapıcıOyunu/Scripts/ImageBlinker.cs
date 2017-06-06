using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageBlinker : MonoBehaviour
{
    public float blinkSpeed;
    Image thisImage;
    Color tempColor;
    bool isFadingOut;

	void Start ()
    {
        thisImage = GetComponent<Image>();
        tempColor = Color.white;
        isFadingOut = true;
	}
	
	void Update ()
    {
		if(isFadingOut)
        {
            tempColor.a -= Time.deltaTime * blinkSpeed;
            thisImage.color = tempColor;
            if (tempColor.a <= 0.4f)
                isFadingOut = false;
        }
        else
        {
            tempColor.a += Time.deltaTime * blinkSpeed;
            thisImage.color = tempColor;
            if (tempColor.a >= 1.0f)
                isFadingOut = true;
        }
	}
}
