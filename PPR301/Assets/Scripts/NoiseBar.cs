using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class NoiseBar : MonoBehaviour
{
    public Image noiseBar;
    public Color greenColor = Color.green;
    public Color yellowColor = Color.yellow;
    public Color redColor = Color.red;
    private float minNoise = 0f;
    private float maxNoise = 100f;
    private float targetNoiseLevel = 0f; 
    private float smoothSpeed = 5f; 

    // Timer
    private float startTime;
    private float timeLimit = 3f;

    // Event
    public event Action OnNoiseMaxed;

    void Start()
    {   
        noiseBar.fillAmount = 0f;
        startTime = Time.time;
    }

    void Update()
    {
        // Smoothly interpolate current noise level towards the target value
        float currentFill = noiseBar.fillAmount;
        float smoothedFill = Mathf.Lerp(currentFill, targetNoiseLevel, Time.deltaTime * smoothSpeed);
        noiseBar.fillAmount = smoothedFill;

        // Set color based on smoothed noise level
        if (smoothedFill < 0.4f)
        {
            noiseBar.color = greenColor;
        }
        else if (smoothedFill < 0.6f)
        {
            noiseBar.color = yellowColor;
        }
        else
        {
            noiseBar.color = redColor;

            // Trigger event when noise level reaches red zone
            OnNoiseMaxed?.Invoke();
        }
    }

    public void UpdateNoiseLevel(float noiseLevel)
    {   
        // Normalise noise level between 0 and 1
        float normalizedNoise = Mathf.InverseLerp(minNoise, maxNoise, noiseLevel);

        // Restrict noise to yellow zone for the first 3 seconds
        if (Time.time - startTime < timeLimit)
        {   
            // Clamp noise level
            normalizedNoise = Mathf.Min(normalizedNoise, 0.59f);
        }

        targetNoiseLevel = normalizedNoise;
    }
}
