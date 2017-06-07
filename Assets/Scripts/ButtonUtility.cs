using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class ButtonUtility : MonoBehaviour {

    public InputField username, password;
    public GameObject gameList, signInMenu, signInMenu_back;
    public GameObject errorPanel;
    private bool firstTime = true;
    VirtualKeyboard vk = new VirtualKeyboard();


    // Use this for initialization
    void Start ()
    {
        if (PlayerPrefs.GetInt("ReturningFromGame") == 1)
        {
            gameList.SetActive(true);
            signInMenu.SetActive(false);
            signInMenu_back.SetActive(false);
        }

        PlayerPrefs.SetInt("ReturningFromGame", 0);
	}

    public void signIn()
    {
        if (username.text == "hasta" && password.text == "hasta")
        {
            print("Hasta Girişi");
            CloseKeyboard();
            gameList.SetActive(true);
            signInMenu.SetActive(false);
            signInMenu_back.SetActive(false);
        }
        else if (username.text == "doktor" && password.text == "doktor")
        {
            print("Doktor Girişi");
            CloseKeyboard();
            SceneManager.LoadScene("Doktor");
           
        }
        else
        {
            print("Hatalı şifre ya da kullanıcı adı");
            errorPanel.SetActive(true);
        }
    }
    // Update is called once per frame

   

	void Update () {
        if (firstTime)
        {
            if (username.isFocused || password.isFocused)
            {
                OpenKeyboard();
                firstTime = false;
            }
            else
            {
                CloseKeyboard();
                firstTime = true;
            }
        }

	}
   

    public void OpenKeyboard()
    {
        {
            vk.ShowTouchKeyboard();
        }
    }

    public void CloseKeyboard()
    {
        {
            vk.HideTouchKeyboard();
        }
    }
}
