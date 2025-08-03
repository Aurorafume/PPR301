// ==========================================================================
// Meowt of Tune - Show Icon Script
// Owned and contributed to by: brookcoli (Itch.io) - Aurorafume (GitHub), donkzilla03 (Itch.io) - AliAK03 (GitHub), JaydenFielderTorrens (Itch.io) - Squib35 (GitHub), komorebimoriko (Itch.io), TheRealCrizz (GitHub), LordVGahn (GitHub)
// itch.io listing: https://brookcoli.itch.io/meowt-of-tune - GitHub Repository: https://github.com/Aurorafume/PPR301
// ==========================================================================
//
// WHAT DOES THIS DO:
// This script manages an in-world UI icon, such as an interaction prompt that
// appears over an object. Its primary functions are to control the icon's
// visibility and to make it continuously face the main camera, ensuring it's
// always readable by the player. This is often called a "billboard" effect.
//
// Core functionalities include:
// - A public method to set the icon's visibility on or off.
// - A "billboard" behaviour that forces the icon to always face the camera.
//
// Dependencies:
// - Typically attached to an interactable object.
// - The 'playerMouseIcon' child object must be assigned in the Inspector.
// - Relies on another script (like 'PlayerInteractHandler') to call the
//   'SetIconActive()' method.
// - Requires a camera in the scene tagged as "MainCamera" to function correctly.
//
// ==========================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the visibility and camera-facing behaviour of an in-world UI icon.
/// </summary>
public class ShowIconScript : MonoBehaviour
{
    [Header("Icon Reference")]
    [Tooltip("The GameObject for the icon that will be shown and rotated.")]
    public GameObject playerMouseIcon;

    /// <summary>
    /// Called every frame. If the icon is active, ensures it faces the camera.
    /// </summary>
    void Update()
    {
        if (playerMouseIcon.activeInHierarchy)
        {
            ContinuouslyFaceCamera();
        }
    }

    /// <summary>
    /// Rotates the icon to always face the main camera.
    /// </summary>
    void ContinuouslyFaceCamera()
    {
        // Point the icon towards the main camera's position.
        playerMouseIcon.transform.LookAt(Camera.main.transform.position, Vector3.up);
        // Rotate the icon 180 degrees on its Y-axis to make its front face visible.
        playerMouseIcon.transform.Rotate(0f, 180f, 0f);
    }

    /// <summary>
    /// A public method for other scripts to show or hide the icon.
    /// </summary>
    /// <param name="active">True to show the icon, false to hide it.</param>
    public void SetIconActive(bool active)
    {
        playerMouseIcon.SetActive(active);
    }
}