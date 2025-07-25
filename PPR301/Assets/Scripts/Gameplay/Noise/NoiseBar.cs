using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NoiseBar : MonoBehaviour
{
    [Header("Noise Bar Settings")]
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

    [Header("Noise Level Settings")]
    private float noisePercentage = 0f;
    private float targetNoiseLevel = 0f;
    private float smoothSpeed = 5f;
    private bool isChasing = false;
    public event System.Action OnNoiseMaxed;

    [Header("References")]
    public States states;
    public EnemySpawning enemySpawning;

    public float CurrentNoisePercentage => noisePercentage;

    void Start()
    {
        states = FindObjectOfType<States>();
        enemySpawning = FindObjectOfType<EnemySpawning>();

        noiseLevels = new Sprite[][] {
            level1Frames, level2Frames, level3Frames, level4Frames, level5Frames, level6Frames
        };

        if (level1Frames.Length > 0)
        {
            noiseBarImage.sprite = level1Frames[0];
        }
        nextFrameTime = Time.time + frameRate;
    }

    void Update()
    {
        noisePercentage = Mathf.Lerp(noisePercentage, targetNoiseLevel, Time.deltaTime * smoothSpeed);
        UpdateNoiseBarSprite();

        float redIntensity = Mathf.Lerp(0f, 1f, noisePercentage);
        float actualRedIntensity = forceMaxBackground ? 1f : redIntensity;
        float actualAlpha = forceMaxBackground ? 1f : redIntensity;
        float actualRadius = forceMaxBackground ? 0.3f : Mathf.Lerp(1f, 0.3f, noisePercentage);

        if (background.material != null)
        {
            background.material.SetFloat("_Alpha", actualAlpha);
            background.material.SetFloat("_Radius", actualRadius);
        }

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

    public void UpdateNoiseLevel(float currentNoise, float maxNoise)
    {
        if (maxNoise <= 0)
        {
            targetNoiseLevel = 0f;
            return;
        }

        targetNoiseLevel = Mathf.Clamp01(currentNoise / maxNoise);

        if (targetNoiseLevel >= 1f && !isChasing && states != null && states.playerIsOnPlatform)
        {
            if (enemySpawning == null || enemySpawning.GetCurrentEnemySpawnPoint() == null)
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
        if (isChasing)
        {
            if (chaseWarningFrames.Length > 0)
                noiseBarImage.sprite = chaseWarningFrames[currentFrame % chaseWarningFrames.Length];
            return;
        }

        int levelIndex = Mathf.Clamp(Mathf.FloorToInt(noisePercentage * noiseLevels.Length), 0, noiseLevels.Length - 1);

        if (noiseLevels[levelIndex] != null && noiseLevels[levelIndex].Length > 0)
        {
            noiseBarImage.sprite = noiseLevels[levelIndex][currentFrame % noiseLevels[levelIndex].Length];
        }

        float alpha = Mathf.Lerp(0.5f, 1f, noisePercentage);
        noiseBarImage.color = new Color(1f, 1f, 1f, alpha);
    }

    IEnumerator ChaseWarningAnimation()
    {
        while (isChasing)
        {
            if (chaseWarningFrames.Length > 0)
            {
                noiseBarImage.sprite = chaseWarningFrames[currentFrame % chaseWarningFrames.Length];
            }
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
        if (level1Frames.Length > 0)
        {
            noiseBarImage.sprite = level1Frames[0];
        }
    }
    
    public void ForceNoiseSpikeFromLight()
    {
        targetNoiseLevel = 1f;
        noisePercentage = 1f;

        if (!isChasing && states.playerIsOnPlatform)
        {
            isChasing = true;
            OnNoiseMaxed?.Invoke();
            StartCoroutine(ChaseWarningAnimation());
        }
    }
}
