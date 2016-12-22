using UnityEngine;
using System.Collections;

public class IntroScript : MonoBehaviour
{
    private RCC_CarControllerV3 carController;
    public GameObject introCamera;
    public GameObject mainCamera;
    public GameObject gameCanvas;

    public void Start()
    {
        carController = GetComponent<RCC_CarControllerV3>();
        Invoke("GiveGas", 1);
    }

    public void Update()
    {
        if (transform.position.x < -1.86f && transform.position.z < 1.98f)
        {
            carController.gasInput = 0;
            carController.brakeInput = 0.5f;
            Invoke("OpenCamera", 2);
        }
    }

    public void GiveGas()
    {
        carController.gasInput = 0.4f;
    }

    public void OpenCamera()
    {
        introCamera.SetActive(false);
        mainCamera.SetActive(true);
        gameCanvas.SetActive(true);
    }
}
