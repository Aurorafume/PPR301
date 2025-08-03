// ==========================================================================
// Meowt of Tune - Mansion Cameras
// Owned and contributed to by: brookcoli (Itch.io) - Aurorafume (GitHub), donkzilla03 (Itch.io) - AliAK03 (GitHub), JaydenFielderTorrens (Itch.io) - Squib5 (GitHub), komorebimoriko (Itch.io), TheRealCrizz (GitHub), LordVGahn (GitHub)
// itch.io listing: https://brookcoli.itch.io/meowt-of-tune - GitHub Repository: https://github.com/Aurorafume/PPR301
// ==========================================================================
//
// WHAT DOES THIS DO:
// This is a simple initialisation script for managing cameras in a specific
// scene. Its sole function is to set which of two cameras is the active one
// when the scene begins by controlling their render depth.
//
// Core functionalities include:
// - Setting the 'depth' property of two cameras to control which one is
//   rendered on top.
//
// Dependencies:
// - Requires two Camera objects ('camera1' and 'camera2') to be assigned in
//   the Inspector.
//
// ==========================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sets the initial active camera for the scene by managing camera depths.
/// </summary>
public class MansionCameras : MonoBehaviour
{
    [Header("Camera References")]
    [Tooltip("The camera to be set as the primary, active camera.")]
    public Camera camera1;
    [Tooltip("The camera to be set as the secondary, inactive camera.")]
    public Camera camera2;

    /// <summary>
    /// Sets the initial camera priorities on startup.
    /// </summary>
    void Start()
    {
        // Cameras with a higher depth value are rendered on top.
        // This ensures camera1 is the active camera when the scene loads.
        if (camera1) camera1.depth = 1;
        if (camera2) camera2.depth = 0;
    }
}