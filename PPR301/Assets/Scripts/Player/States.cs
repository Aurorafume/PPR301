using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class States : MonoBehaviour
{
    [Header("Player State Flags")]
    [Tooltip("Indicates whether the game is currently over.")]
    public bool gameOver = false;

    [Tooltip("True if the player is currently hiding in a hiding spot.")]
    public bool playerIsHiding = false;

    [Tooltip("True if the player is currently standing on a valid platform.")]
    public bool playerIsOnPlatform = false;

    [Tooltip("True if the player is in the Memory Room Game")]
    public bool InMemoryRoomGame = false;

    [Header("Scene References")]
    [Tooltip("Reference to the player GameObject.")]
    public GameObject player;

    [Tooltip("Reference to the current enemy GameObject.")]
    public GameObject enemy;

    [Tooltip("Reference to the EnemySpawning manager.")]
    public EnemySpawning enemySpawning;

    [Header("Environment Detection")]
    [Tooltip("Layer mask used to identify platforms the player can stand on.")]
    public LayerMask platformLayer;

    void start()
    {
        ResetGameState();
    }

    void Update()
    {   
        // Check if the player is in a hiding spot and set the game over state accordingly
        if (gameOver)
        {
            FindObjectOfType<GameOverMenu>().ShowGameOverMenu();
            
            ShootProjectile.trumpetList.RemoveAll(obj => obj == null);
        }

        CheckIfPlayerIsOnPlatform();
    }

    void OnTriggerEnter(Collider other)
    {   
        // Check if the player is in a hiding spot or on a platform
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
        // Check if the player is on a platform using a raycast
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
        // Reset the player state when exiting a hiding spot or platform
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

    public void ResetGameState()
    {
        // Reset basic flags
        gameOver = false;
        playerIsHiding = false;
        playerIsOnPlatform = false;

        // Reset static enemy spawn flag
        NoiseHandler.enemyExists = false;

        // Destroy any lingering enemy from a previous session
        GameObject existingEnemy = GameObject.FindWithTag("Enemy");
        if (existingEnemy != null)
        {
            Destroy(existingEnemy);
            Debug.Log("ResetGameState: Existing enemy destroyed.");
        }

        // Reset noise bar visuals
        NoiseBar noiseBar = FindObjectOfType<NoiseBar>();
        if (noiseBar != null)
        {
            noiseBar.ForceChaseVisuals(false);
            Debug.Log("ResetGameState: Noise visuals reset.");
        }

        Debug.Log("ResetGameState: Game state reset complete.");
    }
}
