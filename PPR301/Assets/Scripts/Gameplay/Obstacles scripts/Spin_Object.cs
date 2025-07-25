// ==========================================================================
// Meowt of Tune - Spin Object
// Owned and contributed to by: brookcoli (Itch.io) - Aurorafume (GitHub), donkzilla03 (Itch.io) - AliAK03 (GitHub), JaydenFielderTorrens (Itch.io) - Squib35 (GitHub), komorebimoriko (Itch.io), TheRealCrizz (GitHub), LordVGahn (GitHub)
// itch.io listing: https://brookcoli.itch.io/meowt-of-tune - GitHub Repository: https://github.com/Aurorafume/PPR301
// ==========================================================================
//
// WHAT DOES THIS DO:
// This is a simple utility script that adds a continuous rotation effect to any
// GameObject. It is designed to be a lightweight and reusable component for
// creating simple animations or visual flair for items in the scene.
//
// Core functionalities include:
// - Rotating a GameObject around its vertical (Y) axis.
// - A public variable to easily adjust the rotation speed from the Unity Editor.
//
// Dependencies:
// - UnityEngine for core component functionality.
//
// ==========================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A simple component that continuously rotates its GameObject around the Y-axis.
/// </summary>
public class Spin_Object : MonoBehaviour
{
    [Header("Rotation Settings")]
    [Tooltip("The speed at which the object rotates. Can be negative for reverse rotation.")]
    public float spinSpeed = 50f;

    /// <summary>
    /// Called every frame by Unity.
    /// </summary>
    void Update()
    {
        // Apply a rotation around the world's Y-axis.
        // The rotation amount is scaled by Time.deltaTime to ensure it is smooth and
        // independent of the frame rate.
        transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.World);
    }
}