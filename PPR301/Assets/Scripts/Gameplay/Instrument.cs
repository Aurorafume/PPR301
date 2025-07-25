// ==========================================================================
// Meowt of Tune - Instrument
// Owned and contributed to by: brookcoli (Itch.io) - Aurorafume (GitHub), donkzilla03 (Itch.io) - AliAK03 (GitHub), JaydenFielderTorrens (Itch.io) - Squib35 (GitHub), komorebimoriko (Itch.io), TheRealCrizz (GitHub), LordVGahn (GitHub)
// itch.io listing: https://brookcoli.itch.io/meowt-of-tune - GitHub Repository: https://github.com/Aurorafume/PPR301
// ==========================================================================
//
// WHAT DOES THIS DO:
// This script makes a GameObject act like a simple instrument. When the player
// character collides with it, it plays a designated audio clip. This is useful
// for creating interactive sound objects in the environment.
//
// Core functionalities include:
// - Detecting a collision with an object tagged "Player".
// - Playing an assigned AudioSource on impact.
//
// Dependencies:
// - UnityEngine for core and physics functionality.
// - An AudioSource component on the same GameObject.
// - A Collider component to define its physical shape.
// - A Rigidbody component is required on either this object or the player
//   for collision events to be triggered.
//
// ==========================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Plays a sound when the player collides with this object.
/// </summary>
public class Instrument : MonoBehaviour
{
    [Header("Sound Settings")]
    [Tooltip("The AudioSource component containing the sound to play on impact.")]
    public AudioSource sound;

    /// <summary>
    /// Called by Unity's physics engine when a collision occurs.
    /// </summary>
    /// <param name="collision">Data related to the collision event.</param>
    void OnCollisionEnter(Collision collision)
    {
        // Check if the object we collided with is the player.
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player collided with an instrument.");
            
            // If a sound source is assigned, play it.
            if (sound != null)
            {
                sound.Play();
            }
        }
    }
}