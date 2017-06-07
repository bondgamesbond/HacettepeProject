using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManagerAraba : MonoBehaviour
{
    public string[] feedbackTexts;
    public string[] wrongFeedbackTexts;
    /* Different mask cameras for erasing the texture */
    public GameObject[] maskCameras;

    /* Singleton instance */
    public static GameManagerAraba Instance;

    /* Current mask camera and the index at maskCameras array */
    private GameObject currentMaskCamera;
    private int currentMaskCameraIndex;

    /* Level information of the game */
    public int level;

	/* Feedback text */
	public Text feedbackText;

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
        currentMaskCameraIndex = 1;
        currentMaskCamera = maskCameras[currentMaskCameraIndex - 1];
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

        /* Increase the level */
        level++;

        /* If there are mask cameras left get the next camera */
        if (currentMaskCameraIndex < maskCameras.Length)
        {
            currentMaskCamera.GetComponent<MaskCamera>().objectToErase.SetActive(false);
            currentMaskCamera = maskCameras[currentMaskCameraIndex - 1];
            currentMaskCamera.SetActive(true);

            switch (level)
            {
                case 0:
                    feedbackText.text = feedbackTexts[0];
                    break;
                case 1:
                    feedbackText.text = feedbackTexts[1];
                    break;
                case 2:
                    feedbackText.text = feedbackTexts[2];
                    break;
                case 3:
                    //feedbackText.text = "Lütfen istediðiniz bir hareketle aracý kurulayýnýz!";
                    feedbackText.text = feedbackTexts[3];
                    break;
                default:
                    feedbackText.text = "Lütfen istediðiniz bir hareketle araca cila atýnýz!";
                    break;
            }
        }
        else
        {
            feedbackText.text = "Tebrikler!";
        }
    }

    public void returnToMainMenu()
    {
        PlayerPrefs.SetInt("ReturningFromGame", 1);
        SceneManager.LoadSceneAsync("MainMenu");
    }
}
