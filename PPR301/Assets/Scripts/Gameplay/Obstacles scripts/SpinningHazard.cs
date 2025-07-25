// ==========================================================================
// Meowt of Tune - Spinning Hazard
// Owned and contributed to by: brookcoli (Itch.io) - Aurorafume (GitHub), donkzilla03 (Itch.io) - AliAK03 (GitHub), JaydenFielderTorrens (Itch.io) - Squib35 (GitHub), komorebimoriko (Itch.io), TheRealCrizz (GitHub), LordVGahn (GitHub)
// itch.io listing: https://brookcoli.itch.io/meowt-of-tune - GitHub Repository: https://github.com/Aurorafume/PPR301
// ==========================================================================
//
// WHAT DOES THIS DO:
// This script procedurally generates a pattern of hazard objects at game start.
// It creates a series of concentric, cross-shaped rings of objects around a
// central point. All spawned hazards are parented to a specified Transform.
//
// To create the "spinning" effect, an animator or another script (like an
// ObjectSpinner) should be used to rotate the designated parent object.
//
// Core functionalities include:
// - Spawning a configurable number of hazard rings.
// - Setting the distance between each ring.
// - Parenting all spawned objects to a central rotator for synchronised movement.
//
// Dependencies:
// - UnityEngine for core game object functionality.
// - A separate script or animator is required on the parent object to make it spin.
//
// ==========================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawns concentric rings of hazard objects and parents them to a central Transform.
/// </summary>
public class SpinningHazard : MonoBehaviour
{
    [Header("Spawner Settings")]
    [Tooltip("The number of concentric rings of hazards to spawn.")]
    public int ringsToSpawn = 3;
    [Tooltip("The distance for the first ring and the spacing between subsequent rings.")]
    public float ringDistance = 2f;
    [Tooltip("The hazard GameObject prefab to instantiate.")]
    public GameObject hazardPrefab;
    [Tooltip("The parent Transform for all spawned hazards. This is the object that should be rotated.")]
    public Transform parentObject;

    /// <summary>
    /// Called by Unity when the script instance is being loaded.
    /// </summary>
    void Start()
    {
        // Check for missing references to prevent errors.
        if (hazardPrefab == null)
        {
            Debug.LogError("Hazard Prefab is not assigned.", this);
            return;
        }
        if (parentObject == null)
        {
            Debug.LogError("Parent Object is not assigned.", this);
            return;
        }

        SpawnHazardPattern();
    }

    /// <summary>
    /// Generates the hazard objects in a pattern of concentric rings.
    /// </summary>
    private void SpawnHazardPattern()
    {
        float currentRingDistance = ringDistance;

        // Loop for each ring we want to spawn.
        for (int i = 0; i < ringsToSpawn; i++)
        {
            // Spawn four hazards in a cross shape for the current ring.
            Vector3 center = transform.position;
            SpawnHazard(center + new Vector3(currentRingDistance, 0, 0)); // Right
            SpawnHazard(center + new Vector3(-currentRingDistance, 0, 0)); // Left
            SpawnHazard(center + new Vector3(0, 0, currentRingDistance)); // Forward
            SpawnHazard(center + new Vector3(0, 0, -currentRingDistance)); // Back

            // Increase the distance for the next ring.
            currentRingDistance += ringDistance;
        }
    }

    /// <summary>
    /// Instantiates a single hazard prefab at a given position and parents it.
    /// </summary>
    /// <param name="position">The world position to spawn the hazard.</param>
    private void SpawnHazard(Vector3 position)
    {
        GameObject newHazard = Instantiate(hazardPrefab, position, Quaternion.identity);
        newHazard.transform.SetParent(parentObject);
    }
}