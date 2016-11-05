using UnityEngine;
using System.Collections;

public class Baloon : MonoBehaviour
{
    SkeletonAnimation animation;

    public bool isBoomed = false;
    
	void Start ()
    {
        animation = GetComponent<SkeletonAnimation>();
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if(animation!=null)
        {
            if (animation.AnimationName == "")
                animation.AnimationName = "pop";
            if(animation.AnimationName=="boom" && animation.state.GetCurrent(0).time>=animation.state.GetCurrent(0).endTime)
            {
                if (transform.name != "baloonTheBest")
                    animation.AnimationName = "";
                gameObject.SetActive(false);
            }
        }
	}
}
