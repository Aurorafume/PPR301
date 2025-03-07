using UnityEngine;
using System.Linq;

public class MicrophoneInput : MonoBehaviour
{
    public int sampleRate = 44100;
    public string selectedMic;
    private bool isRecording = false;
    private AudioClip micClip;
    private float[] audioSamples = new float[1024];

    void Start()
    {
        // Get the default microphone
        if (Microphone.devices.Length > 0)
        {
            selectedMic = Microphone.devices[0];
        }
        else
        {
            Debug.LogError("No microphone detected!");
        }
    }

    void Update()
    {
        // Start recording when the user presses "R"
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartMicrophone();
        }

        // Stop recording when the user presses "T"
        if (Input.GetKeyDown(KeyCode.T))
        {
            StopMicrophone();
        }

        // Continuously read microphone data if recording
        if (isRecording && micClip)
        {
            ReadMicrophoneData();
        }
    }

    public void StartMicrophone()
    {   
        // Prevent starting multiple recordings
        if (isRecording) return;

        // Start recording with the selected microphone
        if (selectedMic != null)
        {
            micClip = Microphone.Start(selectedMic, true, 10, sampleRate);
            isRecording = true;
            Debug.Log("Microphone recording started...");
        }
    }

    public void StopMicrophone()
    {   
        // Prevent stopping if not recording
        if (!isRecording) return;

        // Stop recording and reset flag
        Microphone.End(selectedMic);
        isRecording = false;
        Debug.Log("Microphone recording stopped.");
    }

    private void ReadMicrophoneData()
    {   
        // Prevent reading data if clip is null
        if (!micClip) return;

        // Get the current position of the microphone recording
        int micPosition = Microphone.GetPosition(selectedMic);
        if (micPosition < audioSamples.Length) return;

        micClip.GetData(audioSamples, micPosition - audioSamples.Length);
    }

    public float GetCurrentNoiseLevel()
    {   
        // Compute RMS (Root Mean Square) volume level and convert to dB
        float rms = Mathf.Sqrt(audioSamples.Select(x => x * x).Sum() / audioSamples.Length);
        float normalisedRMS = rms / 1.0f; 
        float decibelLevel = 20f * Mathf.Log10(normalisedRMS + 1e-10f); 

        // Ensure positive values by offsetting
        float positiveDecibelLevel = decibelLevel + 65f;

        return Mathf.Max(positiveDecibelLevel, 0f);
    }
}
