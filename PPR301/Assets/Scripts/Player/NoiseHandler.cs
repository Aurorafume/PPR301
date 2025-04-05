using System.Collections;
using UnityEngine;

public class NoiseHandler : MonoBehaviour
{
    public float jumpNoise = 10f;
    public float collisionNoise = 5f;
    public float voiceNoiseMargin = 5f;
    private float additionalNoise = 0f;

    public AmbientNoise ambientNoise;
    private MicrophoneInput microphoneInput;
    public NoiseBar noiseBar;
    public GameObject enemyManagerPrefab;

    public bool canSpawnEnemy = false;
    public static bool enemyExists = false;
    public float spawnCooldown = 10f;
    public EnemySpawning enemySpawning;

    void Start()
    {       
        microphoneInput = GetComponent<MicrophoneInput>();
        if (microphoneInput == null)
        {
            Debug.LogError("NoiseHandler: Missing MicrophoneInput component!");
        }
        
        if (ambientNoise == null)
        {
            Debug.LogError("NoiseHandler: AmbientNoise reference not set in inspector!");
        }

        additionalNoise = 0f;

        if (noiseBar != null)
        {
            noiseBar.OnNoiseMaxed -= TrySpawnEnemyManager;
            noiseBar.OnNoiseMaxed += TrySpawnEnemyManager;
        }
    }

    void Update()
    {       
        // Proceed only if the microphone and ambient calibration are ready
        if (microphoneInput != null && ambientNoise != null && ambientNoise.isCalibrated)
        {
            float micNoise = microphoneInput.GetCurrentNoiseLevel();

            // Subtract ambient noise; ensure we don't go negative
            float adjustedNoise = Mathf.Max(micNoise - ambientNoise.ambientNoiseBaseline, 0f);

            // Combine with any extra noise events (jumps, collisions, etc.)
            float totalNoise = adjustedNoise + additionalNoise;

            // Update the noise bar using a dynamic maximum based on ambient noise + margin
            noiseBar.UpdateNoiseLevel(totalNoise, ambientNoise.ambientNoiseBaseline, voiceNoiseMargin);

            // Decay the additional noise over time
            additionalNoise = Mathf.Lerp(additionalNoise, 0, Time.deltaTime * 0.5f);
        }
    }

    // Called by other scripts (e.g on jump or collision events)
    public void GenerateNoise(float extraNoise)
    {       
        additionalNoise += Mathf.Abs(extraNoise);
    }

    void OnEnable()
    {       
        if (noiseBar != null)
        {
            noiseBar.OnNoiseMaxed -= TrySpawnEnemyManager;
            noiseBar.OnNoiseMaxed += TrySpawnEnemyManager;
        }
    }

    void OnDisable()
    {       
        if (noiseBar != null)
        {
            noiseBar.OnNoiseMaxed -= TrySpawnEnemyManager;
        }
    }

    // Trigger enemy spawn when noise reaches a critical level
    public void TrySpawnEnemyManager()
    {   
        if (enemySpawning == null)
        {
            Debug.LogError("EnemySpawning reference is not set!");
            return;
        }

        // Check what platform the player is on
        Transform currentPlatform = enemySpawning.GetCurrentEnemySpawnPoint();
        if (currentPlatform == null)
        {
            Debug.LogWarning("No platform found for enemy spawn.");
            return;
        }

        if (canSpawnEnemy && !enemyExists)
        {
            SpawnEnemyManager();
            enemyExists = true;
            StartCoroutine(SpawnCooldown());
        }
    }

    // Spawns the enemy manager
    void SpawnEnemyManager()
    {
        if (enemyManagerPrefab != null && enemySpawning != null)
        {
            Transform spawnPoint = enemySpawning.GetCurrentEnemySpawnPoint();
            if (spawnPoint != null)
            {
                Instantiate(enemyManagerPrefab, spawnPoint.position, spawnPoint.rotation);
                Debug.Log("Enemy Manager Spawned at " + spawnPoint.position);
            }
            else
            {
                Debug.LogWarning("No matching platform/spawn point found for current player position.");
            }
        }
        else
        {
            Debug.LogError("Enemy Manager Prefab or EnemySpawning is not assigned!");
        }
    }

    // Called by EnemyAI when the enemy has fully faded out and should be despawned.
    public static void NotifyEnemyDespawned()
    {       
        enemyExists = false;
        GameObject enemyManager = GameObject.Find("Enemy2D(Clone)");
        if (enemyManager != null)
        {
            Destroy(enemyManager);
        }
    }

    IEnumerator SpawnCooldown()
    {       
        canSpawnEnemy = false;
        yield return new WaitForSeconds(spawnCooldown); 
        canSpawnEnemy = true;
    }
}
