// ==========================================================================
// Meowt of Tune - Noise Bar
// Owned and contributed to by: brookcoli (Itch.io) - Aurorafume (GitHub), donkzilla03 (Itch.io) - AliAK03 (GitHub), JaydenFielderTorrens (Itch.io) - Squib35 (GitHub), komorebimoriko (Itch.io), TheRealCrizz (GitHub), LordVGahn (GitHub)
// itch.io listing: https://brookcoli.itch.io/meowt-of-tune - GitHub Repository: https://github.com/Aurorafume/PPR301-tune
// ==========================================================================
//
// WHAT DOES THIS DO:
// This script manages the player's noise level visualisation using a multi-level
// animated noise bar. It responds to gameplay events like movement or light-triggered
// noise, and updates both the visual bar and UI background to reflect how close the
// player is to triggering a chase.
//
// Core functionalities include:
// - Smooth animation of noise levels using frame-based sprite switching.
// - Different visual states for escalating noise intensity (6 levels).
// - Triggering a chase warning and spawning enemies when max noise is hit.
// - Flashing red background for tension and visual feedback.
// - Optional "forced" visual override when enemies are actively chasing.
//
// Dependencies:
// - UnityEngine.UI for handling UI elements.
// - Custom 'States' and 'EnemySpawning' scripts for gameplay logic.
//
// ==========================================================================

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NoiseBar : MonoBehaviour
{
    [Header("Noise Bar Settings")]
    public Image noiseBarImage;                        // Main noise bar sprite display
    public Image background;                           // Red warning background

    public Sprite[] level1Frames;                      // Sprite frames
    public Sprite[] level2Frames;                      // Sprite frames
    public Sprite[] level3Frames;                      // Sprite frames
    public Sprite[] level4Frames;                      // Sprite frames
    public Sprite[] level5Frames;                      // Sprite frames
    public Sprite[] level6Frames;                      // Sprite frames
    public Sprite[] chaseWarningFrames;                // Animated sprites for chase state

    private Sprite[][] noiseLevels;                    // Array holding all level frame sets
    private int currentFrame = 0;                      // Index for sprite animation frames
    private float frameRate = 0.15f;                   // Time between frame changes
    private float nextFrameTime;                       // Timestamp for next frame switch
    private bool forceMaxBackground = false;           // Whether to show max warning visuals

    [Header("Noise Level Settings")]
    private float noisePercentage = 0f;                // Current noise percentage (0–1)
    private float targetNoiseLevel = 0f;               // Desired noise level (used for smoothing)
    private float smoothSpeed = 5f;                    // Lerp speed for smoothing noise
    private bool isChasing = false;                    // Whether the player is being chased

    public event System.Action OnNoiseMaxed;           // Event fired when noise reaches max

    [Header("References")]
    public States states;                              // Global game state tracker
    public EnemySpawning enemySpawning;                // Enemy spawner script reference

    public float CurrentNoisePercentage => noisePercentage;

    void Start()
    {
        // Get references and initialise noise bar
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
        // --- START MANUAL TRIGGER TEST ---
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.LogWarning("MANUAL SPAWN TRIGGERED! Firing OnNoiseMaxed event now...");
            OnNoiseMaxed?.Invoke();
        }
        // --- END MANUAL TRIGGER TEST ---
    
        // Smoothly interpolate current noise towards target
        noisePercentage = Mathf.Lerp(noisePercentage, targetNoiseLevel, Time.deltaTime * smoothSpeed);

        UpdateNoiseBarSprite();

        // Adjust red background intensity and size based on noise level
        float redIntensity = Mathf.Lerp(0f, 1f, noisePercentage);
        float actualRedIntensity = forceMaxBackground ? 1f : redIntensity;
        float actualAlpha = forceMaxBackground ? 1f : redIntensity;
        float actualRadius = forceMaxBackground ? 0.3f : Mathf.Lerp(1f, 0.3f, noisePercentage);

        if (background.material != null)
        {
            background.material.SetFloat("_Alpha", actualAlpha);
            background.material.SetFloat("_Radius", actualRadius);
        }

        // Toggle red background visibility based on noise level
        if (noisePercentage > 0.05f)
        {
            background.color = new Color(1f, 0f, 0f, actualRedIntensity);
            background.gameObject.SetActive(true);
        }
        else
        {
            background.gameObject.SetActive(false);
        }

        // Advance frame for animated sprites
        if (Time.time >= nextFrameTime)
        {
            currentFrame = (currentFrame + 1) % 4;
            nextFrameTime = Time.time + frameRate;
        }
    }

    /// <summary>
    /// Called by external objects to update the current noise value.
    /// Triggers a chase if noise reaches maximum.
    /// </summary>
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
            // Prevent spawning without valid spawn point
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

    /// <summary>
    /// Updates the visual sprite for the noise bar based on the current level.
    /// </summary>
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

    /// <summary>
    /// Coroutine to animate the noise bar during chase mode.
    /// Loops until chase state is turned off.
    /// </summary>
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

    /// <summary>
    /// Manually enables/disables chase visuals (e.g. when enemy is spawned).
    /// </summary>
    public void ForceChaseVisuals(bool active)
    {
        forceMaxBackground = active;
        isChasing = active;

        if (!active)
        {
            StopChase();
        }
    }

    /// <summary>
    /// Resets the chase state and visuals to the default noise bar.
    /// </summary>
    public void StopChase()
    {
        isChasing = false;
        if (level1Frames.Length > 0)
        {
            noiseBarImage.sprite = level1Frames[0];
        }
    }

    /// <summary>
    /// Forces the noise bar to spike to full and trigger a chase.
    /// Used by light interactions or scripted events.
    /// </summary>
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
