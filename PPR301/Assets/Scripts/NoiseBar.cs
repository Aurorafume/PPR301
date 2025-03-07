using System.Collections;
using System.Collections.Generic;
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
        }
    }

    public void UpdateNoiseLevel(float noiseLevel)
    {
        // Normalise noise level between 0 and 1
        targetNoiseLevel = Mathf.InverseLerp(minNoise, maxNoise, noiseLevel);
    }
}
