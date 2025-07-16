using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Buttons : MonoBehaviour
{
    public States states;

    public void Play()
    {
        SceneManager.LoadScene("MainDemoA3");
    }
    public void message()
    {
        Debug.Log("hello");
    }
    public void PlayDemo()
    {   
        // Reset the game state
        states.ResetGameState();
        // Load the demo scene
        SceneManager.LoadScene("MainDemoA3");
    }
    public void PlayDemo2()
    {
        // Load the demo scene
        SceneManager.LoadScene("Demo_2");
    }
    public void ChangeMusic()
    {
        Debug.Log("Music changed!!");
    }
}
