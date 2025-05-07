using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject mainMenuPanel;
    public GameObject instructionsPanel;
    public GameObject settingsPanel;

    void Start()
    {
        mainMenuPanel.SetActive(true);
        instructionsPanel.SetActive(false);
        settingsPanel.SetActive(false);
    }

    public void OnPlayPressed()
    {
        mainMenuPanel.SetActive(false);
        instructionsPanel.SetActive(true);
    }

    public void OnMainSceneLoaded()
    {   
        NoiseHandler.ResetStatics();
        Debug.Log("Main scene loaded!");
        SceneManager.LoadScene("MainDemoA3");
    }

    public void OnPlayDemo1Pressed()
    {
        Debug.Log("Play Demo 1 started!");
        SceneManager.LoadScene("Main");
    }

    public void OnPlayDemo2Pressed()
    {
        Debug.Log("Play Demo 2 started!");
        SceneManager.LoadScene("Demo_2");
    }

    public void OnSettingsPressed()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void OnBackPressed()
    {
        settingsPanel.SetActive(false);
        instructionsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void OnLeavePressed()
    {
        Debug.Log("Exiting the game...");
        Application.Quit();
    }
}
