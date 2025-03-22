using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NoiseBar : MonoBehaviour
{
    [Header("UI Components")]
    public Image noiseBarImage;
    public Image background; 

    [Header("Sprite Frames")]
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

    [Header("Noise Variables")]
    private float minNoise = 0f;
    private float maxNoise = 80f;
    private float noisePercentage = 0f;
    private float targetNoiseLevel = 0f;
    private float smoothSpeed = 5f;
    private bool isChasing = false;
    public event System.Action OnNoiseMaxed;

    void Start()
    {
        // Store noise level sprite frames
        noiseLevels = new Sprite[][] {
            level1Frames, level2Frames, level3Frames, level4Frames, level5Frames, level6Frames
        };

        // Set initial noise bar sprite
        noiseBarImage.sprite = level1Frames[0];
        nextFrameTime = Time.time + frameRate;
    }

    void Update()
    {
        // Smooth transition between noise levels
        noisePercentage = Mathf.Lerp(noisePercentage, targetNoiseLevel, Time.deltaTime * smoothSpeed);
        UpdateNoiseBarSprite();

        // Adjust red background intensity based on noise
        float redIntensity = Mathf.Lerp(0f, 1f, noisePercentage);

        float actualRedIntensity = forceMaxBackground ? 1f : Mathf.Lerp(0f, 1f, noisePercentage);
        float actualAlpha = forceMaxBackground ? 1f : Mathf.Lerp(0f, 1f, noisePercentage);
        float actualRadius = forceMaxBackground ? 0.3f : Mathf.Lerp(1f, 0.3f, noisePercentage);

        background.material.SetFloat("_Alpha", actualAlpha);
        background.material.SetFloat("_Radius", actualRadius);

        // Apply the effect only when noise is present
        if (noisePercentage > 0.05f)
        {
            background.color = new Color(1f, 0f, 0f, redIntensity);
            background.gameObject.SetActive(true);
        }
        else
        {
            background.gameObject.SetActive(false);
        }

        // Handle animation frame updates
        if (Time.time >= nextFrameTime)
        {
            currentFrame = (currentFrame + 1) % 4;
            nextFrameTime = Time.time + frameRate;
        }

        // Update the red background intensity
        if (actualRedIntensity > 0.05f)
        {
            background.color = new Color(1f, 0f, 0f, actualRedIntensity);
            background.gameObject.SetActive(true);
        }
        else
        {
            background.gameObject.SetActive(false);
        }
    }

    public void ForceChaseVisuals(bool active)
    {   
        // Force the chase visuals to be active
        forceMaxBackground = active;
        isChasing = active;

        if (!active)
        {
            StopChase();
        }
    }

    public void UpdateNoiseLevel(float noiseLevel)
    {   
        // Update the noise level
        targetNoiseLevel = Mathf.InverseLerp(minNoise, maxNoise, noiseLevel);

        if (targetNoiseLevel >= 1f && !isChasing)
        {
            isChasing = true;
            OnNoiseMaxed?.Invoke();
            StartCoroutine(ChaseWarningAnimation());
        }
    }

    void UpdateNoiseBarSprite()
    {   
        // Update the noise bar sprite based on the noise level
        if (isChasing)
        {
            noiseBarImage.sprite = chaseWarningFrames[currentFrame];
            return;
        }

        // Determine the noise level index
        int levelIndex = Mathf.Clamp(Mathf.FloorToInt(noisePercentage * noiseLevels.Length), 0, noiseLevels.Length - 1);
        noiseBarImage.sprite = noiseLevels[levelIndex][currentFrame];

        // Adjust transparency based on noise level
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

    public void StopChase()
    {   
        // Stop the chase
        isChasing = false;
        noiseBarImage.sprite = level1Frames[0];
    }

    void DespawnEnemyManager()
    {   
        StopChase();
    }
}
