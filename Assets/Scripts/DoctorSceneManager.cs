using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DoctorSceneManager : MonoBehaviour {
    public GameObject chartsMenu, patientsMenu, gamesMenu;
    public GameObject RMPMenu;
    public GameObject pieChartPrefab, barChartPrefab;
    public GameObject patientName1, patientName2;

    public void selectPatient()
    {
        patientsMenu.SetActive(false);
        gamesMenu.SetActive(true);
        patientName1.GetComponent<Text>().text = patientsMenu.transform.GetChild(2).transform.GetChild(0).GetComponent<Text>().text;
        patientName2.GetComponent<Text>().text = patientsMenu.transform.GetChild(2).transform.GetChild(0).GetComponent<Text>().text;

    }
    public void selectGame()
    {
        gamesMenu.SetActive(false);
        chartsMenu.SetActive(true);
    }
    public void selectRMP()
    {
        gamesMenu.SetActive(false);
        RMPMenu.SetActive(true);
    }
    public void backToPatientsMenu()
    {
        gamesMenu.SetActive(false);
        patientsMenu.SetActive(true);
    }
    public void backToGamesMenu()
    {
        RMPMenu.SetActive(false);
        chartsMenu.SetActive(false);
        gamesMenu.SetActive(true);
    }
    public void changeSceneToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void openPieChart()
    {
        pieChartPrefab.SetActive(true);
        barChartPrefab.SetActive(false);
    }
    public void openBarChart()
    {
        pieChartPrefab.SetActive(false);
        barChartPrefab.SetActive(true);
    }
}
