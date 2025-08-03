// ==========================================================================
// Meowt of Tune - Player Teleport
// Owned and contributed to by: brookcoli (Itch.io) - Aurorafume (GitHub), donkzilla03 (Itch.io) - AliAK03 (GitHub), JaydenFielderTorrens (Itch.io) - Squib35 (GitHub), komorebimoriko (Itch.io), TheRealCrizz (GitHub), LordVGahn (GitHub)
// itch.io listing: https://brookcoli.itch.io/meowt-of-tune - GitHub Repository: https://github.com/Aurorafume/PPR301
// ==========================================================================
//
// WHAT DOES THIS DO:
// This script provides a simple teleportation functionality. When the player
// character enters the trigger volume this script is attached to, they are
// immediately moved to a specified target location.
//
// Core functionalities include:
// - Detecting player entry into a trigger volume.
// - Instantly teleporting the player to a target transform's position.
//
// Dependencies:
// - Must be attached to a GameObject with a Collider set to "Is Trigger".
// - The 'player' and 'teleportTarget' transforms must be assigned in the
//   Inspector.
// - The player GameObject must be tagged as "Player".
//
// ==========================================================================

using UnityEngine;

/// <summary>
/// A simple trigger that teleports the player to a target location upon entry.
/// </summary>
public class PlayerTeleport : MonoBehaviour
{
    [Header("Teleport References")]
    [Tooltip("The player's transform to be teleported.")]
    public Transform player;
    [Tooltip("The destination transform where the player will be teleported.")]
    public Transform teleportTarget;

    /// <summary>
    /// Called when another collider enters this object's trigger volume.
    /// </summary>
    /// <param name="other">The collider that entered the trigger.</param>
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that entered is the player.
        if (other.CompareTag("Player"))
        {
            // Instantly move the player to the target's position.
            player.position = teleportTarget.position;
        }
    }
}