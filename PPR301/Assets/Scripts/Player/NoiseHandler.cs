using System.Collections;
using UnityEngine;

public class NoiseHandler : MonoBehaviour
{
    // Extra noise events (e.g jump, collision)
    public float jumpNoise = 10f;
    public float collisionNoise = 5f;
    [Tooltip("Additional noise required above ambient to register as talking.")]
    public float voiceNoiseMargin = 5f;

    private float additionalNoise = 0f;

    // References
    public AmbientNoise ambientNoise;
    private MicrophoneInput microphoneInput;
    public NoiseBar noiseBar;
    public GameObject enemyManagerPrefab;

    // Enemy spawning
    public bool canSpawnEnemy = false;
    private static bool enemyExists = false;
    public float spawnCooldown = 10f;
    public Vector3 enemySpawnLocation;

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
    void TrySpawnEnemyManager()
    {       
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
        if (enemyManagerPrefab != null)
        {
            Instantiate(enemyManagerPrefab, enemySpawnLocation, Quaternion.identity);
            Debug.Log("Enemy Manager Spawned!");
        }
        else
        {
            Debug.LogError("Enemy Manager Prefab is not assigned!");
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
