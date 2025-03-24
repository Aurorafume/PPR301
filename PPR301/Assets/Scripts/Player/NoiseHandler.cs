using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseHandler : MonoBehaviour
{
    // Noise parameters
    public float jumpNoise = 5f; 
    public float collisionNoise = 10f;
    public float voiceNoiseThreshold;
    private float additionalNoise = 0f;

    // References
    public PlayerMovement playerMovement;
    private MicrophoneInput microphoneInput;
    public NoiseBar noiseBar;
    public GameObject enemyManagerPrefab;

    // Enemy
    public bool canSpawnEnemy = false;
    private static bool enemyExists = false;
    public float spawnCooldown = 10f;

    void Start()
    {       
        // Get references
        microphoneInput = GetComponent<MicrophoneInput>();
        additionalNoise = 0f;

        if (microphoneInput == null)
        {   
            // Log error if microphone input component is missing
            Debug.LogError("MicrophoneInput component is missing!");
        }

        if (noiseBar != null)
        {   
            // Subscribe to event
            noiseBar.OnNoiseMaxed -= TrySpawnEnemyManager;
            noiseBar.OnNoiseMaxed += TrySpawnEnemyManager;
        }

    }

    void Update()
    {   
        if (microphoneInput != null)
        {
            // Get current noise level from mic
            float micNoise = microphoneInput.GetCurrentNoiseLevel();
            
            // Combine mic noise with accumulated noise
            float totalNoise = micNoise + additionalNoise;

            // Update noise bar
            noiseBar.UpdateNoiseLevel(totalNoise);

            // Decay additional noise over time (simulating noise fading)
            additionalNoise = Mathf.Lerp(additionalNoise, 0, Time.deltaTime * 0.5f);
        }
    }

    void OnEnable()
    {   
        // Subscribe to event
        if (noiseBar != null)
        {
            noiseBar.OnNoiseMaxed -= TrySpawnEnemyManager;
            noiseBar.OnNoiseMaxed += TrySpawnEnemyManager;
        }
    }

    void OnDisable()
    {   
        // Unsubscribe from event
        if (noiseBar != null)
        {
            noiseBar.OnNoiseMaxed -= TrySpawnEnemyManager;
        }
    }

    public void GenerateNoise(float extraNoise)
    {   
        // Add extra noise to the accumulated noise
        additionalNoise += Mathf.Abs(extraNoise);
    }

    void TrySpawnEnemyManager()
    {   
        // Try to spawn enemy manager if conditions are met
        if (canSpawnEnemy && !enemyExists)
        {
            SpawnEnemyManager();
            enemyExists = true;
            StartCoroutine(SpawnCooldown());
        }
    }

    public static void NotifyEnemyDespawned()
    {   
        // Notify that enemy has been despawned and delete the enemy manager
        enemyExists = false;
        GameObject enemyManager = GameObject.Find("Enemy2D(Clone)");
        Destroy(enemyManager);
    }

    void SpawnEnemyManager()
    {   
        // Spawn enemy manager prefab
        if (enemyManagerPrefab != null)
        {
            Instantiate(enemyManagerPrefab, Vector3.zero, Quaternion.identity);
            Debug.Log("Enemy Manager Spawned!");
        }
        else
        {
            Debug.LogError("Enemy Manager Prefab is not assigned!");
        }
    }

    IEnumerator SpawnCooldown()
    {   
        // Prevent enemy manager from spawning too frequently
        canSpawnEnemy = false;
        yield return new WaitForSeconds(spawnCooldown); 
        canSpawnEnemy = true;
    }
}
