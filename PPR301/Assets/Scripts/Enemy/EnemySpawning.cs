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

    public PlatformSpawnPair[] platformSpawnPairs;
    public LayerMask PlatformLayer;
    public EnemyAI enemyAI;
    public GameObject enemyPrefab;
    public NoiseBar noiseBar;
    public States states;
    public NoiseHandler noiseHandler;
    public GameObject Player;


    void Start()
    {   
        foreach (var pair in platformSpawnPairs)
        {
            Debug.Log($"Platform: {pair.platform.name} paired with Spawn: {pair.spawnPoint.name}");
        }
    }

    public Transform GetCurrentEnemySpawnPoint()
    {
        Debug.Log("GetCurrentEnemySpawnPoint called");

        if (states == null || Player == null || platformSpawnPairs == null || platformSpawnPairs.Length == 0)
            return null;

        RaycastHit hit;
        float checkDistance = 2f;

        if (Physics.Raycast(Player.transform.position, Vector3.down, out hit, checkDistance, PlatformLayer))
        {
            GameObject platformHit = hit.collider.gameObject;
            Debug.Log("Platform Hit: " + platformHit.name);

            foreach (var pair in platformSpawnPairs)
            {
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
