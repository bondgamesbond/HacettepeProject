using UnityEngine;
using System.Collections;

public class InputManagerAraba : MonoBehaviour
{
    /* Motion of the touch of the player */
    public MotionAraba motion;

    /* Flag to check if the touch input has moved */
    private bool moved;

    /* Touch input and first touch position */
    private Touch touch;
    private Vector2 firstTouchPosition;

    private void Update()
    {
        /* Check the motion */
        CheckMotion();
    }

    /// <summary>
    /// Change the motion according to the touch input.
    /// </summary>
    private void CheckMotion()
    {
        /* There is at least one touch */
        if (Input.touchCount > 0)
        {
            /* Get the first touch only */
            touch = Input.GetTouch(0);

            /* Determine the action according to the touch phase... */
            if (touch.phase == TouchPhase.Began)
            {
                /* ...began and get the first touch position */
                firstTouchPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                /* ...moved */
                moved = true;
            }
            else if (moved)
            {
                /* Touch is moved and become... */
                if (touch.phase == TouchPhase.Stationary)
                {
                    /* ...stationary and determine the motion */
                    DetermineMotion(touch.position);
                    moved = false;
                    firstTouchPosition = touch.position;
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    /* ...ended and determine the motion */
                    DetermineMotion(touch.position);
                    moved = false;
                }
                return;
            }
        }
    }

    /// <summary>
    /// Determine the motion with using the first position of the touch and
    /// given new touch position.
    /// </summary>
    /// <param name="touchPosition">Last touch position to find motion.</param>
    private void DetermineMotion(Vector2 touchPosition)
    {
        /* Difference between the last touch position and first touch position */
        Vector2 difference = touchPosition - firstTouchPosition;
        float difX = Mathf.Abs(difference.x);
        float difY = Mathf.Abs(difference.y);

        /* According to the difX / difY motion is... */
        if (difX / difY >= 4.0f)
        {
            /* ...horizontal */
            motion = MotionAraba.Horizontal;
        }
        else if (difX / difY <= 0.25f)
        {
            /* ...vertical */
            motion = MotionAraba.Vertical;
        }
        else if (difX / difY <= 2.0f && difX / difY >= 0.5f)
        {
            /* ...spherical */
            motion = MotionAraba.Spherical;
        }
        else
        {
            /* ...complex */
            motion = MotionAraba.Complex;
        }
    }
}

/* Enumeration for the motions of the touch input */
public enum MotionAraba
{
    Horizontal,
    Vertical,
    Spherical,
    Complex
}
