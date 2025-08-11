using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryMenu : MonoBehaviour
{
    public void Restart()
    {
        SceneManager.LoadScene("MainDemoA3");
    }
    public void Menu()
    {
        SceneManager.LoadScene("StartMenu");
    }
}
