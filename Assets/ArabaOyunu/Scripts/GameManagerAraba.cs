using UnityEngine;
using System.Collections;

public class GameManagerAraba : MonoBehaviour
{
    /* Different mask cameras for erasing the texture */
    public GameObject[] maskCameras;

    /* Singleton instance */
    public static GameManagerAraba Instance;

    /* Current mask camera and the index at maskCameras array */
    private GameObject currentMaskCamera;
    private int currentMaskCameraIndex;

    void Awake()
    {
        /* Provide Singleton functionality */
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        /* Initialize the mask camera properties */
        currentMaskCameraIndex = 0;
        currentMaskCamera = maskCameras[currentMaskCameraIndex];
    }

    void Update()
    {

    }

    /// <summary>
    /// Change the current mask camera to the next one in the scene.
    /// </summary>
    public void ChangeToNextMaskCamera()
    {
        /* Deactivate the current one */
        currentMaskCamera.SetActive(false);

        /* Increment the index of the mask camera */
        currentMaskCameraIndex++;

        /* If there are mask cameras left get the next camera */
        if (currentMaskCameraIndex < maskCameras.Length)
        {
            currentMaskCamera = maskCameras[currentMaskCameraIndex];
            currentMaskCamera.SetActive(true);
        }
    }
}
