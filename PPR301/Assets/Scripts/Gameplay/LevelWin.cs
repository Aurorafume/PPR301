using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelWin : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            ScoreManager scoreManager = FindObjectOfType<ScoreManager>();
            if (scoreManager)
            {
                scoreManager.LevelWin();
                gameObject.SetActive(false);
            }
        }
    }
}
