using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NoiseBar : MonoBehaviour
{
    [Header("Noise Bar Settings")]
    [Tooltip("The image component for the noise bar")]
    public Image noiseBarImage;

    [Tooltip("The background image for the noise bar")]
    public Image background;

    [Tooltip("The frames for the noise bar animation")]
    public Sprite[] level1Frames;
    public Sprite[] level2Frames;
    public Sprite[] level3Frames;
    public Sprite[] level4Frames;
    public Sprite[] level5Frames;
    public Sprite[] level6Frames;
    public Sprite[] chaseWarningFrames;

    private Sprite[][] noiseLevels;
    private int currentFrame = 0;
    private float frameRate = 0.15f;
    private float nextFrameTime;
    private bool forceMaxBackground = false;
    
    [Header("Noise Level Settings")]
    [Tooltip("The maximum noise level before the enemy starts chasing")]
    public float noisePercentage = 0f;
    private float targetNoiseLevel = 0f;
    private float smoothSpeed = 5f;
    private bool isChasing = false;
    public event System.Action OnNoiseMaxed;
    public float TargetNoiseLevel { get; private set; }
    public States states;
    public EnemySpawning enemySpawning;

    void Start()
    {   
        // Initialise the noise bar and its components
        states = FindObjectOfType<States>();
        enemySpawning = FindObjectOfType<EnemySpawning>();

        noiseLevels = new Sprite[][] {
            level1Frames, level2Frames, level3Frames, level4Frames, level5Frames, level6Frames
        };

        noiseBarImage.sprite = level1Frames[0];
        nextFrameTime = Time.time + frameRate;
    }

    void Update()
    {   
        // Update the noise bar based on the current noise level
        noisePercentage = Mathf.Lerp(noisePercentage, targetNoiseLevel, Time.deltaTime * smoothSpeed);
        UpdateNoiseBarSprite();

        float redIntensity = Mathf.Lerp(0f, 1f, noisePercentage);
        float actualRedIntensity = forceMaxBackground ? 1f : redIntensity;
        float actualAlpha = forceMaxBackground ? 1f : redIntensity;
        float actualRadius = forceMaxBackground ? 0.3f : Mathf.Lerp(1f, 0.3f, noisePercentage);

        background.material.SetFloat("_Alpha", actualAlpha);
        background.material.SetFloat("_Radius", actualRadius);

        if (noisePercentage > 0.05f)
        {
            background.color = new Color(1f, 0f, 0f, actualRedIntensity);
            background.gameObject.SetActive(true);
        }
        else
        {
            background.gameObject.SetActive(false);
        }

        if (Time.time >= nextFrameTime)
        {
            currentFrame = (currentFrame + 1) % 4;
            nextFrameTime = Time.time + frameRate;
        }
    }

    public void UpdateNoiseLevel(float noiseLevel, float ambientBaseline, float voiceMargin)
    {   
        // Update the noise level based on the microphone input and ambient noise
        float dynamicMaxNoise = ambientBaseline + voiceMargin;
        targetNoiseLevel = Mathf.Clamp01(noiseLevel / dynamicMaxNoise);
        Debug.Log($"[NoiseBar] Noise: {noiseLevel}, Ambient: {ambientBaseline}, Margin: {voiceMargin}, Target %: {targetNoiseLevel}");

        if (targetNoiseLevel >= 1f && !isChasing && states.playerIsOnPlatform)
        {   
            // Check if the player is on a platform and trigger the chase warning
            Transform currentPlatform = enemySpawning.GetCurrentEnemySpawnPoint();
            if (currentPlatform == null)
            {
                Debug.LogWarning("No platform found for enemy spawn.");
                return;
            }

            isChasing = true;
            OnNoiseMaxed?.Invoke();
            StartCoroutine(ChaseWarningAnimation());
        }
    }

    void UpdateNoiseBarSprite()
    {   
        // Update the noise bar sprite based on the current noise level
        if (isChasing)
        {
            noiseBarImage.sprite = chaseWarningFrames[currentFrame];
            return;
        }

        int levelIndex = Mathf.Clamp(Mathf.FloorToInt(noisePercentage * noiseLevels.Length), 0, noiseLevels.Length - 1);
        noiseBarImage.sprite = noiseLevels[levelIndex][currentFrame];

        float alpha = Mathf.Lerp(0.5f, 1f, noisePercentage);
        noiseBarImage.color = new Color(1f, 1f, 1f, alpha);
    }

    IEnumerator ChaseWarningAnimation()
    {   
        // Play the chase warning animation
        while (isChasing)
        {
            noiseBarImage.sprite = chaseWarningFrames[currentFrame];
            yield return new WaitForSeconds(frameRate);
        }
    }

    public void ForceChaseVisuals(bool active)
    {   
        // Force the chase visuals on or off
        forceMaxBackground = active;
        isChasing = active;

        if (!active)
        {
            StopChase();
        }
    }

    public void StopChase()
    {   
        // Stop the chase visuals and reset the noise bar
        isChasing = false;
        noiseBarImage.sprite = level1Frames[0];
    }
}
