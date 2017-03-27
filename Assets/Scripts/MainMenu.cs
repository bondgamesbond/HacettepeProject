using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject sliderRight, sliderLeft;
    public RectTransform gameList;
    int slideId, gameCount;
	void Start ()
    {
        slideId = 0;
        gameCount = gameList.childCount;
	}
	
	void Update () {
	
	}

    public void loadRestrictGame()
    {
        SceneManager.LoadSceneAsync("RestrictionMapScene");
    }

    public void loadGame(string gameName)
    {
        SceneManager.LoadSceneAsync(gameName);
    }

    public void closeApp()
    {
        Application.Quit();
    }


    Vector3 tempPos;
    public void slideGamesToLeft()
    {
        slideId--;
        tempPos = gameList.localPosition;
        tempPos.x += 586;
        gameList.GetChild(slideId).gameObject.SetActive(true);
        gameList.GetChild(slideId + 3).gameObject.SetActive(false);
        gameList.localPosition = tempPos;
        sliderRight.SetActive(true);
        if (slideId == 0)
            sliderLeft.SetActive(false);
    }
    public void slideGamesToRight()
    {
        slideId++;
        tempPos = gameList.localPosition;
        tempPos.x -= 586;
        gameList.GetChild(slideId - 1).gameObject.SetActive(false);
        gameList.GetChild(slideId + 2).gameObject.SetActive(true);
        gameList.localPosition = tempPos;
        sliderLeft.SetActive(true);
        if (slideId + 3 == gameCount)
            sliderRight.SetActive(false);
    }
}
