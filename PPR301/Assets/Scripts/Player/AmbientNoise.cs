using System.Collections;
using UnityEngine;

public class AmbientNoise : MonoBehaviour
{
    // Calibration settings
    public float calibrationDuration = 5f;
    public float sampleInterval = 0.1f;

    // Ambient noise level
    public float ambientNoiseBaseline = 0f;
    public bool isCalibrated = false;

    private MicrophoneInput microphoneInput;

    void Start()
    {
        microphoneInput = GetComponent<MicrophoneInput>();
        if (microphoneInput == null)
        {
            Debug.LogError("AmbientNoise: No MicrophoneInput component found on this GameObject.");
            return;
        }
        
        // Start calibration
        StartCoroutine(CalibrateAmbientNoise());
    }

    IEnumerator CalibrateAmbientNoise()
    {
        Debug.Log("Starting ambient noise calibration");
        float sum = 0f;
        int sampleCount = 0;
        float startTime = Time.time;

        // Sample noise for the duration of calibrationDuration
        while (Time.time < startTime + calibrationDuration)
        {
            float currentNoise = microphoneInput.GetCurrentNoiseLevel();
            sum += currentNoise;
            sampleCount++;
            yield return new WaitForSeconds(sampleInterval);
        }
        
        // Calculate average ambient noise level
        ambientNoiseBaseline = (sampleCount > 0) ? sum / sampleCount : 0f;
        isCalibrated = true;
        Debug.Log("Ambient noise calibrated: " + ambientNoiseBaseline);
    }
}
