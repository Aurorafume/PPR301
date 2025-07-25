// ==========================================================================
// Meowt of Tune - Contact Noise
// Owned and contributed to by: brookcoli (Itch.io) - Aurorafume (GitHub), donkzilla03 (Itch.io) - AliAK03 (GitHub), JaydenFielderTorrens (Itch.io) - Squib35 (GitHub), komorebimoriko (Itch.io), TheRealCrizz (GitHub), LordVGahn (GitHub)
// itch.io listing: https://brookcoli.itch.io/meowt-of-tune - GitHub Repository: https://github.com/Aurorafume/PPR301
// ==========================================================================
//
// WHAT DOES THIS DO:
// This script triggers a noise event when an object (like the player or an item) 
// makes contact with a surface or another object. It uses a cooldown (immunity period)
// to prevent spamming repeated noise events within short intervals.
//
// Core functionalities include:
// - Generating a noise value through the NoiseHandler system.
// - Enforcing a short delay (immunityDuration) between consecutive noise events.
// - Serialised noise value and timing control for easy tuning.
//
// Dependencies:
// - NoiseHandler: must be present in the scene to process noise generation.
//
// ==========================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContactNoise : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Amount of noise generated upon contact.")]
    float noiseAmount = 10f;

    [SerializeField]
    [Tooltip("Minimum delay between successive noise triggers.")]
    float immunityDuration = 0.5f;

    private NoiseHandler noiseHandler;     // Reference to the global noise handler
    private float lastNoiseTime = 0f;      // Timestamp of last noise generation

    void Awake()
    {
        // Cache reference to NoiseHandler in the scene
        noiseHandler = FindObjectOfType<NoiseHandler>();
    }

    /// <summary>
    /// Public method to trigger a noise event from this object.
    /// Will only activate if enough time has passed since the last noise.
    /// </summary>
    public void GenerateContactNoise()
    {
        // Skip if immunity cooldown hasn't passed
        if (Time.time - lastNoiseTime < immunityDuration)
        {
            return;
        }

        // Send noise signal to the handler
        if (noiseHandler != null)
        {
            noiseHandler.GenerateNoise(noiseAmount);
            lastNoiseTime = Time.time;
        }
    }
}
