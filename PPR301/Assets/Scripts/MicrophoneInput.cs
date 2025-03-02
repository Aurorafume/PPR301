using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class MicrophoneInput : MonoBehaviour
{
    public AudioSource audioSource; // Assign an AudioSource in the Inspector
    public int sampleRate = 44100; // Common sample rates: 44100, 48000
    public string selectedMic; // Stores the selected microphone
    private float volumeLevel = 0f; // Stores current volume level

    private bool isRecording = false;
    private float[] audioSamples = new float[1024]; // For visualisation

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

        // Stop recording when the user presses "S"
        if (Input.GetKeyDown(KeyCode.S))
        {
            StopMicrophone();
        }

        // Update audio visualisation
        if (audioSource.isPlaying)
        {
            audioSource.GetOutputData(audioSamples, 0);
        }
    }

    public void StartMicrophone()
    {
        if (isRecording) return;

        if (selectedMic != null)
        {
            audioSource.clip = Microphone.Start(selectedMic, true, 10, sampleRate);
            audioSource.loop = true;
            while (!(Microphone.GetPosition(selectedMic) > 0)) { } // Wait for mic to start
            audioSource.Play();
            isRecording = true;
            Debug.Log("Microphone recording started...");
        }
    }

    public void StopMicrophone()
    {
        if (!isRecording) return;

        Microphone.End(selectedMic);
        audioSource.Stop();
        isRecording = false;
        Debug.Log("Microphone recording stopped.");
    }

    /* Uncomment if you want audio visualisation
    // Draw waveform visualisation using GL lines
    private void OnRenderObject()
    {
        if (audioSamples == null || audioSamples.Length == 0)
            return;

        GL.PushMatrix();
        GL.LoadPixelMatrix();
        GL.Begin(GL.LINES);
        GL.Color(Color.green);

        for (int i = 0; i < audioSamples.Length - 1; i++)
        {
            float x1 = i * (Screen.width / (float)audioSamples.Length);
            float y1 = Screen.height / 2 + audioSamples[i] * 100;
            float x2 = (i + 1) * (Screen.width / (float)audioSamples.Length);
            float y2 = Screen.height / 2 + audioSamples[i + 1] * 100;

            GL.Vertex3(x1, y1, 0);
            GL.Vertex3(x2, y2, 0);
        }

        GL.End();
        GL.PopMatrix();
    }*/

    private void OnGUI()
    {
        GUI.color = Color.white;
        
        // Compute RMS (Root Mean Square) volume level
        float rms = Mathf.Sqrt(audioSamples.Select(x => x * x).Sum() / audioSamples.Length);

        // Normalise RMS against the full scale (1.0) to avoid extremely negative dB values
        float normalisedRMS = rms / 1.0f; // Unity audio is between -1.0 and 1.0

        // Convert to dB (ensure we never take log(0) by adding a small offset)
        float decibelLevel = 20f * Mathf.Log10(normalisedRMS + 1e-10f); 

        // Display the volume level
        GUI.Label(new Rect(10, 10, 300, 20), $"Volume Level: {decibelLevel:F2} dB");
    }
}
