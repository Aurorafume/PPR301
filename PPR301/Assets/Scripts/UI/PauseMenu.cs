using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public GameObject settingsMenuUI;
    private bool isPaused = false;

    void Start()
    {   
        // Hide pause menu and settings menu
        pauseMenuUI.SetActive(false);
        settingsMenuUI.SetActive(false);
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
                settingsMenuUI.SetActive(false);
                pauseMenuUI.SetActive(true);
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
        // Hide pause menu and settings menu
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Pause()
    {   
        // Show pause menu
        pauseMenuUI.SetActive(true);
        settingsMenuUI.SetActive(false);
        Time.timeScale = 0f;
        isPaused = true;

        Cursor.lockState = CursorLockMode.None;
    }

    public void LoadMainMenu()
    {   
        // Load the main menu
        Time.timeScale = 1f;
        SceneManager.LoadScene("StartMenu");
    }

    public void QuitGame()
    {   
        // Quit the game
        Debug.Log("Quitting game...");
        Application.Quit();
    }

    public void OpenSettings()
    {   
        // Show settings menu
        settingsMenuUI.SetActive(true);
        pauseMenuUI.SetActive(false);
    }

    public void CloseSettings()
    {   
        // Hide settings menu
        settingsMenuUI.SetActive(false);
        pauseMenuUI.SetActive(true);
    }
}