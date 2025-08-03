// ==========================================================================
// Meowt of Tune - Carriable
// Owned and contributed to by: brookcoli (Itch.io) - Aurorafume (GitHub), donkzilla03 (Itch.io) - AliAK03 (GitHub), JaydenFielderTorrens (Itch.io) - Squib35 (GitHub), komorebimoriko (Itch.io), TheRealCrizz (GitHub), LordVGahn (GitHub)
// itch.io listing: https://brookcoli.itch.io/meowt-of-tune - GitHub Repository: https://github.com/Aurorafume/PPR301
// ==========================================================================
//
// WHAT DOES THIS DO:
// This script is a component that makes a GameObject able to be picked up,
// carried, and dropped by the player. It is designed to work alongside an
// 'Interactable' script. It handles all the physical changes required for a
// smooth carrying behaviour, such as parenting the object to the player and
// managing its physics and collider states.
//
// Core functionalities include:
// - Toggling between "carried" and "dropped" states upon interaction.
// - Attaching the object to a specific "mouth" transform on the player.
// - Disabling physics and setting colliders to triggers while held to prevent
//   buggy physical behaviours.
// - Restoring physics and colliders when the object is dropped.
// - An object type system to allow for different item behaviours in the future.
// - Failsafe logic to correctly reset the state if the object is disabled.
//
// Dependencies:
// - Must be on a GameObject that also has an 'Interactable' component.
// - The object should have a Rigidbody and at least one Collider.
// - A 'PlayerInteractHandler' must be present in the scene to provide the
//   attachment point for the object.
//
// ==========================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines the behaviour for an object that can be carried and dropped by the player.
/// </summary>
public class Carriable : MonoBehaviour
{
    [Header("Object Type")]
    [Tooltip("The type of object, which can define special behaviours.")]
    public ObjectType objectType;

    [Header("Hold Parameters")]
    [Tooltip("Offset from the position of the player's mouth when held.")]
    [SerializeField] Vector3 holdPositionOffset;
    [Tooltip("Orientation of the object when held.")]
    [SerializeField] Vector3 holdOrientation;

    public SoundEffects soundEffects;

    /// <summary>
    /// Defines the category of the carriable object.
    /// </summary>
    public enum ObjectType
    {
        ordinary,
        key,
    }

    private bool held; // Tracks if the object is currently being held.

    // --- Cached Components & References ---
    private Transform mouth;
    private Collider[] colliders;
    private Interactable myInteractable;
    private PlayerInteractHandler playerInteractHandler;
    private Rigidbody rb;

    /// <summary>
    /// Caches component references on this and other GameObjects.
    /// </summary>
    void Awake()
    {
        myInteractable = GetComponent<Interactable>();
        playerInteractHandler = FindObjectOfType<PlayerInteractHandler>();
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Caches references that may rely on objects initialised in Awake.
    /// </summary>
    void Start()
    {
        colliders = GetComponentsInChildren<Collider>();
        mouth = playerInteractHandler.mouth;
        soundEffects = GameObject.Find("Sound effects").GetComponent<SoundEffects>();
    }

    /// <summary>
    /// The main entry point called by the 'Interactable' script. Toggles between carrying and dropping.
    /// </summary>
    public void OnInteraction()
    {
        if (held)
        {
            Drop();
        }
        else
        {
            Carry();
        }
    }

    /// <summary>
    /// Configures the object to be carried by the player.
    /// </summary>
    void Carry()
    {
        //play key sound
        soundEffects.soundEffects[2].Play();
        // Update state flags.
        myInteractable.SetAwaitingFurtherInteraction(true);
        held = true;

        // Attach the object to the player's mouth and set its local position/rotation.
        transform.parent = mouth;
        transform.localPosition = holdPositionOffset;
        transform.localEulerAngles = holdOrientation;

        // Apply changes based on the object's type.
        if (objectType == ObjectType.ordinary)
        {
            SetOrdinaryChanges();
        }
        if (objectType == ObjectType.key)
        {
            SetKeyChanges();
        }

        // Disable physics while the object is being carried.
        if (rb)
        {
            rb.isKinematic = true;
        }
    }

    /// <summary>
    /// Sets the physical properties for an 'ordinary' object being carried.
    /// </summary>
    void SetOrdinaryChanges()
    {
        // Set all colliders to triggers to prevent physical collision with the environment.
        foreach(Collider collider in colliders)
        {
            collider.isTrigger = true;
        }
    }

    /// <summary>
    /// Sets the physical properties for a 'key' object being carried.
    /// </summary>
    void SetKeyChanges()
    {
        // Note: Currently identical to ordinary changes, but structured for future expansion.
        foreach(Collider collider in colliders)
        {
            collider.isTrigger = true;
        }
    }

    /// <summary>
    /// Releases the object from the player's grasp and restores its physical properties.
    /// </summary>
    void Drop()
    {
        // Update state flags.
        myInteractable.SetAwaitingFurtherInteraction(false);
        held = false;

        // Detach the object from the player.
        transform.parent = null;

        // Restore all colliders to their non-trigger state.
        foreach(Collider collider in colliders)
        {
            collider.enabled = true;
            collider.isTrigger = false;
        }

        // Re-enable physics so the object can fall and settle.
        if (rb)
        {
            rb.isKinematic = false;
        }
    }

    /// <summary>
    /// A failsafe that runs if the object is disabled while being held.
    /// </summary>
    void OnDisable()
    {
        // Ensure the interaction handler's state is reset to prevent the player getting stuck.
        if (held)
        {
            myInteractable.SetAwaitingFurtherInteraction(false);
        }
    }
}