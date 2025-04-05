using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NoiseBar : MonoBehaviour
{
    public Image noiseBarImage;
    public Image background; 
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
        // Smoothly interpolate noise percentage for a fluid UI update
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

    // Updated to accept dynamic parameters: ambientBaseline and voiceMargin
    public void UpdateNoiseLevel(float noiseLevel, float ambientBaseline, float voiceMargin)
    {
        // The dynamic maximum is the ambient baseline plus the additional margin required to register talking.
        float dynamicMaxNoise = ambientBaseline + voiceMargin;
        // Calculate the percentage relative to this dynamic maximum.
        targetNoiseLevel = Mathf.Clamp01(noiseLevel / dynamicMaxNoise);

        // If the noise level reaches the top of the scale, trigger an event (e.g for enemy spawns).
        if (targetNoiseLevel >= 1f && !isChasing && states.playerIsOnPlatform)
        {   
            // Find the specific platform and spawn point
            Transform currentPlatform = enemySpawning.GetCurrentEnemySpawnPoint();
            if (currentPlatform == null)
            {
                Debug.LogWarning("No platform found for enemy spawn.");
                return;
            }

            if (currentPlatform != null)
            {
                isChasing = true;
                OnNoiseMaxed?.Invoke();
                StartCoroutine(ChaseWarningAnimation());
            }
        }
    }

    void UpdateNoiseBarSprite()
    {
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
        while (isChasing)
        {
            noiseBarImage.sprite = chaseWarningFrames[currentFrame];
            yield return new WaitForSeconds(frameRate);
        }
    }   

    public void ForceChaseVisuals(bool active)
    {   
        forceMaxBackground = active;
        isChasing = active;

        if (!active)
        {
            StopChase();
        }
    }

    public void StopChase()
    {   
        isChasing = false;
        noiseBarImage.sprite = level1Frames[0];
    }
}
