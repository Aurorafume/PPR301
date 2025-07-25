// ==========================================================================
// Meowt of Tune - Lingering Light
// Owned and contributed to by: brookcoli (Itch.io) - Aurorafume (GitHub), donkzilla03 (Itch.io) - AliAK03 (GitHub), JaydenFielderTorrens (Itch.io) - Squib35 (GitHub), komorebimoriko (Itch.io), TheRealCrizz (GitHub), LordVGahn (GitHub)
// itch.io listing: https://brookcoli.itch.io/meowt-of-tune - GitHub Repository: https://github.com/Aurorafume/PPR301
// ==========================================================================
//
// WHAT DOES THIS DO:
// This script manages a temporary light source that appears with a particle
// effect. The light's intensity is animated over its lifetime using a
// customisable curve, and the object destroys itself after the effect is
// finished. It's ideal for effects like explosions, magic spells, or temporary
// light sources.
//
// Core functionalities include:
// - Spawning a particle effect at its location.
// - Animating the intensity of a Light component over time.
// - Using an AnimationCurve for full control over the light's fade profile.
// - Automatically cleaning up the GameObject after the effect's duration.
//
// Dependencies:
// - UnityEngine for core and particle system functionality.
// - A Light component on the same GameObject.
//
// ==========================================================================

using UnityEngine;

/// <summary>
/// Manages a temporary light source that fades out over time according to a curve.
/// </summary>
[RequireComponent(typeof(Light))]
public class LingeringLight : MonoBehaviour
{
    [Header("Effect Settings")]
    [Tooltip("The particle system to instantiate when this light is created.")]
    public ParticleSystem explosionEffect;
    [Tooltip("A curve controlling the light's intensity over its lifetime. The X-axis is time (0 to 1) and the Y-axis is the intensity multiplier (0 to 1).")]
    public AnimationCurve lightIntensityCurve;

    // The duration of the light effect, typically matched to the particle effect's lifetime.
    private float duration;
    // The initial intensity of the light component before the curve is applied.
    private float startIntensity;
    // The time when the light effect started.
    private float startTime;
    // A cached reference to the Light component for performance.
    private Light lightComponent;

    /// <summary>
    /// Caches the Light component reference on awake.
    /// </summary>
    void Awake()
    {
        lightComponent = GetComponent<Light>();
    }

    /// <summary>
    /// Called by Unity on startup to initialise the effect.
    /// </summary>
    void Start()
    {
        // Store the original intensity and set the initial intensity based on the curve's start.
        if (lightComponent != null)
        {
            startIntensity = lightComponent.intensity;
            lightComponent.intensity = startIntensity * lightIntensityCurve.Evaluate(0f);
        }

        startTime = Time.time;

        // If an explosion effect is assigned, set the duration from it and play it.
        if (explosionEffect != null)
        {
            duration = explosionEffect.main.startLifetime.constantMax;
            InstantiateExplosionEffect();
        }
    }

    /// <summary>
    /// Spawns the assigned particle effect at this object's position.
    /// </summary>
    void InstantiateExplosionEffect()
    {
        Instantiate(explosionEffect, transform.position, Quaternion.identity);
    }

    /// <summary>
    /// Called every frame by Unity to update the light's intensity over time.
    /// </summary>
    void Update()
    {
        // Prevent division by zero if duration is not set.
        if (duration <= 0) return;

        // Calculate a normalised time value (from 0 to 1) representing the effect's progress.
        float normalizedTime = (Time.time - startTime) / duration;
        UpdateLightIntensity(normalizedTime);
    }

    /// <summary>
    /// Updates the light's intensity based on the normalised time and the animation curve.
    /// </summary>
    /// <param name="t">The normalised time (0 to 1) of the effect's lifetime.</param>
    public void UpdateLightIntensity(float t)
    {
        if (lightComponent != null)
        {
            // Evaluate the curve at the current time to get the intensity multiplier.
            float curveMultiplier = lightIntensityCurve.Evaluate(t);
            lightComponent.intensity = startIntensity * curveMultiplier;

            // Destroy the light object after its duration has passed.
            if (t >= 1f)
            {
                Destroy(gameObject);
            }
        }
    }
}