using System.Collections;
using UnityEngine;

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
    {
        staticAmbientAudio = ambientAudio;
        staticChaseAudio = chaseAudio;

        if (ambientAudio != null)
        {
            ambientAudio.volume = 0f;
            ambientAudio.Play(); // Make sure loop = true and playOnAwake is off
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
    {
        float totalNoise = 0f;

        if (microphoneInput != null && ambientNoise != null && ambientNoise.isCalibrated)
        {
            float micNoise = microphoneInput.GetCurrentNoiseLevel();
            float adjustedNoise = Mathf.Max(micNoise - ambientNoise.ambientNoiseBaseline, 0f);
            totalNoise = adjustedNoise + additionalNoise;

            if (adjustedNoise < 1f)
                totalNoise = 0f;

            Debug.Log($"[NoiseHandler] Adjusted: {adjustedNoise}, Total: {totalNoise}");

            noiseBar.UpdateNoiseLevel(totalNoise, ambientNoise.ambientNoiseBaseline, voiceNoiseMargin);
            additionalNoise = Mathf.Lerp(additionalNoise, 0, Time.deltaTime * 0.5f);
        }

        // Use noise bar percentage directly for audio volume
        if (!enemyExists && ambientNoise.isCalibrated && ambientAudio != null && noiseBar != null)
        {
            float targetVolume = Mathf.Clamp01(noiseBar.CurrentNoisePercentage);
            Debug.Log($"[Audio] Volume target from noise bar: {targetVolume}");
            ambientAudio.volume = targetVolume;
            Debug.Log($"[Audio] NoiseBar %: {targetVolume} | Volume: {ambientAudio.volume}");

        }
    }

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

    public void TrySpawnEnemyManager()
    {
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

            if (ambientAudio != null) ambientAudio.Stop();
            if (chaseAudio != null && !chaseAudio.isPlaying) chaseAudio.Play();
        }
    }

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
                Debug.LogWarning("No matching platform/spawn point found.");
            }
        }
        else
        {
            Debug.LogError("Enemy Manager Prefab or EnemySpawning is not assigned!");
        }
    }

    public static void NotifyEnemyDespawned()
    {
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
    {
        canSpawnEnemy = false;
        yield return new WaitForSeconds(spawnCooldown);
        canSpawnEnemy = true;
    }
}
