using UnityEngine;
using System.Collections;

public class ButtonAnim : MonoBehaviour
{
    public float speed = 0.25f, maxScale = 1.1f;
    bool isActive = false, isRising;
    Vector3 tempScale = Vector3.one;
    
	void Start ()
    {
       
    }

    void OnEnable()
    {
        isActive = true;
        isRising = true;
    }

    void OnDisable()
    {
        isActive = false;
        tempScale = Vector3.one;
        transform.localScale = tempScale;
        isRising = true;
    }
	
	void Update ()
    {
        if (isActive)
        {
            if(isRising)
            {
                if (tempScale.x < maxScale)
                {
                    tempScale.x += Time.deltaTime * speed;
                    tempScale.y = tempScale.x;
                }
                else
                    isRising = false;
            }
            else
            {
                if (tempScale.x > 1.0f)
                {
                    tempScale.x -= Time.deltaTime * speed;
                    tempScale.y = tempScale.x;
                }
                else
                    isRising = true;
            }
            transform.localScale = tempScale;
        }
	}
}
