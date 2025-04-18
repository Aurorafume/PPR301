using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    public States states;
    public void Start()
    {
        // Have the mouse unlocked when the game starts
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void RestartGame()
    {
        // Load the main game scene
        SceneManager.LoadScene("Main");
    }

    public void ReturnToMainMenu()
    {
        // Load the main menu scene
        SceneManager.LoadScene("StartMenu");
    }

    public void RestartDemo()
    {   
        states.ResetGameState();
        // Load the demo scene
        SceneManager.LoadScene("MainDemoA2");
    }
}
