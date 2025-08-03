// ==========================================================================
// Meowt of Tune - Out Of Bounds
// Owned and contributed to by: brookcoli (Itch.io) - Aurorafume (GitHub), donkzilla03 (Itch.io) - AliAK03 (GitHub), JaydenFielderTorrens (Ithc.io) - Squib35 (GitHub), komorebimoriko (Itch.io), TheRealCrizz (GitHub), LordVGahn (GitHub)
// itch.io listing: https://brookcoli.itch.io/meowt-of-tune - GitHub Repository: https://github.com/Aurorafume/PPR301
// ==========================================================================
//
// WHAT DOES THIS DO:
// This script manages the respawn behaviour for an object, typically the
// player. It resets the object's position to the current checkpoint if it
// falls below a certain height or collides with a designated hazard zone.
//
// Core functionalities include:
// - A "death plane" check on the Y-axis to detect if the player has fallen.
// - Collision detection with objects tagged as hazards ("Out of bounds", "Light").
// - Stores a 'currentRespawnLocation' that acts as the active checkpoint.
// - Provides a public array of spawn locations for other systems (like a
//   checkpoint or camera zone manager) to reference and use.
//
// Dependencies:
// - Must be attached to the object that needs to respawn (e.g. the player).
// - Requires hazard objects in the scene to have Collider components and the
//   correct tags ("Out of bounds", "Light").
// - Can work with other scripts that update its 'currentRespawnLocation' to
//   create a dynamic checkpoint system.
//
// ==========================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles respawning the attached object when it goes out of bounds or hits a hazard.
/// </summary>
public class OutOfBounds : MonoBehaviour
{
    [Header("Respawn Locations")]
    [Tooltip("An array of potential respawn locations, assignable in the Inspector. Other scripts can reference this array to set the current spawn point.")]
    public Vector3[] spawnLocationsArray;
    [Tooltip("The currently active respawn location where the object will be sent.")]
    public Vector3 currentRespawnLocation;

    /// <summary>
    /// Initialises the default respawn location to the object's starting position.
    /// </summary>
    void Start()
    {
        // By default, the first respawn point is where the object starts the scene.
        currentRespawnLocation =  transform.position;
    }

    /// <summary>
    /// Checks every frame if the object has fallen off the map.
    /// </summary>
    void Update()
    {
        // If the object's Y position is below the death plane, trigger a respawn.
        if(transform.position.y < -8f)
        {
            Respawn();
        }
    }

    /// <summary>
    /// Handles respawning when colliding with a physical hazard.
    /// </summary>
    /// <param name="collision">The collision data from the physics engine.</param>
    void OnCollisionEnter(Collision collision)
    {
        // Check if the object we collided with is tagged as a hazard.
        if (collision.gameObject.CompareTag("Out of bounds") || collision.gameObject.CompareTag("Light"))
        {
            Respawn();
        }
    }

    /// <summary>
    /// Resets the object's position to the current respawn location.
    /// </summary>
    void Respawn()
    {
        transform.position = currentRespawnLocation;
    }
}