using UnityEngine;
using System.Collections;

public class Valve : MonoBehaviour
{

    public bool Placed { get; set; }
    public bool Opened;
    public bool Closed;

    private Vector3 offset;
    private GameObject collidingValve;

    void Start()
    {
        Closed = true;

    }

    /*void OnMouseDown()
    {
        if (!Placed)
        {
            offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        }
    }

    void OnMouseDrag()
    {
        if (!Placed)
        {
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
            Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;

            transform.position = curPosition;
        }
    }

    void OnMouseUp()
    {
        if (!Placed)
        {
            if (collidingValve != null)
            {
                transform.position = collidingValve.transform.position;
                Placed = true;
                GetComponent<ValveRotater>().enabled = true;
            }
            else
            {
                transform.position = new Vector3(18, -7, 0);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name == "ValveShadow")
        {
            collidingValve = other.gameObject;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.name == "ValveShadow")
        {
            collidingValve = null;
        }
    }*/
}
