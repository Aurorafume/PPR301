// ==========================================================================
// Meowt of Tune - Level Win
// Owned and contributed to by: brookcoli (Itch.io) - Aurorafume (GitHub), donkzilla03 (Itch.io) - AliAK03 (GitHub), JaydenFielderTorrens (Itch.io) - Squib35 (GitHub), komorebimoriko (Itch.io), TheRealCrizz (GitHub), LordVGahn (GitHub)
// itch.io listing: https://brookcoli.itch.io/meowt-of-tune - GitHub Repository: https://github.com/Aurorafume/PPR301
// ==========================================================================
//
// WHAT DOES THIS DO:
// This script creates a trigger volume that ends the level when the player
// character enters it. It communicates with a ScoreManager to handle the
// actual level completion logic, such as displaying a win screen.
//
// Core functionalities include:
// - Detecting when an object tagged "Player" enters its trigger.
// - Calling the LevelWin method on a ScoreManager.
// - Deactivating itself to ensure the win condition is only triggered once.
//
// Dependencies:
// - UnityEngine for core and physics functionality.
// - A Collider component on this GameObject, with "Is Trigger" enabled.
// - A ScoreManager script must exist in the scene.
// - The player's GameObject must be tagged "Player".
//
// ==========================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A simple trigger that signals the ScoreManager to end the level when the player enters.
/// </summary>
public class LevelWin : MonoBehaviour
{
    /// <summary>
    /// Called by Unity when a collider enters the trigger volume.
    /// </summary>
    /// <param name="other">The collider that entered the trigger.</param>
    void OnTriggerEnter(Collider other)
    {
        // Check if the entering object is the player.
        if (other.CompareTag("Player"))
        {
            // Find the ScoreManager in the scene.
            ScoreManager scoreManager = FindObjectOfType<ScoreManager>();
            
            // If the ScoreManager is found, trigger the level win sequence.
            if (scoreManager != null)
            {
                scoreManager.LevelWin();
                
                // Deactivate this trigger to prevent it from firing again.
                gameObject.SetActive(false);
            }
            else
            {
                Debug.LogError("LevelWin trigger cannot find ScoreManager in the scene.", this);
            }
        }
    }
}