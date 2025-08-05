// ==========================================================================
// Meowt of Tune - Buttons
// Owned and contributed to by: brookcoli (Itch.io) - Aurorafume (GitHub), donkzilla03 (Itch.io) - AliAK03 (GitHub), JaydenFielderTorrens (Itch.io) - Squib35 (GitHub), komorebimoriko (Itch.io), TheRealCrizz (GitHub), LordVGahn (GitHub)
// itch.io listing: https://brookcoli.itch.io/meowt-of-tune - GitHub Repository: https://github.com/Aurorafume/PPR301
// ==========================================================================
//
// WHAT DOES THIS DO:
// This script is a persistent singleton manager for UI buttons and background
// music. It ensures that music playback and volume settings are maintained
// across different scenes. It handles volume control via a UI slider, a
// mute/unmute toggle, changing tracks, and executing actions like loading
// new game scenes when buttons are clicked.
//
// Core functionalities include:
// - A singleton pattern with DontDestroyOnLoad to persist between scenes.
// - Managing a playlist of audio tracks.
// - Controlling music volume via a UI slider.
// - Muting/unmuting functionality that remembers the last volume level.
// - Cycling through the available music tracks.
// - Updating UI visuals (button sprites, animators) to reflect the audio state.
// - Public methods for UI buttons to call to load different game scenes.
// - Re-hooking UI references automatically after a scene loads to maintain
//   functionality.
//
// Dependencies:
// - Requires UI elements in each scene with specific names (e.g. "Slider",
//   "Stylus") for re-hooking functionality.
// - Relies on a 'States' script for resetting the game state before loading a level.
// - The 'musicList' must be populated with AudioSource components in the Inspector.
//
// ==========================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// A persistent singleton that manages music, volume, and main menu button actions.
/// </summary>
public class Buttons : MonoBehaviour
{
    [Header("Component References")]
    [Tooltip("Reference to the main game state manager.")]
    public States states;
    [Tooltip("The UI Image for the audio button, to swap its sprite.")]
    public Image audioButton;
    public Image audioButton2;
    [Tooltip("The animator for the record player stylus.")]
    public Animator stylusAnimator;
    [Tooltip("The UI Slider for controlling music and audio volume.")]
    public Slider musicSlider;
    public Slider audioSlider;

    [Header("Music & Audio")]
    [Tooltip("The index of the currently playing track in the musicList.")]
    public int musicIndex;
    [Tooltip("A list of all available music tracks (AudioSource components).")]
    public List<AudioSource> musicList = new List<AudioSource>();
    [Tooltip("The sprite to display when audio is enabled.")]
    public Sprite audioUI;
    [Tooltip("The sprite to display when audio is muted.")]
    public Sprite mutedUI;

    public Sprite audioUI2;
    public Sprite mutedUI2;

    private float volumeOverrideMultiplier = 1f;
    private Coroutine fadeMusicCoroutine;

    // --- Private State Variables ---
    private bool mute; // Tracks if the music is currently muted.
    public bool mute2; // Tracks if the audio is currently muted.
    private float lastValue; // Stores the volume level before muting.
    private float lastValue2; // Stores the volume level before muting.

    /// <summary>
    /// Implements the singleton pattern to ensure only one instance of this manager exists.
    /// </summary>
    void Awake()
    {
        // If more than one instance of this script exists, destroy the new one.
        if (FindObjectsOfType<Buttons>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        // Mark this instance to not be destroyed when loading new scenes.
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Subscribes to the sceneLoaded event when this object is enabled.
    /// </summary>
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    /// <summary>
    /// Unsubscribes from the sceneLoaded event when this object is disabled.
    /// </summary>
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// Initialises music and UI elements when the script first starts.
    /// </summary>
    void Start()
    {
        // Play the initial music track if available.
        if (musicList.Count > musicIndex)
        {
            musicSlider.value = musicList[musicIndex].volume;
            audioSlider.value = 1;
            lastValue = musicList[musicIndex].volume;
            musicList[musicIndex].Play();
        }

        // Configure the vinyl record image to ignore clicks on its transparent areas.
        Image img = GameObject.Find("Logo Vynyl")?.GetComponent<Image>();
        if (img != null)
        {
            img.alphaHitTestMinimumThreshold = 0.5f;
        }
    }

    /// <summary>
    /// This method is called every time a new scene is loaded. It re-acquires
    /// references to scene-specific UI elements to ensure functionality is maintained.
    /// </summary>
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Re-find UI elements in the newly loaded scene.
        musicSlider = GameObject.Find("Slider")?.GetComponent<Slider>();
        audioSlider = GameObject.Find("Slider 2")?.GetComponent<Slider>();

        stylusAnimator = GameObject.Find("Stylus")?.GetComponent<Animator>();

        audioButton = GameObject.Find("Music Icon")?.GetComponent<Image>();
        audioButton2 = GameObject.Find("Audio Icon")?.GetComponent<Image>();
        states = FindObjectOfType<States>();

        // Re-configure the volume slider.
        if (musicSlider != null)
        {
            if (musicList.Count > musicIndex)
            {
                musicSlider.value = lastValue;
            }

            // Re-assign the listener to prevent errors from old scene references.
            musicSlider.onValueChanged.RemoveAllListeners();
            musicSlider.onValueChanged.AddListener(SetVolume);
        }
        // Re-configure the audio slider.
        if (audioSlider != null)
        {
            audioSlider.value = lastValue2;


            // Re-assign the listener to prevent errors from old scene references.
            audioSlider.onValueChanged.RemoveAllListeners();
            audioSlider.onValueChanged.AddListener(SetAudioVolume);
        }

        // Re-assign button listeners.
        Button stylusButton = GameObject.Find("Stylus")?.GetComponent<Button>();
        if (stylusButton != null)
        {
            stylusButton.onClick.RemoveAllListeners();
            stylusButton.onClick.AddListener(MuteMusic);
        }
        Button audioB = GameObject.Find("Music Icon")?.GetComponent<Button>();
        if (audioB != null)
        {
            audioB.onClick.RemoveAllListeners();
            audioB.onClick.AddListener(MuteMusic);
        }
        Button audioB2 = GameObject.Find("Audio Icon")?.GetComponent<Button>();
        if (audioB2 != null)
        {
            audioB2.onClick.RemoveAllListeners();
            audioB2.onClick.AddListener(MuteAudio);
        }
        Button recordButton = GameObject.Find("Logo Vynyl")?.GetComponent<Button>();
        if (recordButton != null)
        {
            recordButton.onClick.RemoveAllListeners();
            recordButton.onClick.AddListener(ChangeMusic);
        }
    }

    /// <summary>
    /// Public method for UI buttons to load the main demo scene after resetting the game state.
    /// </summary>
    public void PlayDemo()
    {
        if (states != null)
        {
            states.ResetGameState();
        }
        SceneManager.LoadScene("MainDemoA3");
    }

    /// <summary>
    /// Public method for UI buttons to load the second demo scene.
    /// </summary>
    public void PlayDemo2()
    {
        SceneManager.LoadScene("Demo_2");
    }

    /// <summary>
    /// Sets the music volume. This is intended to be called by the UI slider's OnValueChanged event.
    /// </summary>
    /// <param name="volume">The new volume level, from 0.0 to 1.0.</param>
    public void SetVolume(float volume)
    {
        if (musicList.Count > musicIndex)
        {
            musicList[musicIndex].volume = volume * volumeOverrideMultiplier;
        }

        // Update the mute state and visuals based on the new volume.
        if (volume == 0)
        {
            UpdateMuteState(true);
        }
        else
        {
            lastValue = volume;
            UpdateMuteState(false);
        }
    }
    public void SetAudioVolume(float volume)
    {
        //audioSlider.volume = volume * volumeOverrideMultiplier;
        if (volume == 0)
        {
            UpdateAudioIcon(true);
        }
        else
        {
            lastValue2 = volume;
            UpdateAudioIcon(false);
        }
    }

    /// <summary>
    /// Cycles to the next music track in the playlist.
    /// </summary>
    public void ChangeMusic()
    {
        if (mute || musicList.Count == 0) return;

        // Stop the current track.
        musicList[musicIndex].Stop();

        // Increment the index, wrapping around to the start if at the end of the list.
        musicIndex = (musicIndex + 1) % musicList.Count;

        // Play the new track with the current volume setting.
        musicList[musicIndex].volume = musicSlider.value;
        musicList[musicIndex].Play();
    }

    /// <summary>
    /// Toggles the music mute state.
    /// </summary>
    public void MuteMusic()
    {
        // If not muted, mute the music.
        if (!mute)
        {
            // Store the current volume before setting it to 0.
            lastValue = musicSlider.value;
            musicSlider.value = 0;
            UpdateMuteState(true);
        }
        // If already muted, unmute the music.
        else
        {
            // Restore the volume to its last known value (if it was greater than 0).
            if (lastValue > 0)
            {
                musicSlider.value = lastValue;
            }
            // If the last value was 0, set it to a default audible level.
            else
            {
                musicSlider.value = 0.5f;
            }
            UpdateMuteState(false);
        }
    }
    public void MuteAudio()
    {
        // If not muted, mute the audio.
        if (!mute2)
        {
            //mute2 = true;
            lastValue2 = audioSlider.value;
            audioSlider.value = 0;
            UpdateAudioIcon(true);
        }
        // If already muted, unmute the audio.
        else
        {
            //mute2 = false;
            if (lastValue2 > 0)
            {
                audioSlider.value = lastValue2;
            }
            // If the last value was 0, set it to a default audible level.
            else
            {
                audioSlider.value = 0.5f;
            }
            UpdateAudioIcon(false);
        }
    }

    /// <summary>
    /// Updates the mute flag and all related UI visuals.
    /// </summary>
    /// <param name="isMuted">The new mute state.</param>
    private void UpdateMuteState(bool isMuted)
    {
        mute = isMuted;
        if (audioButton != null)
        {
            audioButton.sprite = isMuted ? mutedUI : audioUI;
        }
        if (stylusAnimator != null)
        {
            stylusAnimator.SetBool("Mute", isMuted);
        }
    }
    private void UpdateAudioIcon(bool isMuted)
    {
        mute2 = isMuted;
        audioButton2.sprite = isMuted ? mutedUI2 : audioUI2;
    }

    public void FadeMusic(float target, float fadeSpeed)
    {
        Debug.Log("Fading Music");
        if (fadeMusicCoroutine != null)
        {
            StopCoroutine(fadeMusicCoroutine);
            fadeMusicCoroutine = null;
        }
        fadeMusicCoroutine = StartCoroutine(FadeMusicCoroutine(target, fadeSpeed));
    }

    IEnumerator FadeMusicCoroutine(float target, float fadeSpeed)
    {
        while (volumeOverrideMultiplier != target)
        {
            volumeOverrideMultiplier = Mathf.MoveTowards(volumeOverrideMultiplier, target, fadeSpeed * Time.deltaTime);
            SetVolume(lastValue);

            yield return new WaitForEndOfFrame();
        }

        volumeOverrideMultiplier = target;
        SetVolume(lastValue);
        fadeMusicCoroutine = null;
    }
}