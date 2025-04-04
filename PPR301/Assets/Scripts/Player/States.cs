using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class States : MonoBehaviour
{
    public bool gameOver = false;
    public bool playerIsHiding = false;
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

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hiding Spot"))
        {
            playerIsHiding = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Hiding Spot"))
        {
            playerIsHiding = false;
        }
    }
}
