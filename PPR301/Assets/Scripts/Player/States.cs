using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class States : MonoBehaviour
{
    public bool gameOver = false;
    public GameObject player;
    public GameObject enemy;

    void Update()
    {
        if (gameOver)
        {
            SceneManager.LoadScene("GameOver");
            gameOver = false;
        }
    }
}
