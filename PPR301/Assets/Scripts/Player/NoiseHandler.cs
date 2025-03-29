using System.Collections;
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
    public AmbientNoise ambientNoise;
    public GameObject enemyManagerPrefab;

    // Enemy spawning
    public bool canSpawnEnemy = false;
    private static bool enemyExists = false;
    public float spawnCooldown = 10f;

    void Start()
    {       
        // Get the MicrophoneInput component (should be on the same GameObject)
        microphoneInput = GetComponent<MicrophoneInput>();
        if (microphoneInput == null)
        {   
            Debug.LogError("NoiseHandler: Missing MicrophoneInput component!");
        }
        
        // Ensure AmbientNoise reference is assigned in the Inspector
        if (ambientNoise == null)
        {
            Debug.LogError("NoiseHandler: AmbientNoise reference not set in inspector!");
        }

        additionalNoise = 0f;

        // Subscribe to the noise max event
        if (noiseBar != null)
        {   
            noiseBar.OnNoiseMaxed -= TrySpawnEnemyManager;
            noiseBar.OnNoiseMaxed += TrySpawnEnemyManager;
        }
    }

    void Update()
    {   
        // Only proceed if both microphone input and ambient calibration are ready
        if (microphoneInput != null && ambientNoise != null && ambientNoise.isCalibrated)
        {
            // Get current noise from microphone
            float micNoise = microphoneInput.GetCurrentNoiseLevel();

            // Subtract ambient noise baseline; ensure it doesn't drop below zero
            float adjustedNoise = Mathf.Max(micNoise - ambientNoise.ambientNoiseBaseline, 0f);

            // Combine with any extra noise (e.g., from jumps or collisions)
            float totalNoise = adjustedNoise + additionalNoise;

            // Update the noise bar with the adjusted noise level
            noiseBar.UpdateNoiseLevel(totalNoise);

            // Gradually decay the additional noise over time
            additionalNoise = Mathf.Lerp(additionalNoise, 0, Time.deltaTime * 0.5f);
        }
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

    public void GenerateNoise(float extraNoise)
    {   
        // Add extra noise (e.g., from player actions)
        additionalNoise += Mathf.Abs(extraNoise);
    }

    void TrySpawnEnemyManager()
    {   
        // Spawn enemy manager if conditions are met
        if (canSpawnEnemy && !enemyExists)
        {
            SpawnEnemyManager();
            enemyExists = true;
            StartCoroutine(SpawnCooldown());
        }
    }

    public static void NotifyEnemyDespawned()
    {   
        // Called externally when the enemy despawns
        enemyExists = false;
        GameObject enemyManager = GameObject.Find("Enemy2D(Clone)");
        Destroy(enemyManager);
    }

    void SpawnEnemyManager()
    {   
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
        // Prevent enemy manager spawns from occurring too frequently
        canSpawnEnemy = false;
        yield return new WaitForSeconds(spawnCooldown); 
        canSpawnEnemy = true;
    }
}
