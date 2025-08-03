// ==========================================================================
// Meowt of Tune - Pause Menu
// Owned and contributed to by: brookcoli (Itch.io) - Aurorafume (GitHub), donkzilla03 (Itch.io) - AliAK03 (GitHub), JaydenFielderTorrens (Itch.io) - Squib35 (GitHub), komorebimoriko (Itch.io), TheRealCrizz (GitHub), LordVGahn (GitHub)
// itch.io listing: https://brookcoli.itch.io/meowt-of-tune - GitHub Repository: https://github.com/Aurorafume/PPR301
// ==========================================================================
//
// WHAT DOES THIS DO:
// This script controls the in-game pause menu. It handles pausing and resuming
// the game, showing and hiding the relevant UI panels (pause menu, settings,
// and the main game HUD), and managing the actions for the menu's buttons,
// such as loading the main menu or quitting the game.
//
// Core functionalities include:
// - Toggling the pause state with the Escape key.
// - Pausing and resuming the game using 'Time.timeScale'.
// - Managing the visibility of the pause menu, a settings sub-menu, and the
//   in-game HUD.
// - Handling the cursor's lock state and visibility for gameplay vs. menu navigation.
// - Providing public methods for UI buttons to call.
//
// Dependencies:
// - Requires UI GameObjects ('pauseMenuUI', 'settingsMenuUI', 'noiseBar')
//   to be assigned in the Inspector.
// - UI buttons on the menu must be hooked up to the public methods in this script.
//
// ==========================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Manages the pause menu UI, game pausing, and menu navigation.
/// </summary>
public class PauseMenu : MonoBehaviour
{
    [Header("UI Panels")]
    [Tooltip("The parent GameObject for the main pause menu.")]
    public GameObject pauseMenuUI;
    [Tooltip("The parent GameObject for the settings sub-menu.")]
    public GameObject settingsMenuUI;
    [Tooltip("The in-game HUD element (e.g. the noise bar) to hide when paused.")]
    public GameObject noiseBar;

    // Tracks the current pause state of the game.
    private bool isPaused = false;

    /// <summary>
    /// Initialises the game to a non-paused state.
    /// </summary>
    void Start()
    {
        Resume();
    }

    /// <summary>
    /// Checks for the pause input key each frame.
    /// </summary>
    void Update()
    {
        // Listen for the Escape key press.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // If in the settings sub-menu, 'Escape' acts as a back button.
            if (settingsMenuUI.activeSelf)
            {
                CloseSettings();
            }
            // Otherwise, toggle the main pause menu.
            else
            {
                if (isPaused)
                    Resume();
                else
                    Pause();
            }
        }
    }

    /// <summary>
    /// Resumes the game from the paused state. Public for use with UI buttons.
    /// </summary>
    public void Resume()
    { 
        pauseMenuUI.SetActive(false);
        settingsMenuUI.SetActive(false);
        noiseBar.SetActive(true);
        Time.timeScale = 1f; // Resume game time.
        isPaused = false;
        SetGameplayCursorState(true);
    }

    /// <summary>
    /// Pauses the game and shows the main pause menu.
    /// </summary>
    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        settingsMenuUI.SetActive(false);
        noiseBar.SetActive(false);
        Time.timeScale = 0f; // Freeze game time.
        isPaused = true;
        SetGameplayCursorState(false);
    }

    /// <summary>
    /// The callback method for the 'Settings' button.
    /// </summary>
    public void OpenSettings()
    {
        settingsMenuUI.SetActive(true);
        pauseMenuUI.SetActive(false);
    }

    /// <summary>
    /// The callback method for the 'Back' button in the settings menu.
    /// </summary>
    public void CloseSettings()
    {
        settingsMenuUI.SetActive(false);
        pauseMenuUI.SetActive(true);
    }

    /// <summary>
    /// The callback method for the 'Main Menu' button.
    /// </summary>
    public void LoadMainMenu()
    {
        Time.timeScale = 1f; // Ensure time is resumed before loading a new scene.
        SceneManager.LoadScene("StartMenu");
    }

    /// <summary>
    /// The callback method for the 'Quit' button.
    /// </summary>
    public void QuitGame()
    {
        // This will close the application. It only works in a built game, not in the Unity Editor.
        Application.Quit();
    }

    /// <summary>
    /// A helper method to manage the cursor's lock state and visibility.
    /// </summary>
    /// <param name="isGameplay">True to lock and hide the cursor for gameplay; false to unlock and show it for menus.</param>
    private void SetGameplayCursorState(bool isGameplay)
    {
        if (isGameplay)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}