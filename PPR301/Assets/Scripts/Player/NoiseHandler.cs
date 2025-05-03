using System.Collections;
using UnityEngine;

public class NoiseHandler : MonoBehaviour
{   
    [Header("Noise Settings")]
    [Tooltip("Noise generated when the player jumps")]
    public float jumpNoise;

    [Tooltip("Noise generated when the player collides with an object")]
    public float collisionNoise;

    [Tooltip("The amount above ambient noise required to consider the player as speaking. Higher values make it harder to trigger noise detection, which is useful in noisy environments.")] 
    public float voiceNoiseMargin;

    [Tooltip("Temporary noise added by gameplay events (e.g jumping or object collisions). This value decays over time.")]
    private float additionalNoise = 0f;

    [Header("References")]
    public AmbientNoise ambientNoise;
    private MicrophoneInput microphoneInput;
    public NoiseBar noiseBar;
    public GameObject enemyManagerPrefab;
    public EnemySpawning enemySpawning;

    [Header("Enemy Spawning")]
    [Tooltip("Can the enemy spawn?")]
    public bool canSpawnEnemy = false;

    [Tooltip("Is the enemy currently spawned?")]
    public static bool enemyExists = false;

    [Tooltip("The cooldown time before the enemy can spawn again.")]
    public float spawnCooldown = 10f;

    void Start()
    {   
        // Initialise references and check for missing components
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
        // Update the noise bar based on microphone input and ambient noise
        if (microphoneInput != null && ambientNoise != null && ambientNoise.isCalibrated)
        {
            float micNoise = microphoneInput.GetCurrentNoiseLevel();
            float adjustedNoise = Mathf.Max(micNoise - ambientNoise.ambientNoiseBaseline, 0f);
            float totalNoise = adjustedNoise + additionalNoise;

            // Reject clicks and low spikes
            if (adjustedNoise < 1f)
                totalNoise = 0f;

            Debug.Log($"[NoiseHandler] Adjusted: {adjustedNoise}, Total: {totalNoise}");

            noiseBar.UpdateNoiseLevel(totalNoise, ambientNoise.ambientNoiseBaseline, voiceNoiseMargin);
            additionalNoise = Mathf.Lerp(additionalNoise, 0, Time.deltaTime * 0.5f);
        }
    }

    public void GenerateNoise(float extraNoise)
    {   
        // Add temporary noise from gameplay events (e.g. jumping, colliding with objects)
        additionalNoise += Mathf.Abs(extraNoise);
    }

    void OnEnable()
    {   
        // Subscribe to the noise bar event when the object is enabled
        if (noiseBar != null)
        {
            noiseBar.OnNoiseMaxed -= TrySpawnEnemyManager;
            noiseBar.OnNoiseMaxed += TrySpawnEnemyManager;
        }
    }

    void OnDisable()
    {   
        // Unsubscribe from the noise bar event when the object is disabled
        if (noiseBar != null)
        {
            noiseBar.OnNoiseMaxed -= TrySpawnEnemyManager;
        }
    }

    public void TrySpawnEnemyManager()
    {   
        // Check if the enemy can spawn and if it already exists
        if (enemySpawning == null)
        {
            Debug.LogError("EnemySpawning reference is not set!");
            return;
        }

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

    void SpawnEnemyManager()
    {    
        // Check if the enemy manager prefab and enemy spawning reference are set, and spawn the enemy manager at the current platform
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

    public static void NotifyEnemyDespawned()
    {   
        // Notify that the enemy has been despawned and reset the enemy existence flag
        enemyExists = false;
        GameObject enemyManager = GameObject.Find("Enemy2D(Clone)");
        if (enemyManager != null)
        {
            Destroy(enemyManager);
        }
    }

    IEnumerator SpawnCooldown()
    {   
        // Set the spawn cooldown to prevent immediate respawning of the enemy
        canSpawnEnemy = false;
        yield return new WaitForSeconds(spawnCooldown);
        canSpawnEnemy = true;
    }
}
