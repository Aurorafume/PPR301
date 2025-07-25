// ==========================================================================
// Pressure Plate
// Owned and contributed to by: brookcoli (Itch.io) - Aurorafume (GitHub), donkzilla03 (Itch.io) - AliAK03 (GitHub), JaydenFielderTorrens (Itch.io) - Squib35 (GitHub), komorebimoriko (Itch.io), TheRealCrizz (GitHub), LordVGahn (GitHub)
// itch.io listing: https://brookcoli.itch.io/meowt-of-tune - GitHub Repository: https://github.com/Aurorafume/PPR301
// ==========================================================================
//
// WHAT DOES THIS DO:
// This script creates a functional pressure plate for puzzles or level
// interactions in a Unity game. It activates an event, such as opening a door,
// when the correct object or the player steps on it.
//
// Core functionalities include:
// - Two activation modes: a permanent "TouchButton" and a temporary "HoldButton".
// - Can be configured to respond to either the player or a specific object.
// - Provides visual feedback by swapping its material when its state changes.
// - Controls a target GameObject (like a door) by activating/deactivating it.
//
// Dependencies:
// - UnityEngine for component and physics functionality.
// - A trigger Collider component on the same GameObject.
// - A MeshRenderer to allow for material swapping.
//
// ==========================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages a pressure plate that can be activated by the player or an object to control a door.
/// </summary>
public class PressurePlate : MonoBehaviour
{
    [Header("Plate State & Target")]
    [Tooltip("Is the pressure plate currently activated?")]
    public bool activated;
    [Tooltip("The door or object that this plate controls.")]
    public GameObject door;

    [Header("Visuals")]
    [Tooltip("The material used when the plate is inactive.")]
    public Material offMaterial;
    [Tooltip("The material used to show the plate is active (glowing).")]
    public Material glowMaterial;
    
    [Header("Activation Logic")]
    [Tooltip("Determines the plate's behavior: 'Touch' for permanent activation, 'Hold' to require continuous pressure.")]
    public ButtonType buttonType;
    [Tooltip("Determines what can activate this plate: the 'Player' or a specific 'Object'.")]
    public TouchType touchBy;
    [Tooltip("The specific GameObject that activates this plate. Only used if 'Touch By' is set to 'Object'.")]
    public GameObject obj;

    /// <summary>
    /// Defines the behavior of the button upon activation.
    /// </summary>
    public enum ButtonType
    {
        TouchButton, // Stays on after being pressed once.
        HoldButton   // Must be held down to stay on.
    }

    /// <summary>
    /// Defines what type of object can activate the plate.
    /// </summary>
    public enum TouchType
    {
        Player,
        Object
    }

    /// <summary>
    /// Called by Unity when a collider enters the plate's trigger volume.
    /// </summary>
    /// <param name="collider">The collider that entered the trigger.</param>
    void OnTriggerEnter(Collider collider)
    {
        if (activated) return; // Do nothing if already on.

        bool shouldActivate = false;
        switch (touchBy)
        {
            case TouchType.Player:
                if (collider.CompareTag("Player"))
                {
                    shouldActivate = true;
                }
                break;
            case TouchType.Object:
                if (collider.gameObject == obj)
                {
                    shouldActivate = true;
                }
                break;
        }

        if (shouldActivate)
        {
            ActivatePlate();
        }
    }

    /// <summary>
    /// Called by Unity when a collider exits the plate's trigger volume.
    /// </summary>
    /// <param name="collider">The collider that exited the trigger.</param>
    void OnTriggerExit(Collider collider)
    {
        // Only deactivate if it's a "HoldButton".
        if (buttonType != ButtonType.HoldButton) return;
        
        bool shouldDeactivate = false;
        switch (touchBy)
        {
            case TouchType.Player:
                if (collider.CompareTag("Player"))
                {
                    shouldDeactivate = true;
                }
                break;
            case TouchType.Object:
                if (collider.gameObject == obj)
                {
                    shouldDeactivate = true;
                }
                break;
        }

        if (shouldDeactivate)
        {
            DeactivatePlate();
        }
    }

    /// <summary>
    /// Handles the logic for activating the pressure plate.
    /// </summary>
    private void ActivatePlate()
    {
        Debug.Log("Pressure Plate Activated.");
        activated = true;
        GetComponent<MeshRenderer>().material = glowMaterial;
        if (door != null)
        {
            door.SetActive(false); // Open the door.
        }
    }

    /// <summary>
    /// Handles the logic for deactivating the pressure plate.
    /// </summary>
    private void DeactivatePlate()
    {
        Debug.Log("Pressure Plate Deactivated.");
        activated = false;
        GetComponent<MeshRenderer>().material = offMaterial;
        if (door != null)
        {
            door.SetActive(true); // Close the door.
        }
    }
}