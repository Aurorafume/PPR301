using System.Collections;
using UnityEngine;

public class AmbientNoise : MonoBehaviour
{
    /*[Header("Calibration Settings")]
    [Tooltip("How long to sample ambient mic noise on startup")]
    public float calibrationDuration = 5f;

    [Tooltip("Delay between mic samples during calibration")]
    public float sampleInterval = 0.1f;

    [Tooltip("The baseline ambient noise level in dB")]
    public float ambientNoiseBaseline = 0f;

    [Header("Calibration State")]
    [Tooltip("Is the ambient noise calibrated?")]
    public bool isCalibrated = false;

    private MicrophoneInput microphoneInput;

    void Start()
    {   // Initialise the microphone input component and start calibration
        microphoneInput = GetComponent<MicrophoneInput>();
        if (microphoneInput == null)
        {
            Debug.LogError("AmbientNoise: No MicrophoneInput component found on this GameObject.");
            return;
        }

        StartCoroutine(CalibrateAmbientNoise());
    }

    IEnumerator CalibrateAmbientNoise()
    {   
        // Wait for the microphone to be ready before starting calibration
        Debug.Log("Starting ambient noise calibration");
        float sum = 0f;
        int sampleCount = 0;
        float startTime = Time.time;
        
        while (Time.time < startTime + calibrationDuration)
        {
            float currentNoise = microphoneInput.GetCurrentNoiseLevel();
            sum += currentNoise;
            sampleCount++;
            yield return new WaitForSeconds(sampleInterval);
        }

        // Calculate the average noise level and set it as the baseline
        ambientNoiseBaseline = (sampleCount > 0) ? sum / sampleCount : 0f;
        isCalibrated = true;
        Debug.Log($"[Calibrated] Ambient Noise Baseline: {ambientNoiseBaseline} dB");
    }

    // Helper to normalise live mic input based on calibration
    public float GetNormalizedNoise(float currentNoise, float margin)
    {
        float adjusted = Mathf.Max(currentNoise - ambientNoiseBaseline, 0f);
        return Mathf.Clamp01(adjusted / margin);
    }*/
}
