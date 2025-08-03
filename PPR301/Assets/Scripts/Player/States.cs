// ==========================================================================
// Meowt of Tune - States
// Owned and contributed to by: brookcoli (Itch.io) - Aurorafume (GitHub), donkzilla03 (Itch.io) - AliAK03 (GitHub), JaydenFielderTorrens (Itch.io) - Squib35 (GitHub), komorebimoriko (Itch.io), TheRealCrizz (GitHub), LordVGahn (GitHub)
// itch.io listing: https://brookcoli.itch.io/meowt-of-tune - GitHub Repository: https://github.com/Aurorafume/PPR301
// ==========================================================================
//
// WHAT DOES THIS DO:
// This script acts as a central game state manager. It holds several key
// boolean flags that other scripts can reference to understand the current
// state of the game and the player (e.g. is the game over? is the player
// hiding?). It provides a single source of truth for these states and includes
// a utility to reset the game to a clean initial state.
//
// Core functionalities include:
// - Tracking crucial player states like 'gameOver', 'playerIsHiding', etc.
// - Detecting when the player enters or leaves trigger zones for hiding spots.
// - Checking if the player is on a platform using both triggers and raycasts.
// - A public method to reset the game, destroying leftover enemies and resetting flags.
// - Handling the 'Game Over' sequence by triggering the game over menu.
//
// Dependencies:
// - Requires various scene objects to be tagged correctly ("Hiding Spot", "Player", "Enemy").
// - Interacts with other scripts: 'GameOverMenu', 'ShootProjectile', 'NoiseBar',
//   and 'EnemySpawning'.
// - The 'platformLayer' must be configured correctly in the Inspector.
//
// ==========================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// A central manager for holding and updating critical game state flags.
/// </summary>
public class States : MonoBehaviour
{
    [Header("Player State Flags")]
    [Tooltip("Indicates whether the game is currently over.")]
    public bool gameOver = false;
    [Tooltip("True if the player is currently inside a hiding spot.")]
    public bool playerIsHiding = false;
    [Tooltip("True if the player is currently standing on a valid platform.")]
    public bool playerIsOnPlatform = false;
    [Tooltip("True if the player is in the Memory Room mini-game.")]
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

    /// <summary>
    /// Initialises the game to a clean state on startup.
    /// </summary>
    void Start()
    {
        ResetGameState();
    }

    /// <summary>
    /// Checks for game over conditions and runs continuous state checks each frame.
    /// </summary>
    void Update()
    {
        // If the gameOver flag has been set by another script, handle the game over sequence.
        if (gameOver)
        {
            // Show the game over menu UI.
            FindObjectOfType<GameOverMenu>().ShowGameOverMenu();
            // Clean up any null entries from the static trumpet list.
            ShootProjectile.trumpetList.RemoveAll(obj => obj == null);
            // Reset the flag immediately to prevent this from running every frame.
            gameOver = false;
        }

        // Continuously check if the player is standing on a platform via raycast.
        CheckIfPlayerIsOnPlatform();
    }

    /// <summary>
    /// Updates player state flags when entering a trigger volume.
    /// </summary>
    /// <param name="other">The collider that was entered.</param>
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

    /// <summary>
    /// Updates player state flags when exiting a trigger volume.
    /// </summary>
    /// <param name="other">The collider that was exited.</param>
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

    /// <summary>
    /// A downward raycast check to determine if the player is on a platform.
    /// </summary>
    void CheckIfPlayerIsOnPlatform()
    {
        if (Physics.Raycast(player.transform.position, Vector3.down, 1f, platformLayer))
        {
            playerIsOnPlatform = true;
        }
        else
        {
            playerIsOnPlatform = false;
        }
    }

    /// <summary>
    /// A utility method to check if a GameObject belongs to a specific LayerMask.
    /// </summary>
    /// <param name="obj">The GameObject to check.</param>
    /// <param name="layerMask">The LayerMask to check against.</param>
    /// <returns>True if the object's layer is in the mask.</returns>
    bool IsInLayerMask(GameObject obj, LayerMask layerMask)
    {
        return ((layerMask.value & (1 << obj.layer)) > 0);
    }

    /// <summary>
    /// Resets all relevant game state variables and cleans up the scene for a new session.
    /// </summary>
    public void ResetGameState()
    {
        // Reset boolean state flags.
        gameOver = false;
        playerIsHiding = false;
        playerIsOnPlatform = false;

        // Reset the static flag that tracks if an enemy is currently active.
        NoiseHandler.enemyExists = false;

        // Find and destroy any enemy from a previous session to prevent duplicates.
        GameObject existingEnemy = GameObject.FindWithTag("Enemy");
        if (existingEnemy != null)
        {
            Destroy(existingEnemy);
        }

        // Find and reset the noise bar UI visuals.
        NoiseBar noiseBar = FindObjectOfType<NoiseBar>();
        if (noiseBar != null)
        {
            noiseBar.ForceChaseVisuals(false);
        }
    }
}