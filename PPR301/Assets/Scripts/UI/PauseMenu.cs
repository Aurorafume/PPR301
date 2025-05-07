using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public GameObject settingsMenuUI;
    public GameObject noiseBar;
    private bool isPaused = false;

    void Start()
    {
        // Hide pause menu and settings menu
        pauseMenuUI.SetActive(false);
        settingsMenuUI.SetActive(false);

        // Ensure Noise Bar is visible at the start
        noiseBar.SetActive(true);

        Time.timeScale = 1f;
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Pause game when escape key is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (settingsMenuUI.activeSelf)
            {
                CloseSettings();
            }
            else
            {
                if (isPaused)
                    Resume();
                else
                    Pause();
            }
        }
    }

    public void Resume()
    {   
        pauseMenuUI.SetActive(false);
        settingsMenuUI.SetActive(false);
        // Show the Noise Bar again
        noiseBar.SetActive(true);

        Time.timeScale = 1f;
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        settingsMenuUI.SetActive(false);
        // Hide the Noise Bar when paused
        noiseBar.SetActive(false);

        Time.timeScale = 0f;
        isPaused = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void OpenSettings()
    {
        // Show settings menu
        settingsMenuUI.SetActive(true);
        pauseMenuUI.SetActive(false);
        // Also hide the Noise Bar when in settings
        noiseBar.SetActive(false);
    }

    public void CloseSettings()
    {
        // Hide settings menu and show pause menu again
        settingsMenuUI.SetActive(false);
        pauseMenuUI.SetActive(true);
        // Keep Noise Bar hidden if we’re still in the pause menu
        noiseBar.SetActive(false);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("StartMenu");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}
