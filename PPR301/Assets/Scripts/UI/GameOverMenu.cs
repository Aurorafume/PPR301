using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    public GameObject gameOverMenu;

    CheckpointManager checkpointManager;
    GameObject player;
    PlayerMovement playerMovement;
    NoiseHandler noiseHandler;

    void Awake()
    {
        checkpointManager = CheckpointManager.Instance;
        playerMovement = FindObjectOfType<PlayerMovement>();
        player = playerMovement.gameObject;
        noiseHandler = FindObjectOfType<NoiseHandler>();
    }

    void Start()
    {
        gameOverMenu.SetActive(false);
    }

    public void ShowGameOverMenu()
    {
        checkpointManager.SendPlayerToLastCheckpoint();
        playerMovement.playerCollider.enabled = false; // Disable player collider to prevent further movement

        // Ensure camera resets to a normal state if in a top-down area
        Cameras cameras = FindObjectOfType<Cameras>();
        if (cameras != null)
        {
            cameras.ForceNoTopDownCamera(); // Ensure the camera is not in top-down mode
        }
        gameOverMenu.SetActive(true);
        Time.timeScale = 0f; // Pause the game
        Cursor.lockState = CursorLockMode.None; // Unlock the cursor
        Cursor.visible = true; // Make the cursor visible
        noiseHandler.SetZeroAdditionalNoise(); // Reset additional noise to zero
    }

    public void RestartGame()
    {
        if (checkpointManager.HasCheckpoint())
        {
            if (player != null)
            {
                CloseGameOverMenu();
            }
        }
        else
        {
            // Load the main game scene
            SceneManager.LoadScene("MainDemoA3");
        }
        
        if (playerMovement != null)
        {
            playerMovement.playerCollider.enabled = true;
        }
    }

    void CloseGameOverMenu()
    {
        gameOverMenu.SetActive(false);
        Time.timeScale = 1f; // Resume the game time
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor
        Cursor.visible = false; // Hide the cursor
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f; // Resume the game time
        // Load the main menu scene
        SceneManager.LoadScene("StartMenu");
    }
}
