using UnityEngine;
using System.Collections;

public class IntroScript : MonoBehaviour
{
    private RCC_CarControllerV3 carController;
    public GameObject introCamera, mainCamera, gameCanvas, dragCanvas, raceTrack, reflectionProbe;

    public Vector2 carStopCheckPositionXZ = new Vector2(-81.35046f, 77.23959f);
    public Transform carStopPositionObject;

    public Transform onSolTeker, onSagTeker, arkaTeker1, arkaTeker2;
    public float onTekerlerRotationX, arkaTekerlerRotationX;

    private Rigidbody carRigidbody;
    private bool carStopped;

    public void Start()
    {
        carController = GetComponent<RCC_CarControllerV3>();
        carRigidbody = GetComponent<Rigidbody>();
        carStopped = false;
        Invoke("GiveGas", 1);
    }

    public void FixedUpdate()
    {
        if (!carStopped && transform.position.x >= carStopCheckPositionXZ.x && transform.position.z >= carStopCheckPositionXZ.y)
        {
            carStopped = true;
            carRigidbody.isKinematic = true;
            carController.enabled = false;
            transform.position = carStopPositionObject.position;
            transform.rotation = carStopPositionObject.rotation;
            onSolTeker.localEulerAngles = new Vector3(onTekerlerRotationX, 0, 0);
            onSagTeker.localEulerAngles = new Vector3(onTekerlerRotationX, 0, 0);
            arkaTeker1.localEulerAngles = new Vector3(arkaTekerlerRotationX, 0, 0);
            arkaTeker2.localEulerAngles = new Vector3(arkaTekerlerRotationX, 0, 0);
            //Debug.Log("wheel x rotations: " + onSolTeker.rotation.eulerAngles.x + " " + onSagTeker.rotation.eulerAngles.x + " " + arkaTeker1.rotation.eulerAngles.x + " " + arkaTeker2.rotation.eulerAngles.x);
            Invoke("OpenCamera", 2);
        }
    }

    public void GiveGas()
    {
        carController.gasInput = 1f;
    }

    public void OpenCamera()
    {
        introCamera.SetActive(false);
        mainCamera.SetActive(true);
        gameCanvas.SetActive(true);
        dragCanvas.SetActive(true);
        GameManagerAraba.Instance.currentMaskCameraObject.SetActive(true);
        carController.gameObject.SetActive(false);
        raceTrack.SetActive(false);
        reflectionProbe.SetActive(false);
    }
}
