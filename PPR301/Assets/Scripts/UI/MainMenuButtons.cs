// ==========================================================================
// Meowt of Tune - Main Menu Buttons
// Owned and contributed to by: brookcoli (Itch.io) - Aurorafume (GitHub), donkzilla03 (Itch.io) - AliAK03 (GitHub), JaydenFielderTorrens (Itch.io) - Squib35 (GitHub), komorebimoriko (Itch.io), TheRealCrizz (GitHub), LordVGahn (GitHub)
// itch.io listing: https://brookcoli.itch.io/meowt-of-tune - GitHub Repository: https://github.com/Aurorafume/PPR301
// ==========================================================================
//
// WHAT DOES THIS DO:
// This script is a UI controller for the game's main menu. It manages showing
// and hiding different menu panels (e.g. main, instructions, settings) and
// contains the callback functions for all the menu's buttons, such as
// starting the game or quitting the application.
//
// Core functionalities include:
// - Managing the visibility of different UI panels.
// - Providing public methods for UI buttons to call.
// - Loading different game scenes.
// - Quitting the application.
// - Resetting static game data before starting a new game session.
// - Ensuring the mouse cursor is unlocked and visible for UI navigation.
//
// Dependencies:
// - Requires UI panels ('mainMenuPanel', 'instructionsPanel', 'settingsPanel')
//   to be assigned in the Inspector.
// - UI buttons in the scene must be hooked up to the public methods in this script.
// - Relies on the static 'NoiseHandler.ResetStatics()' method.
//
// ==========================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the UI panels and button functionality for the main menu.
/// </summary>
public class MainMenuButtons : MonoBehaviour
{
    [Header("UI Panels")]
    [Tooltip("The parent GameObject for the main menu panel.")]
    public GameObject mainMenuPanel;
    [Tooltip("The parent GameObject for the instructions panel.")]
    public GameObject instructionsPanel;
    [Tooltip("The parent GameObject for the settings panel.")]
    public GameObject settingsPanel;

    /// <summary>
    /// Sets up the initial menu state and cursor visibility.
    /// </summary>
    void Start()
    {
        // Unlock the cursor and make it visible for menu navigation.
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Ensure only the main menu panel is visible at the start.
        mainMenuPanel.SetActive(true);
        instructionsPanel.SetActive(false);
        settingsPanel.SetActive(false);
    }

    /// <summary>
    /// The callback method for the 'Play' button. Shows the instructions panel.
    /// </summary>
    public void OnPlayPressed()
    {
        NoiseHandler.ResetStatics();
        mainMenuPanel.SetActive(false);
        instructionsPanel.SetActive(true);
    }

    /// <summary>
    /// The callback method for a 'Continue' or 'Start' button on the instructions panel.
    /// </summary>
    public void OnMainSceneLoaded()
    { 
        NoiseHandler.ResetStatics();
        SceneManager.LoadScene("MainDemoA3");
    }

    /// <summary>
    /// The callback method for the 'Play Demo 1' button.
    /// </summary>
    public void OnPlayDemo1Pressed()
    {
        SceneManager.LoadScene("Main");
    }

    /// <summary>
    /// The callback method for the 'Play Demo 2' button.
    /// </summary>
    public void OnPlayDemo2Pressed()
    {
        SceneManager.LoadScene("Demo_2");
    }

    /// <summary>
    /// The callback method for the 'Settings' button.
    /// </summary>
    public void OnSettingsPressed()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    /// <summary>
    /// The callback method for any 'Back' buttons in sub-menus.
    /// </summary>
    public void OnBackPressed()
    {
        settingsPanel.SetActive(false);
        instructionsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    /// <summary>
    /// The callback method for the 'Leave' or 'Quit' button.
    /// </summary>
    public void OnLeavePressed()
    {
        // This will close the application. It only works in a built game, not in the Unity Editor.
        Application.Quit();
    }
}