using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class States : MonoBehaviour
{
    public bool gameOver = false;
    public bool playerIsHiding = false;
    public bool playerIsOnPlatform = false;
    public GameObject player;
    public GameObject enemy;
    public EnemySpawning enemySpawning;
    public LayerMask platformLayer;

    void Update()
    {
        if (gameOver)
        {
            SceneManager.LoadScene("GameOver");
            gameOver = false;
        }

        CheckIfPlayerIsOnPlatform();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hiding Spot"))
        {
            playerIsHiding = true;
        }

        if (IsInLayerMask(other.gameObject, platformLayer))
        {
            playerIsOnPlatform = true;
        }
    }

    void CheckIfPlayerIsOnPlatform()
    {
        RaycastHit hit;
        float checkDistance = 1f;

        if (Physics.Raycast(player.transform.position, Vector3.down, out hit, checkDistance, platformLayer))
        {
            playerIsOnPlatform = true;
        }
        else
        {
            playerIsOnPlatform = false;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Hiding Spot"))
        {
            playerIsHiding = false;
        }

        if (IsInLayerMask(other.gameObject, platformLayer))
        {
            playerIsOnPlatform = false;
        }
    }

    // Utility method to check if an object is in a specific LayerMask
    bool IsInLayerMask(GameObject obj, LayerMask layerMask)
    {
        return ((layerMask.value & (1 << obj.layer)) > 0);
    }
}
