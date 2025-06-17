using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawning : MonoBehaviour
{   
    [System.Serializable]
    public class PlatformSpawnPair
    {
        public GameObject platform;
        public GameObject spawnPoint;
    }

    [Header("Enemy Spawning Pairs")]
    [Tooltip("Pairs of platforms and their corresponding spawn points.")]
    public PlatformSpawnPair[] platformSpawnPairs;

    [Header("Layer Masks")]
    [Tooltip("Layer mask for platforms.")]
    public LayerMask PlatformLayer;

    [Header("References")]
    public GameObject enemyAI;
    public NoiseBar noiseBar;
    public States states;
    public NoiseHandler noiseHandler;
    public GameObject Player;


    void Start()
    {   
        // Check for missing references
        foreach (var pair in platformSpawnPairs)
        {
            Debug.Log($"Platform: {pair.platform.name} paired with Spawn: {pair.spawnPoint.name}");
        }
    }

    public Transform GetCurrentEnemySpawnPoint()
    {
        // Check if the player is on a platform and return the corresponding spawn point
        //Debug.Log("GetCurrentEnemySpawnPoint called");

        if (states == null || Player == null || platformSpawnPairs == null || platformSpawnPairs.Length == 0)
            return null;

        RaycastHit hit;
        float checkDistance = 2f;

        // Check if the player is on a platform using a raycast
        if (Physics.Raycast(Player.transform.position, Vector3.down, out hit, checkDistance, PlatformLayer))
        {
            GameObject platformHit = hit.collider.gameObject;
            //Debug.Log("Platform Hit: " + platformHit.name);

            foreach (var pair in platformSpawnPairs)
            {
                // Check if the platform in the pair matches the hit platform
                if (pair.platform == platformHit)
                {
                    Debug.Log("Enemy Spawn Point Found: " + pair.spawnPoint.name);
                    return pair.spawnPoint.transform;
                }
            }
        }

        return null;
    }
}
