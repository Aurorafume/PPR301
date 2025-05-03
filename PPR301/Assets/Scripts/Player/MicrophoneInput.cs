using UnityEngine;
using System.Linq;
using System.Collections;

public class MicrophoneInput : MonoBehaviour
{   
    [Header("Microphone Settings")]
    [Tooltip("The sample rate for the microphone input")]
    public int sampleRate = 44100;

    [Tooltip("The selected microphone device")]
    public string selectedMic;

    private bool isRecording = false;
    private AudioClip micClip;
    private float[] audioSamples = new float[1024];

    IEnumerator DelayMicReadings()
    {
        yield return new WaitForSeconds(2f);
        isRecording = true;
    }

    void Start()
    {   
        // Check if any microphone devices are available and select the first one
        if (Microphone.devices.Length > 0)
        {
            selectedMic = Microphone.devices[0];
            StartMicrophone();
            StartCoroutine(DelayMicReadings());
        }
        else
        {
            Debug.LogError("No microphone detected!");
        }
    }

    void Update()
    {   
        // If recording, read microphone data
        if (isRecording && micClip)
        {
            ReadMicrophoneData();
        }
    }

    public void StartMicrophone()
    {   
        // Start recording from the selected microphone
        if (isRecording) return;

        if (selectedMic != null)
        {
            micClip = Microphone.Start(selectedMic, true, 10, sampleRate);
            isRecording = true;
            Debug.Log("Microphone recording started...");
        }
    }

    public void StopMicrophone()
    {   
        // Stop recording from the selected microphone
        if (!isRecording) return;

        Microphone.End(selectedMic);
        isRecording = false;
        Debug.Log("Microphone recording stopped.");
    }

    private void ReadMicrophoneData()
    {   
        // Read the microphone data into the audioSamples array
        if (!micClip) return;

        int micPosition = Microphone.GetPosition(selectedMic);
        if (micPosition < audioSamples.Length) return;

        micClip.GetData(audioSamples, micPosition - audioSamples.Length);
    }

    public float GetCurrentNoiseLevel()
    {   
        // Calculate the current noise level in decibels
        float rms = Mathf.Sqrt(audioSamples.Select(x => x * x).Sum() / audioSamples.Length);
        float normalisedRMS = rms / 1.0f;
        float decibelLevel = 20f * Mathf.Log10(normalisedRMS + 1e-10f); // avoid log(0)
        float positiveDecibelLevel = decibelLevel + 65f;

        return Mathf.Max(positiveDecibelLevel, 0f);
    }

    public float GetSmoothedNoiseLevel(float[] buffer)
    {   
        // Calculate the average noise level from the buffer
        float avg = 0f;
        for (int i = 0; i < buffer.Length; i++)
            avg += Mathf.Abs(buffer[i]);
        avg /= buffer.Length;

        float db = 20f * Mathf.Log10(avg + 1e-10f);
        return Mathf.Max(db + 65f, 0f);
    }
}
