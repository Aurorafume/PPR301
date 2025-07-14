using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContactNoise : MonoBehaviour
{
    [SerializeField] float noiseAmount = 10f;
    [SerializeField] float immunityDuration = 0.5f;

    NoiseHandler noiseHandler;
    float lastNoiseTime = 0f;

    void Awake()
    {
        noiseHandler = FindObjectOfType<NoiseHandler>();
    }

    public void GenerateContactNoise()
    {
        if (Time.time - lastNoiseTime < immunityDuration)
        {
            return; // Ignore noise generation if within immunity duration
        }
        
        if (noiseHandler != null)
        {
            noiseHandler.GenerateNoise(noiseAmount);

            lastNoiseTime = Time.time;
        }
    }
}
