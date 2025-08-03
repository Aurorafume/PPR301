// ==========================================================================
// Meowt of Tune - Cameras
// Owned and contributed to by: brookcoli (Itch.io) - Aurorafume (GitHub), donkzilla03 (Itch.io) - AliAK03 (GitHub), JaydenFielderTorrens (Itch.io) - Squib35 (GitHub), komorebimoriko (Itch.io), TheRealCrizz (GitHub), LordVGahn (GitHub)
// itch.io listing: https://brookcoli.itch.io/meowt-of-tune - GitHub Repository: https://github.com/Aurorafume/PPR301
// ==========================================================================
//
// WHAT DOES THIS DO:
// This is a central camera and game state controller. It manages the transitions
// between the default third-person player camera and various fixed-angle or
// top-down cameras placed in the level. These transitions are triggered
// automatically when the player enters specific zones. This script also updates
// other game systems to align with the current camera view, such as player
// controls and respawn locations.
//
// Core functionalities include:
// - Switching between multiple cameras by managing their 'depth' property.
// - Trigger-based activation of fixed camera zones (e.g. "Area1", "Area2").
// - Smoothly interpolating a camera holder object between camera positions.
// - Invoking a C# event to sync player movement controls with camera perspective.
// - Disabling player abilities (like jumping) in specific zones.
// - Updating the player's respawn point based on the currently active zone.
//
// Dependencies:
// - Requires multiple Camera components and a camera holder 'obj' to be assigned.
// - Depends on other scripts: PlayerMovement and OutOfBounds.
// - Requires trigger volumes in the scene with tags like "Area1", "Area2", etc.
// - Other scripts (like PlayerMovement) must subscribe to the OnEnterTopDownCamera
//   event to function correctly.
//
// ==========================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages camera switching and related game state changes based on player location.
/// </summary>
public class Cameras : MonoBehaviour
{
    [Header("Camera References")]
    [Tooltip("The primary third-person camera that follows the player.")]
    public Camera camera1;
    [Tooltip("The fixed camera for Area 1.")]
    public Camera camera2;
    [Tooltip("The fixed camera for Area 2.")]
    public Camera camera3;
    [Tooltip("The fixed camera for Area 3.")]
    public Camera camera4;
    [Tooltip("Unused camera reference.")]
    public Camera camera5;
    [Tooltip("Unused camera reference.")]
    public Camera camera6;
    [Tooltip("A special-purpose camera, potentially for cutscenes (currently unused in logic).")]
    public Camera KeyCamera;
    [Tooltip("A camera object that is smoothly moved between different camera positions.")]
    public GameObject obj;

    [Header("Component References")]
    [Tooltip("Reference to the PlayerMovement script to modify player state.")]
    public PlayerMovement script;
    [Tooltip("Reference to the OutOfBounds script to set respawn points.")]
    public OutOfBounds BoundsScript;
    
    [Header("Camera State & Movement")]
    [Tooltip("When true, the camera object moves towards the player camera. When false, it moves towards a fixed camera.")]
    public bool move;
    [Tooltip("The index for the cameraList, determining which fixed camera to use.")]
    public int followCamArray;
    [Tooltip("List of all fixed cameras that can be switched to.")]
    public List<Camera> cameraList = new List<Camera>();
    [Tooltip("How quickly the camera object interpolates to its target position.")]
    public float smoothingFactor;

    [Header("Events")]
    [Tooltip("The forward direction for player movement when in the default top-down view.")]
    public float topDownForwardDirection = -90;
    /// <summary>
    /// Event invoked when entering or exiting a top-down camera view.
    /// Bool: Is the view now top-down? Float: What is the new forward angle for player movement?
    /// </summary>
    public static event Action<bool, float> OnEnterTopDownCamera;

    private bool robotFollow; // Internal state flag for when the camera has finished transitioning.
    private Vector3 velocity = Vector3.zero; // Used by SmoothDamp for velocity calculation.

    /// <summary>
    /// Initialises camera depths, populates the camera list, and sets the initial state.
    /// </summary>
    void Start()
    {
        // Set the primary camera to be active and others inactive.
        if (camera1) camera1.depth = 1;
        if (camera2) camera2.depth = 0;
        if (camera3) camera3.depth = 0;
        if (camera4) camera4.depth = 0;

        // Add the fixed cameras to a list for easy access by index.
        cameraList.Add(camera2);
        cameraList.Add(camera3);
        cameraList.Add(camera4);
        
        // Start in the default player-following camera mode.
        move = true;
    }
    
    /// <summary>
    /// Main update loop, called once per frame.
    /// </summary>
    void Update()
    {
        // Handle all camera state logic.
        HandleCameraState();
    }

    /// <summary>
    /// The main state machine for controlling camera transitions and positions.
    /// </summary>
    void HandleCameraState()
    {
        // --- STATE 1: Moving to or following the Player Camera ---
        if(move)
        {
            // If the camera is still transitioning back to the player...
            if(!robotFollow)
            {
                // Smoothly interpolate the camera holder's position and rotation towards the player camera.
                float t = 1 - Mathf.Exp(-smoothingFactor * Time.deltaTime);
                obj.transform.rotation = Quaternion.Slerp(obj.transform.rotation, camera1.transform.rotation, t);
                obj.transform.position = Vector3.Lerp(obj.transform.position, camera1.transform.position, t);
                smoothingFactor += Time.deltaTime * 100;
            }

            // Once the transition is complete (very close to the target)...
            if (Vector3.Distance(obj.transform.position, camera1.transform.position) < 0.01f)
            {
                // Snap to the final position and set the follow state to true.
                obj.transform.position = camera1.transform.position;
                robotFollow = true;
            }
            // If the camera is in the 'robotFollow' state...
            else if(robotFollow)
            {
                // Hard-lock the camera holder's transform to the player camera's transform.
                smoothingFactor = 0;
                obj.transform.position = camera1.transform.position;
                obj.transform.rotation = camera1.transform.rotation;
                
                // Ensure the player camera is active and the fixed camera is not.
                camera1.depth = 1;
                camera2.depth = 0; // Assuming followCamera was intended to be camera2 or similar
            }
        }
        // --- STATE 2: Moving to a Fixed Camera ---
        else if(!move)
        {
            // Switch active camera depth to the selected fixed camera.
            camera1.depth = 0;
            cameraList[followCamArray].depth = 1;

            // Reset state flags from the other mode.
            robotFollow = false;
            
            // Smoothly move the camera object towards the target fixed camera's position.
            obj.transform.position = Vector3.SmoothDamp(obj.transform.position, cameraList[followCamArray].transform.position, ref velocity, 0.3f);
            
            // Snap to position when very close.
            if (Vector3.Distance(obj.transform.position, cameraList[followCamArray].transform.position) < 0.01f)
            {
                obj.transform.position = cameraList[followCamArray].transform.position;
            }
            
            // Smoothly rotate towards the fixed camera's orientation.
            obj.transform.rotation = Quaternion.Slerp(obj.transform.rotation, cameraList[followCamArray].transform.rotation, Time.deltaTime * 3);
        }
    }

    /// <summary>
    /// Detects when the player enters a fixed camera zone.
    /// </summary>
    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Area1"))
        {
            followCamArray = 0; // Set target camera to the first in the list.
            move = false; // Switch to fixed camera mode.
            script.noJumpMode = true; // Disable player jumping.
            OnEnterTopDownCamera?.Invoke(true, topDownForwardDirection); // Notify systems of camera change.
            BoundsScript.currentRespawnLocation = BoundsScript.spawnLocationsArray[0]; // Update respawn point.
        }
        else if (collision.gameObject.CompareTag("Area2"))
        {
            followCamArray = 1;
            move = false;
            script.noJumpMode = true;
            OnEnterTopDownCamera?.Invoke(true, -90);
            BoundsScript.currentRespawnLocation = BoundsScript.spawnLocationsArray[1];
        }
        else if (collision.gameObject.CompareTag("Area3"))
        {
            followCamArray = 2;
            move = false;
            script.noJumpMode = true;
            OnEnterTopDownCamera?.Invoke(true, -90);
            BoundsScript.currentRespawnLocation = BoundsScript.spawnLocationsArray[1];
        }
    }

    /// <summary>
    /// Detects when the player exits a fixed camera zone.
    /// </summary>
    void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("Area1") || 
            collision.gameObject.CompareTag("Area2") || 
            collision.gameObject.CompareTag("Area3"))
        {
            move = true; // Switch back to player-following camera mode.
            script.noJumpMode = false; // Re-enable player jumping.
            OnEnterTopDownCamera?.Invoke(false, topDownForwardDirection); // Notify systems we are leaving top-down view.
        }
    }
}