using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class MicrophoneInput : MonoBehaviour
{
    public AudioSource audioSource;
    public int sampleRate = 44100;
    public string selectedMic;
    // private float volumeLevel = 0f;

    private bool isRecording = false;
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

        // Update audio visualisation
        if (audioSource.isPlaying)
        {
            audioSource.GetOutputData(audioSamples, 0);
        }
    }

    public void StartMicrophone()
    {   
        // Check if already recording
        if (isRecording) return;

        // Start recording using the selected microphone
        if (selectedMic != null)
        {
            audioSource.clip = Microphone.Start(selectedMic, true, 10, sampleRate);
            audioSource.loop = true;
            while (!(Microphone.GetPosition(selectedMic) > 0)) { }
            audioSource.Play();
            isRecording = true;
            Debug.Log("Microphone recording started...");
        }
    }

    public void StopMicrophone()
    {
        // Check if not recording
        if (!isRecording) return;

        // Stop recording and playback
        Microphone.End(selectedMic);
        audioSource.Stop();
        isRecording = false;
        Debug.Log("Microphone recording stopped.");
    }

    /* Uncomment if u want to add audio visualisation (but I dont think we need this tbh)
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
    }

    public float GetCurrentNoiseLevel()
    {   
        // Compute RMS (Root Mean Square) volume level and convert to dB
        float rms = Mathf.Sqrt(audioSamples.Select(x => x * x).Sum() / audioSamples.Length);
        float normalisedRMS = rms / 1.0f; 
        float decibelLevel = 20f * Mathf.Log10(normalisedRMS + 1e-10f); 

        // Ensure positive values by offsetting
        float positiveDecibelLevel = decibelLevel + 65f;

        // Return the noise level and ensure it's not negative
        return Mathf.Max(positiveDecibelLevel, 0f);
    }
}
