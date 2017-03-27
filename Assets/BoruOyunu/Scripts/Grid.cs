using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Grid : MonoBehaviour
{
	public static int count = 1;
	public static bool countdowning = false;
    /*public static int minShadowNumber = 5;*/

    // For checking appropriate properties of the grid.
    public bool Placed { get; set; }
    public bool ShowShadow { get; set; }
    public bool InGame { get; set; }
    public float countdown;

    // Pipe type and the shadowy pipe of the grid for level.
    public int pipeType;
    private GameObject activePipe;

    void OnEnable()
    {
        // Get the correct pipe type of the grid for the current level and the minimum shadowy pipes.
        pipeType = GameManager.Instance.pipeTypeAtPosition(transform.position);
		countdown = 5;

        // Grid in current level have a pipe.
        if (pipeType >= 0)
        {
            InGame = true;
            // Current levels shadowy path.
            if (GameManager.Instance.ShadowValue == 0) // Everything
            {
                if (pipeType == 6)
                {
                    activePipe = transform.GetChild(0).gameObject;
                }
                else
                {
                    activePipe = transform.GetChild(pipeType).gameObject;
                }
                activePipe.SetActive(true);
                ShowShadow = true;
            }
            else if (GameManager.Instance.ShadowValue == 1) // 50%
            {
				HintShadow(5);

				if (Random.value < 0.65 && count < 9)
                {
                    if (pipeType == 6)
                    {
                        activePipe = transform.GetChild(0).gameObject;
                    }
                    else
                    {
                        activePipe = transform.GetChild(pipeType).gameObject;
                    }
                    activePipe.SetActive(true);
                    ShowShadow = true;

					count++;

                    /*// Decrement the shadow number.
                    minShadowNumber--;
                    Debug.Log(minShadowNumber);*/
                }
                else
                {
                    ShowShadow = false;
                }
            }
            else
            {
				ShowShadow = false;
                HintShadow(5);
            }
        }
    }

    public void HintShadow(int timeToWait)
    {
        if (pipeType == 6)
        {
            activePipe = transform.GetChild(0).gameObject;
        }
        else if (pipeType == 7)
        {
            activePipe = transform.GetChild(6).gameObject;
        }
        else
        {
            activePipe = transform.GetChild(pipeType).gameObject;
        }
        activePipe.SetActive(true);
        GameManager.Instance.game.transform.GetChild(3).gameObject.SetActive(true);
        countdown = timeToWait;
		countdowning = true;

		StartCoroutine(WaitForShadow(timeToWait));
    }

    IEnumerator WaitForShadow(int timeToWait)
    {
        GameManager.Instance.State = GameStateBoru.Paused;
        yield return new WaitForSeconds(timeToWait);
        GameManager.Instance.State = GameStateBoru.Idle;
        GameManager.Instance.game.transform.GetChild(3).gameObject.SetActive(false);
		if (!ShowShadow)
		{
			activePipe.SetActive(false);
		}
		countdowning = false;
    }

    void OnDisable()
    {
        if (activePipe != null)
        {
            activePipe.SetActive(false);
        }
    }

    void Update()
    {
        if (activePipe != null && activePipe.activeInHierarchy && Placed)
        {
            activePipe.SetActive(false);
        }

        if (countdown > 0)
        {
            GameManager.Instance.game.transform.GetChild(4).GetComponent<Text>().text =
                "Boru Yolunu İpucu Kapanana Kadar Dikkatli Bir Şekilde İnceleyin. \n Kalan Süre: " + Mathf.Ceil(countdown);
            countdown -= Time.deltaTime;
        }
    }
    
    // Put the correct pipe to the grid.
    void PutPipe()
    {
        Placed = true;
    }
}
