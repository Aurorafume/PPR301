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

    // Noise level range
    private float minNoise = -65f;
    private float maxNoise = 0f;

    public void UpdateNoiseLevel(float noiseLevel)
    {
        // Normalise noise level between 0 and 1 for fill amount
        float normalisedNoise = Mathf.InverseLerp(minNoise, maxNoise, noiseLevel);
        noiseBar.fillAmount = normalisedNoise;

        // Set color based on noise level
        if (normalisedNoise < 0.4f)
        {
            noiseBar.color = greenColor;
        }
        else if (normalisedNoise < 0.6f)
        {
            noiseBar.color = yellowColor;
        }
        else
        {
            noiseBar.color = redColor;
        }
    }
}
