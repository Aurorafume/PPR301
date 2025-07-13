using UnityEngine;

public class LingeringLight : MonoBehaviour
{
    public ParticleSystem explosionEffect;
    public AnimationCurve lightIntensityCurve;

    float duration;
    float startIntensity;
    float startTime;

    void Start()
    {
        // Set the initial intensity of the light based on the curve
        Light lightComponent = GetComponent<Light>();

        if (lightComponent != null)
        {
            startIntensity = lightComponent.intensity;
            lightComponent.intensity = startIntensity * lightIntensityCurve.Evaluate(0f);
        }

        startTime = Time.time;

        if (explosionEffect != null)
        {
            duration = explosionEffect.main.startLifetime.constantMax;
            InstantiateExplosionEffect();
        }
    }

    void InstantiateExplosionEffect()  
    {
        Instantiate(explosionEffect, transform.position, Quaternion.identity);
    }

    void Update()
    {
        // Update the light intensity over time
        float t = (Time.time - startTime) / duration; // Normalize time to [0, 1]
        UpdateLightIntensity(t);
    }

    // Method to update the light intensity over time
    public void UpdateLightIntensity(float t)
    {
        Light lightComponent = GetComponent<Light>();
        if (lightComponent != null)
        {
            // Evaluate the curve at the given time to get the intensity
            lightComponent.intensity = startIntensity * lightIntensityCurve.Evaluate(t);
            // Destroy the light after the duration has passed
            if (t >= 1f)
            {
                Destroy(gameObject);
            }
        }
    }
}
