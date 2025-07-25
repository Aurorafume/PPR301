// ==========================================================================
// Meowt of Tune - Letter Cube
// Owned and contributed to by: brookcoli (Itch.io) - Aurorafume (GitHub), donkzilla03 (Itch.io) - AliAK03 (GitHub), JaydenFielderTorrens (Itch.io) - Squib35 (GitHub), komorebimoriko (Itch.io), TheRealCrizz (GitHub), LordVGahn (GitHub)
// itch.io listing: https://brookcoli.itch.io/meowt-of-tune - GitHub Repository: https://github.com/Aurorafume/PPR301
// ==========================================================================
//
// WHAT DOES THIS DO:
// This script defines the behavior of a physical, interactable cube that
// represents a single letter in a word puzzle. It allows the player to pick
// up, carry, and place the cube onto designated pedestals.
//
// Core functionalities include:
// - Representing a single letter, which can be set in the editor.
// - Toggling between a "held" and "placed" state.
// - Automatically detecting and snapping to the nearest available pedestal when dropped.
// - Updating its visual appearance in the editor to reflect the assigned letter.
//
// Dependencies:
// - UnityEngine and TMPro for core functionality and text display.
// - A Rigidbody and a trigger Collider on this GameObject.
// - Pedestal custom script on nearby objects for snapping logic.
//
// ==========================================================================

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the state and interaction of an interactable letter cube for a puzzle.
/// </summary>
public class LetterCube : MonoBehaviour
{
    [Header("Cube Configuration")]
    [Tooltip("The letter this cube represents.")]
    public string myLetter;

    // Internal state tracking
    private bool held;
    private Pedestal myPedestal; // The pedestal this cube is currently on.
    private List<Pedestal> pedestalsInRange = new List<Pedestal>();
    private Rigidbody rb;

    /// <summary>
    /// Caches component references on awake.
    /// </summary>
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Editor-only function. Updates the displayed letter on the cube's text components
    /// whenever 'myLetter' is changed in the Inspector for easy visual feedback.
    /// </summary>
    void OnValidate()
    {
        TextMeshProUGUI[] letterTexts = GetComponentsInChildren<TextMeshProUGUI>();
        foreach (TextMeshProUGUI lettertext in letterTexts)
        {
            lettertext.text = myLetter;
        }
    }

    /// <summary>
    /// Toggles the cube's held state, handling the logic for picking up or placing the cube.
    /// This method is intended to be called by a player interaction script.
    /// </summary>
    public void OnInteraction()
    {
        if (held) // If currently held, this action is to drop it.
        {
            held = false;
            TryPlaceOnPedestal();
        }
        else // If not held, this action is to pick it up.
        {
            held = true;
            // If it was on a pedestal, unregister it from that pedestal.
            if (myPedestal)
            {
                myPedestal.SetLetterCube(null);
                myPedestal = null;
            }
        }
    }

    /// <summary>
    /// Called by Unity when this object's collider enters another trigger.
    /// Adds a pedestal to the list of nearby pedestals.
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        Pedestal pedestalInRange = other.GetComponentInParent<Pedestal>();
        if (pedestalInRange)
        {
            pedestalsInRange.Add(pedestalInRange);
        }
    }

    /// <summary>
    /// Called by Unity when this object's collider exits another trigger.
    /// Removes a pedestal from the list of nearby pedestals.
    /// </summary>
    void OnTriggerExit(Collider other)
    {
        Pedestal exitPedestal = other.GetComponentInParent<Pedestal>();
        if (exitPedestal && pedestalsInRange.Contains(exitPedestal))
        {
            pedestalsInRange.Remove(exitPedestal);
        }
    }

    /// <summary>
    /// Finds the closest available pedestal from the list of pedestals in range
    /// and snaps the cube to it.
    /// </summary>
    void TryPlaceOnPedestal()
    {
        if (pedestalsInRange.Count == 0) return;

        Pedestal snapPedestal = null;
        float closestDistance = float.MaxValue;

        // Find the closest pedestal from all pedestals currently in range.
        foreach (Pedestal pedestalInRange in pedestalsInRange)
        {
            float distance = Vector3.Distance(transform.position, pedestalInRange.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                snapPedestal = pedestalInRange;
            }
        }

        // Snap the cube to the chosen pedestal if one was found.
        if (snapPedestal != null)
        {
            transform.position = snapPedestal.snapLocator.position;
            transform.rotation = snapPedestal.snapLocator.rotation;
            snapPedestal.SetLetterCube(this);
            rb.isKinematic = true; // Make the cube kinematic so it doesn't fall off.
            myPedestal = snapPedestal;
        }
    }
}