using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; // Added for reset logic if needed

public class NoiseHandler : MonoBehaviour
{   
    [Header("Noise Settings")]
    public float jumpNoise;
    public float collisionNoise;
    public float voiceNoiseMargin;
    private float additionalNoise = 0f;

    [Header("References")]
    public AmbientNoise ambientNoise;
    private MicrophoneInput microphoneInput;
    public NoiseBar noiseBar;
    public GameObject enemyManagerPrefab;
    public EnemySpawning enemySpawning;

    [Header("Enemy Spawning")]
    public bool canSpawnEnemy = false;
    public static bool enemyExists = false;
    public float spawnCooldown = 10f;

    [Header("Audio")]
    public AudioSource ambientAudio;
    public AudioSource chaseAudio;

    public static AudioSource staticAmbientAudio;
    public static AudioSource staticChaseAudio;

    void Start()
    {   // Initialise static references
        staticAmbientAudio = ambientAudio;
        staticChaseAudio = chaseAudio;

        if (ambientAudio != null)
        {
            ambientAudio.volume = 0f;
            ambientAudio.Play();
        }

        microphoneInput = GetComponent<MicrophoneInput>();
        if (microphoneInput == null)
            Debug.LogError("NoiseHandler: Missing MicrophoneInput component!");

        if (ambientNoise == null)
            Debug.LogError("NoiseHandler: AmbientNoise reference not set!");

        if (noiseBar != null)
        {
            noiseBar.OnNoiseMaxed -= TrySpawnEnemyManager;
            noiseBar.OnNoiseMaxed += TrySpawnEnemyManager;
        }
    }

    void Update()
    {   // Update the noise level and audio volume based on the microphone input and ambient noise
        float totalNoise = 0f;

        if (microphoneInput != null && ambientNoise != null && ambientNoise.isCalibrated)
        {
            float micNoise = microphoneInput.GetCurrentNoiseLevel();
            float adjustedNoise = Mathf.Max(micNoise - ambientNoise.ambientNoiseBaseline, 0f);
            totalNoise = adjustedNoise + additionalNoise;

            if (adjustedNoise < 1f)
                totalNoise = 0f;

            noiseBar.UpdateNoiseLevel(totalNoise, ambientNoise.ambientNoiseBaseline, voiceNoiseMargin);
            additionalNoise = Mathf.Lerp(additionalNoise, 0, Time.deltaTime * 0.5f);
        }

        if (ambientNoise.isCalibrated && ambientAudio != null && noiseBar != null)
        {
            float targetVolume = Mathf.Clamp01(noiseBar.CurrentNoisePercentage);
            if (enemyExists) targetVolume *= 0.2f;
            ambientAudio.volume = Mathf.MoveTowards(ambientAudio.volume, targetVolume, Time.deltaTime * 1.5f);
        }
    }

    public void GenerateNoise(float extraNoise)
    {   // Generate noise based on player actions
        additionalNoise += Mathf.Abs(extraNoise);
    }

    void OnEnable()
    {   // Subscribe to noise bar events
        if (noiseBar != null)
        {
            noiseBar.OnNoiseMaxed -= TrySpawnEnemyManager;
            noiseBar.OnNoiseMaxed += TrySpawnEnemyManager;
        }
    }

    void OnDisable()
    {   // Unsubscribe from noise bar events
        if (noiseBar != null)
        {
            noiseBar.OnNoiseMaxed -= TrySpawnEnemyManager;
        }
    }

    public void TrySpawnEnemyManager()
    {   // Check if the player is on a platform and spawn the enemy manager if conditions are met
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

            if (chaseAudio != null && !chaseAudio.isPlaying) chaseAudio.Play();
        }
    }

    public void SpawnEnemyManager()
    {   // Spawn the enemy manager prefab at the current spawn point
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
                Debug.LogWarning("No matching platform/spawn point found.");
            }
        }
        else
        {
            Debug.LogError("Enemy Manager Prefab or EnemySpawning is not assigned!");
        }
    }

    public static void NotifyEnemyDespawned()
    {   // Notify the handler when the enemy despawns
        enemyExists = false;

        GameObject enemyManager = GameObject.Find("Enemy2D(Clone)");
        if (enemyManager != null)
            Destroy(enemyManager);

        if (staticChaseAudio != null)
            staticChaseAudio.Stop();

        if (staticAmbientAudio != null)
        {
            staticAmbientAudio.volume = 0f;
            staticAmbientAudio.Play();
        }
    }

    IEnumerator SpawnCooldown()
    {   // Coroutine to manage spawn cooldown
        canSpawnEnemy = false;
        yield return new WaitForSeconds(spawnCooldown);
        canSpawnEnemy = true;
    }

    public static void ResetStatics()
    {   // Reset static variables when the scene is reloaded
        enemyExists = false;
        staticAmbientAudio = null;
        staticChaseAudio = null;
    }
}
