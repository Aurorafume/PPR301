// ==========================================================================
// Meowt of Tune - Paw Print 2
// Owned and contributed to by: brookcoli (Itch.io) - Aurorafume (GitHub), donkzilla03 (Itch.io) - AliAK03 (GitHub), JaydenFielderTorrens (Itch.io) - Squib35 (GitHub), komorebimoriko (Itch.io), TheRealCrizz (GitHub), LordVGahn (GitHub)
// itch.io listing: https://brookcoli.itch.io/meowt-of-tune - GitHub Repository: https://github.com/Aurorafume/PPR301
// ==========================================================================
//
// WHAT DOES THIS DO:
// This script creates a visual effect of paw prints being left behind by the
// player character. It only generates prints when the player is moving and on
// the ground, preventing prints from appearing in mid-air. The prints are
// spaced out by a set time interval, alternate between left and right feet,
// and gradually fade out before being destroyed.
//
// Core functionalities include:
// - Instantiating a paw print prefab at timed intervals while the player is
//   moving and grounded.
// - Alternating the horizontal offset of prints to simulate left and right steps.
// - Managing a list of all active paw prints.
// - Gradually fading out the paw prints over time for a clean visual effect.
// - Cleaning up and destroying faded prints to manage memory and performance.
//
// Dependencies:
// - Must be attached to the same GameObject as the 'PlayerMovement' script.
// - Requires a 'pawPrefab' with a SpriteRenderer to be assigned in the Inspector.
//
// ==========================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generates a trail of fading paw prints behind the player as they move on the ground.
/// </summary>
public class PawPrint2 : MonoBehaviour
{
    [Header("Component References")]
    private PlayerMovement script;

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

    // --- Private State Variables ---
    private float timeSinceLastStep = 0f; // Timer to track time since the last paw print was placed.
    private int pawIndex = 0;             // Counter to alternate between left and right paw placement.
    private Vector3 lastPosition;         // Stores the object's position from the last frame to detect movement.

    /// <summary>
    /// Initialises the starting position and caches component references.
    /// </summary>
    void Start()
    {
        lastPosition = transform.position;
        script = GetComponent<PlayerMovement>();
    }

    /// <summary>
    /// Main update loop, called once per frame. Handles step timing and fading.
    /// </summary>
    void Update()
    {
        // Check if the player has moved significantly since the last step was placed.
        float movedDistance = Vector3.Distance(transform.position, lastPosition);
        if (movedDistance > 0.01f)
        {
            timeSinceLastStep += Time.deltaTime;
            // Check if enough time has passed to leave a new paw print.
            if (timeSinceLastStep >= stepInterval)
            {
                LeavePawPrint();
                timeSinceLastStep = 0f;
                lastPosition = transform.position;
            }
        }

        // Continuously fade out all active paw prints.
        FadePaws();
    }

    /// <summary>
    /// Places a new paw print on the ground if the player is grounded.
    /// </summary>
    void LeavePawPrint()
    {
        // Only leave a print if the player is currently on the ground.
        if (script.grounded)
        {
            // Alternate the horizontal offset for left/right paw positions.
            float offsetX = (pawIndex % 2 == 0) ? pawOffsetX : -pawOffsetX;
            pawIndex++;

            // Calculate the spawn position based on the player's current position and the offset.
            Vector3 offset = new Vector3(offsetX, -negatePawHeight, 0f);
            Vector3 spawnPosition = transform.position + transform.rotation * offset;

            // Instantiate the paw print, rotating it to lie flat on the ground.
            GameObject newPaw = Instantiate(pawPrefab, spawnPosition, transform.rotation * Quaternion.Euler(90, 0, 0));
            
            // Ensure the new paw print starts fully opaque.
            SpriteRenderer spriteRenderer = newPaw.GetComponent<SpriteRenderer>();
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);
            
            // Add the new paw to the list to be managed.
            spawnedPaws.Add(newPaw);
        }
    }

    /// <summary>
    /// Iterates through all spawned paw prints to fade them out and destroy them when invisible.
    /// </summary>
    void FadePaws()
    {
        // Iterate backwards through the list, as it is safer when removing items during iteration.
        for (int i = spawnedPaws.Count - 1; i >= 0; i--)
        {
            GameObject paw = spawnedPaws[i];
            if (paw.TryGetComponent(out SpriteRenderer spriteRenderer))
            {
                // Gradually decrease the sprite's alpha value.
                Color color = spriteRenderer.color;
                color.a -= Time.deltaTime / 1.1f;
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