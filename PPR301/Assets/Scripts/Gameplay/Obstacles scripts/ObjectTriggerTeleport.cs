// ==========================================================================
// Meowt of Tune - Object Trigger Teleport
// Owned and contributed to by: brookcoli (Itch.io) - Aurorafume (GitHub), donkzilla03 (Itch.io) - AliAK03 (GitHub), JaydenFielderTorrens (Itch.io) - Squib35 (GitHub), komorebimoriko (Itch.io), TheRealCrizz (GitHub), LordVGahn (GitHub)
// itch.io listing: https://brookcoli.itch.io/meowt-of-tune - GitHub Repository: https://github.com/Aurorafume/PPR301
// ==========================================================================
//
// WHAT DOES THIS DO:
// This script creates a one-time trigger volume that activates a specific
// sequence of events. When a designated object enters the trigger, this script
// will teleport another object to a new location and cause one or more
// other objects to disappear.
//
// Core functionalities include:
// - A trigger that responds only to a specific, assigned GameObject.
// - Teleporting a target object to a predefined destination.
// - Deactivating an optional object to make it disappear.
// - Deactivating the trigger object itself to ensure the event happens only once.
//
// Dependencies:
// - UnityEngine for core game object and physics functionality.
// - A Collider component on the same GameObject, with its "Is Trigger" property
//   enabled.
//
// ==========================================================================

using UnityEngine;

/// <summary>
/// A trigger that teleports an object and deactivates others when a specific object enters it.
/// </summary>
public class ObjectTriggerTeleport : MonoBehaviour
{
    [Header("Trigger & Object Configuration")]
    [Tooltip("The specific GameObject that must enter this trigger to activate the event.")]
    public GameObject triggerObject;
    [Tooltip("The object that will be moved to the target location.")]
    public Transform objectToTeleport;
    [Tooltip("The destination Transform where the object will be teleported.")]
    public Transform teleportTarget;
    [Tooltip("(Optional) A GameObject to set inactive when the trigger is activated.")]
    public GameObject objectToDisappear;

    /// <summary>
    /// Called by Unity when a collider enters this object's trigger volume.
    /// </summary>
    /// <param name="other">The collider that entered the trigger.</param>
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that entered is the designated trigger object.
        if (other.gameObject == triggerObject)
        {
            // Teleport the target object to the destination's position.
            if (objectToTeleport != null && teleportTarget != null)
            {
                objectToTeleport.position = teleportTarget.position;
            }

            // Disable the object that is meant to disappear.
            if (objectToDisappear != null)
            {
                objectToDisappear.SetActive(false);
            }

            // Disable the trigger object itself to make this a one-time event.
            if (triggerObject != null)
            {
                triggerObject.SetActive(false);
            }
        }
    }
}