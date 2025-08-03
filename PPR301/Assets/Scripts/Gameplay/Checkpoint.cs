// ==========================================================================
// Meowt of Tune - Checkpoint
// Owned and contributed to by: brookcoli (Itch.io) - Aurorafume (GitHub), donkzilla03 (Itch.io) - AliAK03 (GitHub), JaydenFielderTorrens (Itch.io) - Squib35 (GitHub), komorebimoriko (Itch.io), TheRealCrizz (GitHub), LordVGahn (GitHub)
// itch.io listing: https://brookcoli.itch.io/meowt-of-tune - GitHub Repository: https://github.com/Aurorafume/PPR301
// ==========================================================================
//
// WHAT DOES THIS DO:
// This script is attached to checkpoint GameObjects within the game world. Its
// sole purpose is to provide a function that, when called, registers its
// location with the CheckpointManager as the player's new respawn point.
//
// Core functionalities include:
// - A public method to activate the checkpoint.
// - Capturing the player's rotation at the moment of activation.
// - Communicating with the central CheckpointManager to save the data.
//
// Dependencies:
// - A singleton instance of CheckpointManager must exist in the scene.
// - A GameObject with the PlayerMovement script must be present.
// - Another script (e.g. a trigger volume) is required to call ActivateCheckpoint().
//
// ==========================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a single checkpoint in the game. When activated, it saves its position
/// and the player's current rotation to the CheckpointManager.
/// </summary>
public class Checkpoint : MonoBehaviour
{
    /// <summary>
    /// Activates the checkpoint, saving its position and the player's current rotation
    /// as the new respawn point.
    /// </summary>
    public void ActivateCheckpoint()
    {
        // Get the player's current rotation to ensure they face the correct direction on respawn.
        Quaternion playerRotation = FindObjectOfType<PlayerMovement>().transform.rotation;

        // Pass this checkpoint's position and the player's rotation to the manager.
        CheckpointManager.Instance.SetCheckpoint(transform.position, playerRotation);
    }
}