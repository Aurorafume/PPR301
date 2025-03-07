using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseHandler : MonoBehaviour
{
    // Noise parameters
    public float jumpNoise = 5f; 
    public float collisionNoise = 10f;
    public float voiceNoiseThreshold;

    // References
    public PlayerMovement playerMovement;
    private MicrophoneInput microphoneInput;
    public NoiseBar noiseBar;

    private float additionalNoise = 0f; // Stores extra noise from jumps & collisions

    void Start()
    {   
        microphoneInput = GetComponent<MicrophoneInput>();

        // Check if MicrophoneInput component is missing
        if (microphoneInput == null)
        {
            Debug.LogError("MicrophoneInput component is missing!");
        }
    }

    void Update()
    {   
        if (microphoneInput != null)
        {
            // Get current noise level from mic
            float micNoise = microphoneInput.GetCurrentNoiseLevel();
            
            // Combine mic noise with accumulated noise
            float totalNoise = micNoise + additionalNoise;

            // Debug log for tracking
            Debug.Log($"Mic Noise: {micNoise}, Additional Noise: {additionalNoise}, Total Noise: {totalNoise}");

            // Update noise bar
            noiseBar.UpdateNoiseLevel(totalNoise);

            // Decay additional noise over time (simulating noise fading)
            additionalNoise = Mathf.Lerp(additionalNoise, 0, Time.deltaTime * 0.5f);
        }
    }

    public void GenerateNoise(float extraNoise)
    {   
        additionalNoise += Mathf.Abs(extraNoise); // Accumulate noise instead of replacing

        // Debugging
        Debug.Log($"New Noise Added: {extraNoise}, Total Additional Noise: {additionalNoise}");
    }
}
