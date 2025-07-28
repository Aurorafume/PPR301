using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    public GameObject gameOverMenu;

    CheckpointManager checkpointManager;
    GameObject player;
    States states;

    NoiseBar noiseBar;

    void Awake()
    {
        checkpointManager = CheckpointManager.Instance;
        states = FindObjectOfType<States>();
        noiseBar = FindObjectOfType<NoiseBar>();
        player = states.gameObject;
    }

    void Start()
    {
        gameOverMenu.SetActive(false);
    }

    public void ShowGameOverMenu()
    {
        gameOverMenu.SetActive(true);
        Time.timeScale = 0f; // Pause the game
        Cursor.lockState = CursorLockMode.None; // Unlock the cursor
        Cursor.visible = true; // Make the cursor visible
    }

    public void RestartGame()
    {

        if (checkpointManager.HasCheckpoint())
        {
            if (player != null)
            {
                checkpointManager.SendPlayerToLastCheckpoint();
                CloseGameOverMenu();
            }
        }
        else
        {
            // Load the main game scene
            SceneManager.LoadScene("MainDemoA3");
        }      
    }

    void CloseGameOverMenu()
    {
        gameOverMenu.SetActive(false);
        Time.timeScale = 1f; // Resume the game time
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor
        Cursor.visible = false; // Hide the cursor
        states.gameOver = false; // Reset game over state
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f; // Resume the game time
        // Load the main menu scene
        SceneManager.LoadScene("StartMenu");
    }
}
