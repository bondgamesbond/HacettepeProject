using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Mouse : MonoBehaviour
{
    float waitTime, tempAnimTime;
    Vector3 tempTargetPos = Vector3.zero, mirrorRotation = new Vector3(0, 180, 0);
    SkeletonAnimation anim;
    Renderer bubbleParticleRenderer;

	void Start ()
    {
        anim = GetComponent<SkeletonAnimation>();
        waitTime = Random.Range(3, 5);
        tempAnimTime = Random.Range(3, 5);
        bubbleParticleRenderer = transform.GetChild(0).GetComponent<Renderer>();
        bubbleParticleRenderer.sortingLayerName = GetComponent<Renderer>().sortingLayerName;
        bubbleParticleRenderer.sortingOrder = GetComponent<Renderer>().sortingOrder + 1;
    }
	
	void Update ()
    {
        waitTime -= Time.deltaTime;
        tempAnimTime -= Time.deltaTime;
        if(waitTime<=0)
        {
            waitTime = Random.Range(3, 7);
            tempTargetPos.x = Random.Range(-8.5f, 27f);
            tempTargetPos.y = Random.Range(-15.7f, 5.1f);
            if (tempTargetPos.x > transform.localPosition.x)
                transform.eulerAngles = mirrorRotation;
            else
                transform.eulerAngles = Vector3.zero;
            transform.DOLocalMove(tempTargetPos, Random.Range(waitTime + 3, waitTime + 4), false);
        }
        if(tempAnimTime<=0)
        {
            tempAnimTime = Random.Range(3, 5);
            anim.loop = false;
            anim.AnimationName = "swim2";
        }
        if (anim != null)
        {
            if (anim.AnimationName == "swim2" && anim.state.GetCurrent(0).time>=anim.state.GetCurrent(0).endTime)
            {
                anim.loop = true;
                anim.AnimationName = "swim1";
            }
        }
	}
}
