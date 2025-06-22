using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piano : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Assign 10 audio clips. Element 0 = 1 key, Element 1 = 2 key, ..., Element 9 = 0 key.")]
    public AudioClip[] keySounds;

    /*[Tooltip("Assign 10 animators. Element 0 = 1 key, Element 1 = 2 key, ..., Element 9 = 0 key.")]
    public Animator[] keyAnimators;*/

    [Tooltip("The AudioSource to play the key sounds from. Will get it from the GameObject if not assigned.")]
    public AudioSource audioSource;

    /*[Tooltip("The name of the trigger in the Animator Controller to play the key press animation.")]
    public string animationTriggerName = "Play";*/
    
    // This variable is set by the GhostCatController, so we hide it from the Inspector.
    [HideInInspector] 
    public GhostCatController ghostCatController;

    // A dictionary to explicitly map a KeyCode to its array index.
    private Dictionary<KeyCode, int> _keyMappings;

    void Start()
    {
        // Populate the dictionary with the correct mappings.
        _keyMappings = new Dictionary<KeyCode, int>
        {
            { KeyCode.Alpha1, 1 },
            { KeyCode.Alpha2, 2 },
            { KeyCode.Alpha3, 3 },
            { KeyCode.Alpha4, 4 },
            { KeyCode.Alpha5, 5 },
            { KeyCode.Alpha6, 6 },
            { KeyCode.Alpha7, 7 },
            { KeyCode.Alpha8, 8 },
            { KeyCode.Alpha9, 9 },
            { KeyCode.Alpha0, 0 } // The '0' key maps to the last element.
        };
        
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

        /*if (keyAnimators.Length != 10)
        {
            Debug.LogError("Error: The 'keyAnimators' array must contain exactly 10 animators.");
        }*/
    }

    void Update()
    {
        // Iterate through our key mappings to see if one was pressed.
        foreach (var mapping in _keyMappings)
        {
            if (Input.GetKeyDown(mapping.Key))
            {
                // The value from the dictionary is the correct array index (0-9).
                int keyIndex = mapping.Value;

                // Play the sound/animation using that index.
                PlayKey(keyIndex);
                
                // Notify the ghost controller which index was triggered.
                if (ghostCatController != null)
                {
                    // The GhostCatController's logic is based on the note index (0-9), which now works correctly.
                    ghostCatController.PlayerPressedKey(keyIndex);
                }
            }
        }
    }
    
    public void PlayKey(int keyIndex)
    {
        // Check if the provided index is valid.
        if (keyIndex < 0 || keyIndex >= 10)
        {
            Debug.LogError($"Error: Invalid key index provided: {keyIndex}. Must be between 0 and 9.");
            return;
        }

        // Check if there is a sound assigned for this key.
        if (keySounds[keyIndex] != null)
        {
            audioSource.PlayOneShot(keySounds[keyIndex]);
        }
        else
        {
            Debug.LogWarning($"Warning: No audio clip assigned to key index {keyIndex}.");
        }

        /*// Check if there is an animator assigned for this key.
        if (keyAnimators[keyIndex] != null)
        {
            keyAnimators[keyIndex].SetTrigger(animationTriggerName);
        }
        else
        {
            Debug.LogWarning($"Warning: No animator assigned to key index {keyIndex}.");
        }*/
    }
}