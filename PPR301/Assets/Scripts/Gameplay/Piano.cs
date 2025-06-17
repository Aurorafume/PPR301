using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piano : MonoBehaviour
{
    [Tooltip("Assign 10 audio clips for the piano keys (1-9, then 0).")]
    public AudioClip[] keySounds;

    /*[Tooltip("Assign 10 animators for the piano key GameObjects.")]
    public Animator[] keyAnimators;*/

    [Tooltip("The AudioSource to play the key sounds from. Will get it from the GameObject if not assigned.")]
    public AudioSource audioSource;

    /*[Tooltip("The name of the trigger in the Animator Controller to play the key press animation.")]
    public string animationTriggerName = "Play";*/

    void Start()
    {
        // If an AudioSource is not assigned in the Inspector, try to get it from this GameObject.
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            // If there is still no AudioSource, add one.
            if (audioSource == null)
            {
                Debug.LogWarning("No AudioSource found. Adding one to this GameObject.");
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        // Check that the arrays are assigned correctly to avoid errors.
        if (keySounds.Length != 10)
        {
            Debug.LogError("Error: The 'keySounds' array must contain exactly 10 audio clips.");
        }

       /* if (keyAnimators.Length != 10)
        {
            Debug.LogError("Error: The 'keyAnimators' array must contain exactly 10 animators.");
        }*/
    }

    void Update()
    {
        // Loop through all our defined keys.
        for (int i = 0; i < _keys.Length; i++)
        {
            // Check if the key was pressed down in this frame.
            if (Input.GetKeyDown(_keys[i]))
            {
                // If the key was pressed, play the corresponding sound and animation.
                PlayKey(i);
            }
        }
    }

    // An array to hold the KeyCode for each of our piano keys.
    private readonly KeyCode[] _keys =
    {
        KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3,
        KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6,
        KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9, KeyCode.Alpha0
    };
    
    public void PlayKey(int keyIndex)
    {
        // Check if the provided index is valid.
        if (keyIndex < 0 || keyIndex >= 10)
        {
            Debug.LogError("Error: Invalid key index provided. Must be between 0 and 9.");
            return;
        }

        // Check if there is a sound assigned for this key.
        if (keySounds[keyIndex] != null)
        {
            // Play the sound using the assigned AudioSource.
            // PlayOneShot allows multiple sounds to play over each other.
            audioSource.PlayOneShot(keySounds[keyIndex]);
        }
        else
        {
            Debug.LogWarning($"Warning: No audio clip assigned to key index {keyIndex}.");
        }

        /*// Check if there is an animator assigned for this key.
        if (keyAnimators[keyIndex] != null)
        {
            // Trigger the animation.
            keyAnimators[keyIndex].SetTrigger(animationTriggerName);
        }
        else
        {
            Debug.LogWarning($"Warning: No animator assigned to key index {keyIndex}.");
        }*/
    }
}
