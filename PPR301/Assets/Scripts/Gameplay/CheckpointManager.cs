// ==========================================================================
// Meowt of Tune - Checkpoint Manager
// Owned and contributed to by: brookcoli (Itch.io) - Aurorafume (GitHub), donkzilla03 (Itch.io) - AliAK03 (GitHub), JaydenFielderTorrens (Itch.io) - Squib35 (GitHub), komorebimoriko (Itch.io), TheRealCrizz (GitHub), LordVGahn (GitHub)
// itch.io listing: https://brookcoli.itch.io/meowt-of-tune - GitHub Repository: https://github.com/Aurorafume/PPR301
// ==========================================================================
//
// WHAT DOES THIS DO:
// This script is a persistent singleton manager for the game's checkpoint
// system. It ensures that player progress (their last checkpoint location) is
// maintained even when scenes are reloaded. It holds the position and rotation
// of the last checkpoint the player activated.
//
// Core functionalities include:
// - Singleton pattern to guarantee a single, globally accessible instance.
// - Persists across scene loads using DontDestroyOnLoad.
// - Stores the position and rotation from the last activated checkpoint.
// - Provides a method to teleport the player back to the last checkpoint.
// - A helper function to check if a checkpoint has ever been saved.
//
// Dependencies:
// - Requires Checkpoint scripts in the scene to call its SetCheckpoint method.
// - Relies on a PlayerMovement script being attached to the player object to
//   find and teleport the player.
// - Needs to be placed on a GameObject in the initial scene of the game.
//
// ==========================================================================

using UnityEngine;

/// <summary>
/// A persistent singleton that manages checkpoint data. It stores the last
/// checkpoint location and handles moving the player to it.
/// </summary>
public class CheckpointManager : MonoBehaviour
{
    // Public static reference to the singleton instance.
    public static CheckpointManager Instance { get; private set; }

    private Vector3 lastCheckpointPosition;     // Stores the position of the last activated checkpoint.
    private Quaternion lastCheckpointRotation;  // Stores the player's rotation at the last checkpoint.
    private bool hasCheckpoint = false;         // Flag to check if any checkpoint has been saved.

    /// <summary>
    /// Enforces the singleton pattern, ensuring only one instance of the manager exists.
    /// </summary>
    private void Awake()
    {
        // If no instance exists, this becomes the instance...
        if (Instance == null) {
            Instance = this;
            // ...and it persists across scene loads.
            DontDestroyOnLoad(gameObject);
        }
        // If an instance already exists, destroy this duplicate.
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Updates the last saved checkpoint with a new position and rotation.
    /// </summary>
    /// <param name="position">The position of the new checkpoint.</param>
    /// <param name="rotation">The player's rotation at the new checkpoint.</param>
    public void SetCheckpoint(Vector3 position, Quaternion rotation)
    {
        // Update the stored position and rotation.
        lastCheckpointPosition = position;
        lastCheckpointRotation = rotation;
        // Mark that a checkpoint has now been saved.
        hasCheckpoint = true;
    }

    /// <summary>
    /// Finds the player and moves them to the last saved checkpoint.
    /// </summary>
    public void SendPlayerToLastCheckpoint()
    {
        // Find the player GameObject in the scene via its movement script.
        GameObject player = FindObjectOfType<PlayerMovement>().gameObject;
        
        // Instantly move the player to the saved checkpoint location and rotation.
        player.transform.position = lastCheckpointPosition;
        player.transform.rotation = lastCheckpointRotation;
    }

    /// <summary>
    /// Checks if a checkpoint has been saved during the game session.
    /// </summary>
    /// <returns>True if a checkpoint has been set, otherwise false.</returns>
    public bool HasCheckpoint()
    {
        return hasCheckpoint;
    }
}