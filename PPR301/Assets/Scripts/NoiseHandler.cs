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
    public float spawnCooldown = 10f;

    void Start()
    {   
        microphoneInput = GetComponent<MicrophoneInput>();
        additionalNoise = 0f;

        // Check if MicrophoneInput component is missing
        if (microphoneInput == null)
        {
            Debug.LogError("MicrophoneInput component is missing!");
        }

        // Subscribe to noise max event
        if (noiseBar != null)
        {
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

            // Debug log for tracking
            Debug.Log($"Mic Noise: {micNoise}, Additional Noise: {additionalNoise}, Total Noise: {totalNoise}");

            // Update noise bar
            noiseBar.UpdateNoiseLevel(totalNoise);

            // Decay additional noise over time (simulating noise fading)
            additionalNoise = Mathf.Lerp(additionalNoise, 0, Time.deltaTime * 0.5f);
        }
    }

    public void GenerateNoise(float extraNoise)
    {   
        // Add extra noise to the accumulated noise
        additionalNoise += Mathf.Abs(extraNoise);

        // Debugging
        Debug.Log($"New Noise Added: {extraNoise}, Total Additional Noise: {additionalNoise}");
    }

    void TrySpawnEnemyManager()
    {   
        // Check if enemy manager can spawn
        if (canSpawnEnemy)
        {
            SpawnEnemyManager();
            StartCoroutine(SpawnCooldown());
        }
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
