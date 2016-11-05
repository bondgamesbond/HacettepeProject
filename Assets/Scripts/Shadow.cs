using UnityEngine;
using System.Collections;

public class Shadow : MonoBehaviour
{
    public Vector3 position;
    public Quaternion rotation;
    Vector3 tempScale = Vector3.one;
    
	void Start ()
    {
	
	}
	
	void Update ()
    {
        if (transform.position != position)
            transform.position = position;
        if (transform.rotation != rotation)
            transform.rotation = rotation;
        if (transform.parent.localScale != Vector3.one)
        {
            tempScale.x = 1 / transform.parent.localScale.x;
            tempScale.y = 1 / transform.parent.localScale.y;
            transform.localScale = tempScale;
        }
	}
}
