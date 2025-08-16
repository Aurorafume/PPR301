using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class TunaScript : MonoBehaviour
{
    public GameObject victoryMenu;
    public GameObject noiseBar;


    // Start is called before the first frame update
    void Start()
    {
    
    }
    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            ScoreManager scoreManager = FindObjectOfType<ScoreManager>();
            if (scoreManager != null)
            {
                scoreManager.HandleGameComplete();
            }
        }
    }
}
