// ==========================================================================
// Meowt of Tune - Record Rotation
// Owned and contributed to by: brookcoli (Itch.io) - Aurorafume (GitHub), donkzilla03 (Itch.io) - AliAK03 (GitHub), JaydenFielderTorrens (Itch.io) - Squib35 (GitHub), komorebimoriko (Itch.io), TheRealCrizz (GitHub), LordVGahn (GitHub)
// itch.io listing: https://brookcoli.itch.io/meowt-of-tune - GitHub Repository: https://github.com/Aurorafume/PPR301
// ==========================================================================
//
// WHAT DOES THIS DO:
// This script is designed for collectible items in a Unity game, such as coins,
// gems, or records. It gives the item two core behaviors: continuous rotation
// for visual appeal, and collection logic that awards score to the player.
//
// Core functionalities include:
// - Rotating the GameObject around its vertical axis at a customisable speed.
// - Detecting when the player enters its trigger collider.
// - Communicating with a ScoreManager to add a specific point value.
// - Destroying the item after it has been collected.
//
// Dependencies:
// - UnityEngine for component and physics functionality.
// - A trigger Collider component on the same GameObject.
// - A ScoreManager script must exist in the scene for score to be awarded.
//
// ==========================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the behavior of a rotating, collectible item that awards score to the player.
/// </summary>
public class RecordRotation : MonoBehaviour
{
    [Header("Behavior & Value")]
    [Tooltip("The speed at which the object rotates around its vertical axis.")]
    public float rotationSpeed = 100f;
    [Tooltip("The number of points awarded to the player upon collection.")]
    public int scoreValue = 1000;

    /// <summary>
    /// Called every frame by Unity to handle the object's rotation.
    /// </summary>
    void Update()
    {
        // Rotate the object around its local Y-axis based on the rotation speed.
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }
    
    /// <summary>
    /// Called by Unity when a collider enters this object's trigger volume.
    /// </summary>
    /// <param name="other">The collider that entered the trigger.</param>
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that entered the trigger is the Player.
        if (other.CompareTag("Player"))
        {
            // Find the ScoreManager component in the scene.
            ScoreManager scoreManager = FindObjectOfType<ScoreManager>();
            
            // If the ScoreManager exists, add the score value to it.
            if (scoreManager != null)
            {
                scoreManager.AddScore(scoreValue);
            }
            else
            {
                Debug.LogWarning("ScoreManager not found in scene. Cannot add score.");
            }
            
            // Destroy this collectible item.
            Destroy(gameObject);
        }
    }
}