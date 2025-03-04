using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoiseBar : MonoBehaviour
{
    public Image noiseBar; // Assign the UI Image in the inspector
    public Color greenColor = Color.green;
    public Color yellowColor = Color.yellow;
    public Color redColor = Color.red;

    private float minNoise = -40f; // Minimum dB
    private float maxNoise = 0f;   // Maximum dB

    public void UpdateNoiseLevel(float noiseLevel)
    {
        // Normalize noise level between 0 and 1 for fill amount
        float normalizedNoise = Mathf.InverseLerp(minNoise, maxNoise, noiseLevel);
        noiseBar.fillAmount = normalizedNoise;

        // Set color based on noise level
        if (normalizedNoise < 0.4f) // Green zone
        {
            noiseBar.color = greenColor;
        }
        else if (normalizedNoise < 0.7f) // Yellow zone
        {
            noiseBar.color = yellowColor;
        }
        else // Red zone
        {
            noiseBar.color = redColor;
        }
    }
}
