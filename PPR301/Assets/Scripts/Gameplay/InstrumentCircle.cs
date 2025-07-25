// ==========================================================================
// Meowt of Tune - Instrument Circle
// Owned and contributed to by: brookcoli (Itch.io) - Aurorafume (GitHub), donkzilla03 (Itch.io) - AliAK03 (GitHub), JaydenFielderTorrens (Itch.io) - Squib35 (GitHub), komorebimoriko (Itch.io), TheRealCrizz (GitHub), LordVGahn (GitHub)
// itch.io listing: https://brookcoli.itch.io/meowt-of-tune - GitHub Repository: https://github.com/Aurorafume/PPR301
// ==========================================================================
//
// WHAT DOES THIS DO:
// This script acts as a manager to detect when the player makes contact with
// any of a specified list of "instrument" objects in the scene. When contact
// is made, it signals a central NoiseHandler to attempt an enemy spawn,
// simulating a loud, attention-grabbing noise.
//
// Core functionalities include:
// - Monitoring a list of GameObjects for player contact.
// - Checking for collision every frame using collider bounds.
// - Triggering an enemy spawn via the NoiseHandler when a collision is detected.
//
// Dependencies:
// - UnityEngine for core and physics functionality.
// - A NoiseHandler script must exist in the scene.
// - The player's GameObject must be tagged "Player".
// - All specified instruments and the player must have Collider components.
//
// ==========================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Detects if the player is touching any instrument in a specified list and triggers a noise event.
/// </summary>
public class InstrumentCircle : MonoBehaviour
{
    [Header("Monitored Instruments")]
    [Tooltip("A list of instrument GameObjects that will trigger a noise event when touched.")]
    public GameObject[] instruments;

    [Header("Component References")]
    [Tooltip("A direct reference to the player's collider for bounds checking.")]
    public Collider playerCollider;

    // Cached references to scene objects and components.
    private NoiseHandler noiseHandler;
    private GameObject player;

    /// <summary>
    /// Called by Unity on startup to cache references to the NoiseHandler and player.
    /// </summary>
    private void Start()
    {
        noiseHandler = FindObjectOfType<NoiseHandler>();
        if (noiseHandler == null)
        {
            Debug.LogError("InstrumentCircle: NoiseHandler not found in scene.", this);
        }

        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("InstrumentCircle: No GameObject with tag 'Player' found in scene.", this);
        }
    }

    /// <summary>
    /// Called every frame by Unity to check for player contact with instruments.
    /// </summary>
    private void Update()
    {
        // Do not proceed if essential references are missing.
        if (noiseHandler == null || player == null) return;

        // Check each instrument in the list for contact.
        foreach (GameObject instrument in instruments)
        {
            if (instrument != null && IsTouchingInstrument(instrument))
            {
                // If contact is made, attempt to spawn an enemy and stop checking for this frame.
                noiseHandler.TrySpawnEnemyManager();
                Debug.Log("Triggered enemy spawn from instrument: " + instrument.name);
                break;
            }
        }
    }

    /// <summary>
    /// Checks if the player's collider bounds are intersecting with a given instrument's collider bounds.
    /// </summary>
    /// <param name="instrument">The instrument GameObject to check against.</param>
    /// <returns>True if the bounds intersect, false otherwise.</returns>
    private bool IsTouchingInstrument(GameObject instrument)
    {
        Collider instrumentCollider = instrument.GetComponent<Collider>();

        if (instrumentCollider == null)
        {
            Debug.LogWarning($"Instrument '{instrument.name}' is missing a Collider component.", instrument);
            return false;
        }

        if (playerCollider == null)
        {
            Debug.LogError("Player Collider reference is not assigned in the Inspector.", this);
            return false;
        }

        // Return true if the player's bounding box intersects with the instrument's bounding box.
        return instrumentCollider.bounds.Intersects(playerCollider.bounds);
    }
}