using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    public GameObject gameOverMenu;

    CheckpointManager checkpointManager;
    GameObject player;

    void Awake()
    {
        checkpointManager = CheckpointManager.Instance;
        player = FindObjectOfType<PlayerMovement>().gameObject;
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
                CloseGameOverMenu();
                checkpointManager.SendPlayerToLastCheckpoint();
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
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f; // Resume the game time
        // Load the main menu scene
        SceneManager.LoadScene("StartMenu");
    }
}
