using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseHandler : MonoBehaviour
{
    // Noise parameters
    public float jumpNoise = -5f; 
    public float collisionNoise = -10f;
    public float voiceNoiseThreshold = -30f;

    // References
    public PlayerMovement playerMovement;
    private MicrophoneInput microphoneInput;
    public NoiseBar noiseBar;

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
        // Check if MicrophoneInput component is not null
        if (microphoneInput != null)
        {
            float currentNoise = microphoneInput.GetCurrentNoiseLevel();
            noiseBar.UpdateNoiseLevel(currentNoise);

            if (currentNoise > voiceNoiseThreshold)
            {
                GenerateNoise(currentNoise);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {   
        // Check if collision object is tagged as "Object"
        if (collision.gameObject.CompareTag("Object"))
        {
            GenerateNoise(collisionNoise);
        }
    }

    public void GenerateNoise(float amount)
    {   
        // Debug noise generated
        Debug.Log($"Noise generated: {amount} dB");

        // Check if noise amount exceeds voice noise threshold (Debugging)
        if (amount > voiceNoiseThreshold)
        {
            Debug.Log("Alert: Player noise detected!");
            Debug.Log("Total noise level: " + amount + " dB");
        }
    }
}
