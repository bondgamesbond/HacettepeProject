using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    
	void Start () {
	
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
}
