// ==========================================================================
// Meowt of Tune - Enemy Spawning
// Owned and contributed to by: brookcoli (Itch.io) - Aurorafume (GitHub), donkzilla03 (Itch.io) - AliAK03 (GitHub), JaydenFielderTorrens (Itch.io) - Squib35 (GitHub), komorebimoriko (Itch.io), TheRealCrizz (GitHub), LordVGahn (GitHub)
// itch.io listing: https://brookcoli.itch.io/meowt-of-tune - GitHub Repository: https://github.com/Aurorafume/PPR301
// ==========================================================================
//
// WHAT DOES THIS DO:
// This script manages enemy spawn locations in the scene. It determines where 
// enemies should appear based on the platform the player is currently on. It 
// allows developers to define pairs of platforms and spawn points, and uses a 
// raycast from the player's position to detect which platform they’re on.
//
// Core functionalities include:
// - Struct-based mapping of platforms to enemy spawn points.
// - Raycast logic to detect which platform the player is currently above.
// - Returns a matching spawn point for enemy instantiation.
// - LightSpawnPair support for extended pairing logic (not yet utilised).
//
// Dependencies:
// - UnityEngine.Physics for raycasting.
// - States, NoiseHandler, and NoiseBar scripts for integration with gameplay.
// - Platform objects must be assigned a specific layer.
//
// ==========================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawning : MonoBehaviour
{
    // Pairing between a platform object and its corresponding enemy spawn location
    [System.Serializable]
    public class PlatformSpawnPair
    {
        public GameObject platform;     // Platform player can stand on
        public GameObject spawnPoint;   // Where the enemy will spawn if player is on the platform
    }

    // Optional light-to-spawn pairing (e.g. used for effects, enemy types, etc.)
    [System.Serializable]
    public class LightSpawnPair
    {
        public MyLight lightSource;
        public Transform spawnPoint;
    }

    [Header("Enemy Spawning Pairs")]
    [Tooltip("Pairs of platforms and their corresponding spawn points.")]
    public PlatformSpawnPair[] platformSpawnPairs;

    [Header("Light to Spawn Point Mapping")]
    [Tooltip("Optional light-object to spawn point mappings.")]
    public LightSpawnPair[] lightSpawnPairs;

    [Header("Layer Masks")]
    [Tooltip("Layer mask for platforms.")]
    public LayerMask PlatformLayer;

    [Header("References")]
    [Tooltip("Enemy prefab to spawn.")]
    public GameObject enemyAI;
    [Tooltip("Noise bar that reflects enemy awareness.")]
    public NoiseBar noiseBar;
    [Tooltip("Reference to global state manager.")]
    public States states;
    [Tooltip("Handles noise-related interactions and enemy tracking.")]
    public NoiseHandler noiseHandler;
    [Tooltip("Reference to the player object.")]
    public GameObject Player;

    void Start()
    {
        // Log platform/spawn mappings at start for validation
        foreach (var pair in platformSpawnPairs)
        {
            Debug.Log($"Platform: {pair.platform.name} paired with Spawn: {pair.spawnPoint.name}");
        }
    }

    void Update()
    {
        // --- DEBUG CHECKS ---
        // Find what platform the player is currently on
        Transform currentSpawnPoint = GetCurrentEnemySpawnPoint();
        if (currentSpawnPoint != null)
        {
            Debug.Log($"Current Spawn Point: {currentSpawnPoint.name}");
        }
        else
        {
            Debug.LogWarning("No valid spawn point found for the player.");
        }
    }

    /// <summary>
    /// Checks if the player is currently standing on a platform,
    /// and if so, returns the spawn point assigned to that platform.
    /// </summary>
    /// <returns>The corresponding enemy spawn point, or null if no match found.</returns>
    public Transform GetCurrentEnemySpawnPoint()
    {
        // --- START DEBUG CHECKS ---
        if (Player == null)
        {
            Debug.LogError("EnemySpawning: The 'Player' GameObject reference is NULL. Re-finding it now.");
            Player = GameObject.Find("Player"); // Attempt to re-find it
            if (Player == null)
            {
                Debug.LogError("FATAL: EnemySpawning could NOT find the 'Player' object even after re-trying. Spawning is impossible.");
                return null;
            }
        }
        // --- END DEBUG CHECKS ---

        // Validate required references
        if (states == null || Player == null || platformSpawnPairs == null || platformSpawnPairs.Length == 0)
            return null;

        RaycastHit hit;
        float checkDistance = 2f; // Distance to check below the player

        // Cast a ray downward from the player to check if they are on a platform
        if (Physics.Raycast(Player.transform.position, Vector3.down, out hit, checkDistance, PlatformLayer))
        {
            GameObject platformHit = hit.collider.gameObject;

            // Compare hit platform to each platform-spawn pair
            foreach (var pair in platformSpawnPairs)
            {
                if (pair.platform == platformHit)
                {
                    Debug.Log("Enemy Spawn Point Found: " + pair.spawnPoint.name);
                    return pair.spawnPoint.transform;
                }
            }
        }

        // No matching platform found
        return null;
    }
}
