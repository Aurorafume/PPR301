// ==========================================================================
// Meowt of Tune - Grounded Trigger
// Owned and contributed to by: brookcoli (Itch.io) - Aurorafume (GitHub), donkzilla03 (Itch.io) - AliAK03 (GitHub), JaydenFielderTorrens (Itch.io) - Squib35 (GitHub), komorebimoriko (Itch.io), TheRealCrizz (GitHub), LordVGahn (GitHub)
// itch.io listing: https://brookcoli.itch.io/meowt-of-tune - GitHub Repository: https://github.com/Aurorafume/PPR301
// ==========================================================================
//
// WHAT DOES THIS DO:
// This script defines a trigger volume that modifies the player's movement
// state. While the player is inside this volume, their ability to jump is
// disabled, and their state is forcefully set to "grounded". This is useful
// for level design scenarios where player jumping should be restricted, such
// as on narrow walkways or during scripted events.
//
// Core functionalities include:
// - Detecting when the player enters, stays within, and exits the trigger.
// - Modifying boolean flags on the main 'PlayerMovement' script to alter its
//   behaviour.
// - Disabling the player's jump ability.
// - Forcing the player's grounded state to true, bypassing normal checks.
// - Reverting all changes when the player leaves the volume.
//
// Dependencies:
// - Must be attached to a GameObject with a Collider set to "Is Trigger".
// - A 'PlayerMovement' script must exist on the player object in the scene.
// - The player GameObject must be tagged as "Player".
//
// ==========================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A trigger volume that forces the player into a "no jump" and "grounded" state.
/// </summary>
public class GroundedTrigger : MonoBehaviour
{
    // A cached reference to the player's movement script.
    PlayerMovement playerMovement;

    /// <summary>
    /// Caches a reference to the PlayerMovement script on startup.
    /// </summary>
    void Awake()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();
    }

    /// <summary>
    /// Called when the player first enters the trigger volume.
    /// </summary>
    /// <param name="other">The collider that entered the trigger.</param>
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            // Immediately disable jumping when the player enters the zone.
            playerMovement.noJumpMode = true;
        }
    }

    /// <summary>
    /// Called every frame that the player remains inside the trigger volume.
    /// </summary>
    /// <param name="other">The collider staying inside the trigger.</param>
    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            // Continuously enforce the no-jump and grounded state for robustness.
            playerMovement.noJumpMode = true;
            playerMovement.groundedAlwaysTrue = true;
        }
    }

    /// <summary>
    /// Called when the player exits the trigger volume.
    /// </summary>
    /// <param name="other">The collider that exited the trigger.</param>
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            // Restore the player's normal movement abilities upon leaving the zone.
            playerMovement.noJumpMode = false;
            playerMovement.groundedAlwaysTrue = false;
        }
    }
}