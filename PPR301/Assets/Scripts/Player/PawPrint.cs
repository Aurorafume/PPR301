// ==========================================================================
// Meowt of Tune - Paw Print
// Owned and contributed to by: brookcoli (Itch.io) - Aurorafume (GitHub), donkzilla03 (Itch.io) - AliAK03 (GitHub), JaydenFielderTorrens (Itch.io) - Squib35 (GitHub), komorebimoriko (Itch.io), TheRealCrizz (GitHub), LordVGahn (GitHub)
// itch.io listing: https://brookcoli.itch.io/meowt-of-tune - GitHub Repository: https://github.com/Aurorafume/PPR301
// ==========================================================================
//
// WHAT DOES THIS DO:
// This script provides a visual effect for the player character, generating a
// trail of paw prints on the ground as the player moves. The placement logic
// handles forward and lateral/diagonal movement differently. The prints fade
// out over time and are then destroyed to keep the scene clean.
//
// Core functionalities include:
// - Spawning paw print prefabs at timed intervals during movement.
// - Using different placement offsets for forward vs. lateral movement.
// - Alternating print placement to simulate left and right feet.
// - Dynamically adjusting the vertical height of prints for standing vs. crouching.
// - Managing a list of active prints, fading them out, and destroying them.
//
// Dependencies:
// - Must be attached to the same GameObject as the 'PlayerMovement' script.
// - Requires a 'pawPrefab' with a SpriteRenderer to be assigned in the Inspector.
// - The 'pawLocationArray' and 'horizontalPawLocationArray' must be configured
//   in the Inspector with the desired offset values.
//
// ==========================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Creates a trail of fading paw prints behind the player as they move.
/// </summary>
public class PawPrint : MonoBehaviour
{
    [Header("Prefabs & References")]
    [Tooltip("The paw print prefab to instantiate.")]
    public GameObject pawPrefab;
    private PlayerMovement playerMovementScript;

    [Header("Timing & Placement")]
    [Tooltip("The time, in seconds, between each paw print being placed.")]
    public float timeBetweenSteps;
    [Tooltip("Vertical offset from the player's pivot to the ground. This is adjusted automatically.")]
    public float negatePawHeight;
    [Tooltip("The alternating horizontal (X) offsets for left and right paws.")]
    public float[] pawLocationArray = new float[2];
    [Tooltip("The alternating forward (Z) offsets, used during lateral movement.")]
    public float[] horizontalPawLocationArray = new float[4];

    // --- Private State Variables ---
    private float stepTimer; // A general timer for placing prints.
    private int footIndex;   // Used to alternate between left/right paw X-offsets.
    private int lateralFootIndex; // Used to alternate forward/backward Z-offsets during lateral movement.
    private List<GameObject> spawnedPaws = new List<GameObject>(); // A list of all active paw prints.

    /// <summary>
    /// Caches a reference to the PlayerMovement script.
    /// </summary>
    void Start()
    {
        playerMovementScript = GetComponent<PlayerMovement>();
    }

    /// <summary>
    /// Main update loop. Checks for movement and manages paw print creation and fading.
    /// </summary>
    void Update()
    {
        float hInput = Input.GetAxisRaw("Horizontal");
        float vInput = Input.GetAxisRaw("Vertical");

        // Check if the player is moving on the ground or while crouching.
        bool isMovingOnGround = (hInput != 0 || vInput != 0) && (playerMovementScript.grounded || playerMovementScript.isCrouching);

        if (isMovingOnGround)
        {
            // If moving only forward/backward, use the standard step logic.
            if (vInput != 0 && hInput == 0)
            {
                HandleForwardSteps();
            }
            // If moving sideways or diagonally, use the lateral step logic.
            else
            {
                HandleLateralSteps();
            }
        }
        
        // Continuously fade out any existing paw prints.
        FadeOutPaws();
        
        // Adjust paw print height based on whether the player is crouching.
        negatePawHeight = playerMovementScript.isCrouching ? 0.24f : 0.49f;
    }

    /// <summary>
    /// Handles the timer and placement logic for forward/backward movement.
    /// </summary>
    public void HandleForwardSteps()
    {
        stepTimer += Time.deltaTime;
        if (stepTimer >= timeBetweenSteps)
        {
            // Alternate between the left and right foot index.
            footIndex = (footIndex + 1) % pawLocationArray.Length;

            // Define the local offset for a forward step.
            Vector3 offset = new Vector3(pawLocationArray[footIndex], -negatePawHeight, 0.5f);
            CreatePawAtOffset(offset);
            
            stepTimer = 0;
        }
    }

    /// <summary>
    /// Handles the timer and placement logic for lateral or diagonal movement.
    /// </summary>
    public void HandleLateralSteps()
    {
        stepTimer += Time.deltaTime;
        if (stepTimer >= timeBetweenSteps)
        {
            // Alternate both the left/right and forward/backward foot indices.
            footIndex = (footIndex + 1) % pawLocationArray.Length;
            lateralFootIndex = (lateralFootIndex + 1) % horizontalPawLocationArray.Length;

            // Define the local offset for a lateral step.
            Vector3 offset = new Vector3(pawLocationArray[footIndex], -negatePawHeight, horizontalPawLocationArray[lateralFootIndex]);
            CreatePawAtOffset(offset);
            
            stepTimer = 0;
        }
    }

    /// <summary>
    /// Instantiates a paw print at the calculated offset and adds it to the list for fading.
    /// </summary>
    /// <param name="localOffset">The local position offset from the player's pivot.</param>
    private void CreatePawAtOffset(Vector3 localOffset)
    {
        // Calculate the world-space spawn position and rotation.
        Vector3 spawnPosition = transform.position + transform.rotation * localOffset;
        Quaternion spawnRotation = transform.rotation * Quaternion.Euler(90, 0, 0); // Rotate to lie flat.

        // Instantiate the prefab.
        GameObject newPaw = Instantiate(pawPrefab, spawnPosition, spawnRotation);
        
        // Ensure the sprite is fully opaque when it first appears.
        SpriteRenderer spriteRenderer = newPaw.GetComponent<SpriteRenderer>();
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1);
        
        // Add the new paw to the list to be managed.
        spawnedPaws.Add(newPaw);
    }

    /// <summary>
    /// Iterates through all spawned paw prints to fade them out and destroy them when invisible.
    /// </summary>
    public void FadeOutPaws()
    {
        // Iterate backwards through the list for safe removal of items during the loop.
        for (int i = spawnedPaws.Count - 1; i >= 0; i--)
        {
            GameObject paw = spawnedPaws[i];

            if (paw.TryGetComponent(out SpriteRenderer spriteRenderer))
            {
                // Reduce the sprite's alpha value over time.
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