using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseHandler : MonoBehaviour
{
    public float jumpNoise = 5f; 
    public float collisionNoise = 10f;
    public float voiceNoiseThreshold = -30f;

    public PlayerMovement playerMovement;
    private MicrophoneInput microphoneInput;
    public NoiseBar noiseBar; // Reference to UI noise bar

    void Start()
    {
        microphoneInput = GetComponent<MicrophoneInput>();

        if (microphoneInput == null)
        {
            Debug.LogError("MicrophoneInput component is missing!");
        }
    }

    void Update()
    {
        if (microphoneInput != null)
        {
            float currentNoise = microphoneInput.GetCurrentNoiseLevel();
            noiseBar.UpdateNoiseLevel(currentNoise); // Update the bar UI

            if (currentNoise > voiceNoiseThreshold)
            {
                GenerateNoise(currentNoise);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Object"))
        {
            GenerateNoise(collisionNoise);
        }
    }

    public void GenerateNoise(float amount)
    {
        Debug.Log($"Noise generated: {amount} dB");

        if (amount > voiceNoiseThreshold)
        {
            Debug.Log("Alert: Player noise detected!");
        }
    }
}
