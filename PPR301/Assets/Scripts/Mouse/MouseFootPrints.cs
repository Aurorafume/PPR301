// ==========================================================================
// Meowt of Tune - Mouse Footprints
// Owned and contributed to by: brookcoli (Itch.io) - Aurorafume (GitHub), donkzilla03 (Itch.io) - AliAK03 (GitHub), JaydenFielderTorrens (Itch.io) - Squib35 (GitHub), komorebimoriko (Itch.io), TheRealCrizz (GitHub), LordVGahn (GitHub)
// itch.io listing: https://brookcoli.itch.io/meowt-of-tune - GitHub Repository: https://github.com/Aurorafume/PPR301
// ==========================================================================
//
// WHAT DOES THIS DO:
// This script creates a visual effect of paw prints being left behind by a
// moving object. It only generates prints when the object is in motion,
// spacing them out by a set time interval. The prints are alternated left
// and right, and they gradually fade out over time before being destroyed.
//
// Core functionalities include:
// - Instantiating a paw print prefab at timed intervals while moving.
// - Alternating the horizontal position of prints to simulate left/right steps.
// - Managing a list of all currently active paw prints.
// - Gradually fading the alpha of each paw print's sprite over time.
// - Automatically cleaning up and destroying prints after they have faded.
//
// Dependencies:
// - Must be attached to the moving GameObject that should leave the prints.
// - Requires a 'pawPrefab' with a SpriteRenderer component to be assigned
//   in the Inspector.
//
// ==========================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generates a trail of fading paw prints behind a moving GameObject.
/// </summary>
public class MouseFootPrints : MonoBehaviour
{
    [Header("Paw Print Settings")]
    [Tooltip("Prefab of the paw print to instantiate.")]
    public GameObject pawPrefab;

    [Tooltip("Time in seconds between each paw print is placed.")]
    public float stepInterval = 0.5f;

    [Tooltip("Horizontal offset to alternate between left and right paw prints.")]
    public float pawOffsetX = 0.1f;

    [Tooltip("Vertical offset to align the paw print with the floor.")]
    public float negatePawHeight = 0.1f;

    [Header("Debug & Runtime")]
    [Tooltip("List of currently spawned paw prints for fading and cleanup.")]
    public List<GameObject> spawnedPaws = new List<GameObject>();

    private float timeSinceLastStep = 0f; // Timer to track time since the last paw print was placed.
    private int pawIndex = 0;             // Counter to alternate between left and right paw placement.
    private Vector3 lastPosition;         // Stores the object's position from the last frame to detect movement.

    /// <summary>
    /// Initialises the starting position for movement detection.
    /// </summary>
    void Start()
    {
        // Store the initial position to compare against in the first Update frame.
        lastPosition = transform.position;
    }

    /// <summary>
    /// Main update loop, called once per frame. Handles step timing and fading.
    /// </summary>
    void Update()
    {
        // Calculate the distance moved since the last frame a print was laid.
        float movedDistance = Vector3.Distance(transform.position, lastPosition);
        
        // Only process steps if the object is actually moving.
        if (movedDistance > 0.01f)
        {
            timeSinceLastStep += Time.deltaTime;
            
            // If enough time has passed, leave a new paw print.
            if (timeSinceLastStep >= stepInterval)
            {
                LeavePawPrint();
                timeSinceLastStep = 0f; // Reset the timer.
                lastPosition = transform.position; // Update the last known position.
            }
        }
        
        // Continuously fade out all active paw prints.
        FadePaws();
    }

    /// <summary>
    /// Calculates the position for and instantiates a new paw print.
    /// </summary>
    void LeavePawPrint()
    {
        // Determine the horizontal offset based on whether it's a left or right step.
        float offsetX = (pawIndex % 2 == 0) ? pawOffsetX : -pawOffsetX;
        pawIndex++; // Increment for the next step.

        // Calculate the final spawn position relative to the mouse's position and rotation.
        Vector3 offset = new Vector3(offsetX, -negatePawHeight, 0f);
        Vector3 spawnPosition = transform.position + transform.rotation * offset;

        // Instantiate the paw print, rotating it to lie flat on the ground.
        GameObject newPaw = Instantiate(pawPrefab, spawnPosition, transform.rotation * Quaternion.Euler(90, 0, 0));
        
        // Ensure the new paw print starts fully opaque.
        SpriteRenderer spriteRenderer = newPaw.GetComponent<SpriteRenderer>();
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);

        // Add the new paw print to the list for tracking and fading.
        spawnedPaws.Add(newPaw);
    }

    /// <summary>
    /// Iterates through all spawned paw prints to fade them out and destroy them when invisible.
    /// </summary>
    void FadePaws()
    {
        // Iterate backwards through the list, as it's safer when removing items during iteration.
        for (int i = spawnedPaws.Count - 1; i >= 0; i--)
        {
            GameObject paw = spawnedPaws[i];
            if (paw.TryGetComponent(out SpriteRenderer spriteRenderer))
            {
                // Gradually decrease the sprite's alpha value.
                Color color = spriteRenderer.color;
                color.a -= Time.deltaTime / 1.1f; // Adjust the denominator to change fade duration.
                spriteRenderer.color = color;

                // If the paw print is fully faded, destroy it and remove it from the list.
                if (color.a <= 0)
                {
                    Destroy(paw);
                    spawnedPaws.RemoveAt(i);
                }
            }
        }
    }
}