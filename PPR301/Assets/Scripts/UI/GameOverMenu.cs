// ==========================================================================
// Meowt of Tune - Game Over Menu
// Owned and contributed to by: brookcoli (Itch.io) - Aurorafume (GitHub), donkzilla03 (Itch.io) - AliAK03 (GitHub), JaydenFielderTorrens (Itch.io) - Squib35 (GitHub), komorebimoriko (Itch.io), TheRealCrizz (GitHub), LordVGahn (GitHub)
// itch.io listing: https://brookcoli.itch.io/meowt-of-tune - GitHub Repository: https://github.com/Aurorafume/PPR301
// ==========================================================================
//
// WHAT DOES THIS DO:
// This script controls the UI and functionality of the Game Over screen. It is
// responsible for pausing the game, showing the menu panel, and handling the
// player's choices, such as restarting from the last checkpoint or returning
// to the main menu.
//
// Core functionalities include:
// - A public method to activate the game over state and show the menu.
// - Pausing and resuming the game by manipulating 'Time.timeScale'.
// - Unlocking and re-locking the mouse cursor for UI interaction.
// - Interacting with the 'CheckpointManager' to move the player back to their
//   last saved checkpoint.
// - Providing public methods for UI buttons to restart the level or load the
//   main menu scene.
//
// Dependencies:
// - Requires a 'gameOverMenu' GameObject to be assigned in the Inspector.
// - Interacts with other managers in the scene: 'CheckpointManager', 'States',
//   and 'NoiseHandler'.
// - The UI buttons on the menu must be hooked up to the 'RestartGame()' and
//   'ReturnToMainMenu()' public methods.
//
// ==========================================================================

using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the game over UI, pausing the game and handling restart/quit options.
/// </summary>
public class GameOverMenu : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("The parent GameObject for the game over menu UI panel.")]
    public GameObject gameOverMenu;

    [Header("Music")]
    [Tooltip("Time taken for the music to fully fade back in after being caught")]
    public float musicFadeInTime = 2.5f;
    private float volumeFadeSpeed;

    // --- Cached Component References ---
    private CheckpointManager checkpointManager;
    private GameObject playerObject;
    private States states;
    private NoiseHandler noiseHandler;
    private Buttons buttons;
    private ScoreManager scoreManager;

    /// <summary>
    /// Caches references to other manager components in the scene.
    /// </summary>
    void Awake()
    {
        checkpointManager = CheckpointManager.Instance;
        states = FindObjectOfType<States>();
        noiseHandler = FindObjectOfType<NoiseHandler>();
        playerObject = states.gameObject;
        buttons = FindObjectOfType<Buttons>();
        scoreManager = FindObjectOfType<ScoreManager>();
    }

    /// <summary>
    /// Ensures the game over menu is hidden at the start of the game.
    /// </summary>
    void Start()
    {
        gameOverMenu.SetActive(false);
        volumeFadeSpeed = 1f / musicFadeInTime;
    }

    /// <summary>
    /// Activates the game over sequence, showing the menu and pausing the game.
    /// </summary>
    public void ShowGameOverMenu()
    {
        gameOverMenu.SetActive(true);
        Time.timeScale = 0f; // Pause all physics and time-based operations.

        // Immediately move the player to the last checkpoint while the game is paused.
        checkpointManager.SendPlayerToLastCheckpoint();

        // Ensure camera resets to a normal state if in a top-down area
        Cameras cameras = FindObjectOfType<Cameras>();
        if (cameras != null)
        {
            cameras.ForceNoTopDownCamera(); // Ensure the camera is not in top-down mode
        }

        // Unlock and show the cursor to allow UI interaction.
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Reset any active noise effects.
        if (noiseHandler != null)
        {
            noiseHandler.ClearNoise();
        }

        if (scoreManager != null)
        {
            scoreManager.AddDeathCount(); // Increment death count when game over occurs.
        }
    }

    /// <summary>
    /// Handles the logic for the 'Restart' or 'Continue' button.
    /// </summary>
    public void ResumeFromLastCheckpoint()
    {
        // If the player has reached a checkpoint, resume the game from that point.
        if (checkpointManager.HasCheckpoint())
        {
            if (playerObject != null)
            {
                CloseGameOverMenu();
            }
        }
        // If no checkpoint has been saved, reload the entire level.
        else
        {
            Time.timeScale = 1f; // Ensure time is resumed before loading a new scene.
            buttons.FadeMusic(1f, volumeFadeSpeed);
            SceneManager.LoadScene("MainDemoA3");
        }
    }

    /// <summary>
    /// Hides the game over menu and resumes the game.
    /// </summary>
    void CloseGameOverMenu()
    {
        // Reset the game state to clean up enemies and flags.
        if (states != null)
        {
            states.ResetGameState();
        }

        gameOverMenu.SetActive(false);
        Time.timeScale = 1f; // Resume game time.
        buttons.FadeMusic(1f, volumeFadeSpeed);
        
        // Re-lock and hide the cursor for gameplay.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    /// <summary>
    /// Handles the logic for the 'Return to Main Menu' button.
    /// </summary>
    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f; // Ensure time is resumed before loading a new scene.
        buttons.FadeMusic(1f, volumeFadeSpeed);
        SceneManager.LoadScene("StartMenu");
    }
}