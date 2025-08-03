// ==========================================================================
// Meowt of Tune - keyMovement
// Owned and contributed to by: brookcoli (Itch.io) - Aurorafume (GitHub), donkzilla03 (Itch.io) - AliAK03 (GitHub), JaydenFielderTorrens (Itch.io) - Squib35 (GitHub), komorebimoriko (Itch.io), TheRealCrizz (GitHub), LordVGahn (GitHub)
// itch.io listing: https://brookcoli.itch.io/meowt-of-tune - GitHub Repository: https://github.com/Aurorafume/PPR301
// ==========================================================================
//
// WHAT DOES THIS DO:
// This script provides simple, four-directional character movement based on
// keyboard input. It translates the object in world space and smoothly
// rotates it to face the direction of movement. The script prioritises
// vertical input, meaning diagonal movement is not possible.
//
// Core functionalities include:
// - Reading raw "Horizontal" and "Vertical" input axes.
// - Translating the GameObject's position based on input.
// - Smoothly rotating the GameObject to face its current movement direction.
// - Restricting movement to four cardinal directions (no diagonals).
//
// Dependencies:
// - Must be attached to the GameObject that is intended to be moved.
// - Relies on Unity's legacy Input Manager having "Horizontal" and "Vertical"
//   axes defined (which they are by default).
//
// ==========================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles simple, four-directional character movement and rotation based on keyboard input.
/// </summary>
public class keyMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("The movement speed of the character.")]
    public float speed = 5;

    private float horizontalInput;
    private float verticalInput;

    /// <summary>
    /// Main update loop, called once per frame.
    /// </summary>
    void Update()
    {
        HandleMovement();
    }

    /// <summary>
    /// Processes player input to move and rotate the character.
    /// </summary>
    public void HandleMovement()
    {
        // Get raw input values (either -1, 0, or 1).
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // Prioritise vertical movement over horizontal, preventing diagonal movement.
        if (verticalInput != 0)
        {
            horizontalInput = 0;
        }

        // Create a movement vector based on the input and scale it by speed and time.
        Vector3 movement = new Vector3(horizontalInput, 0, verticalInput) * speed * Time.deltaTime;

        // Move the character in world space.
        transform.Translate(movement, Space.World);

        // If the character is moving, rotate it to face the movement direction.
        if (movement != Vector3.zero)
        {
            // Calculate the target rotation based on the movement direction.
            Quaternion targetRotation = Quaternion.LookRotation(movement);

            // Smoothly interpolate from the current rotation to the target rotation.
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }
}