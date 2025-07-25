// ==========================================================================
// Meowt of Tune - X_Y_Push
// Owned and contributed to by: brookcoli (Itch.io) - Aurorafume (GitHub), donkzilla03 (Itch.io) - AliAK03 (GitHub), JaydenFielderTorrens (Itch.io) - Squib35 (GitHub), komorebimoriko (Itch.io), TheRealCrizz (GitHub), LordVGahn (GitHub)
// itch.io listing: https://brookcoli.itch.io/meowt-of-tune - GitHub Repository: https://github.com/Aurorafume/PPR301
// ==========================================================================
//
// WHAT DOES THIS DO:
// This script acts as a simple one-way boundary on the X-axis. If the object
// this script is attached to moves past a certain point, it is instantly
// repositioned to a defined reset coordinate. It's useful for creating simple
// containment fields or reset zones.
//
// Core functionalities include:
// - Continuously monitoring the object's X position.
// - Resetting the position if it crosses a boundary threshold.
//
// Dependencies:
// - UnityEngine for core component functionality.
//
// ==========================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A simple script that pushes an object back if it crosses a boundary on the X-axis.
/// </summary>
public class X_Y_Push : MonoBehaviour
{
    [Header("Boundary Settings")]
    [Tooltip("The X-coordinate that acts as the boundary line.")]
    public float boundaryX = -90f;
    [Tooltip("The X-coordinate to move the object to if it crosses the boundary.")]
    public float resetX = -78f;

    /// <summary>
    /// Called every frame by Unity to check the object's position.
    /// </summary>
    void Update()
    {
        // Check if the object's current X position has crossed the boundary.
        if (transform.position.x > boundaryX)
        {
            // If it has, reset its position to the designated X coordinate, keeping its Y and Z coordinates.
            transform.position = new Vector3(resetX, transform.position.y, transform.position.z);
        }
    }
}