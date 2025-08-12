// ==========================================================================
// Meowt of Tune - Draggable
// Owned and contributed to by: brookcoli (Itch.io) - Aurorafume (GitHub), donkzilla03 (Itch.io) - AliAK03 (GitHub), JaydenFielderTorrens (Itch.io) - Squib35 (GitHub), komorebimoriko (Itch.io), TheRealCrizz (GitHub), LordVGahn (GitHub)
// itch.io listing: https://brookcoli.itch.io/meowt-of-tune - GitHub Repository: https://github.com/Aurorafume/PPR301
// ==========================================================================
//
// WHAT DOES THIS DO:
// This script allows a physics-based object to be dragged by the player. It
// creates a physical connection using a 'ConfigurableJoint' component, which
// results in a more dynamic and realistic dragging behaviour. This is ideal
// for physics puzzles or interacting with the environment by pushing and
// pulling objects.
//
// Core functionalities include:
// - Toggling between "grabbed" and "dropped" states upon player interaction.
// - Dynamically creating and destroying a ConfigurableJoint to manage the link.
// - Detailed configuration of the joint's limits, spring, and damper values
//   for fine-tuned physical behaviour.
// - Using the precise point of player interaction as the joint's anchor point.
// - Invoking a C# event to inform other game systems of the drag state.
//
// Dependencies:
// - Must be on a GameObject with an 'Interactable' component and a Rigidbody.
// - Requires a 'PlayerInteractHandler' to be present in the scene.
// - The player GameObject must have a Rigidbody for the joint to connect to.
//
// ==========================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Defines the behaviour for a physics object that can be dragged by the player using a joint.
/// </summary>
public class Draggable : MonoBehaviour
{
    [Header("Drag Parameters")]
    [Tooltip("How far the object can move from the grab point.")]
    [SerializeField] float jointLimit = 0.1f;
    [Tooltip("The spring force of the joint, affecting how strongly it pulls towards the grab point.")]
    [SerializeField] float jointSpring = 100f;
    [Tooltip("The damper of the joint, affecting resistance to movement.")]
    [SerializeField] float jointDamper = 5f;
    
    // --- State & Component References ---
    public bool grabbed; // Tracks if the object is currently being grabbed.
    private Vector3 grabPoint; // The world-space point where the player grabbed the object.
    private ConfigurableJoint joint; // The joint component created at runtime.
    private Interactable myInteractable;
    private PlayerInteractHandler playerInteractHandler;
    private PlayerMovement movement;
    private Rigidbody rb;
    private bool defaultFreezeRotation; // Stores the initial freezeRotation state of the Rigidbody.
    public SoundEffects soundEffects;

    /// <summary>
    /// Event invoked when an object starts or stops being dragged.
    /// Bool: True if dragging has started, false if it has stopped.
    /// </summary>
    public static event Action<bool> OnDragObject;

    /// <summary>
    /// Caches components and stores initial Rigidbody state.
    /// </summary>
    void Awake()
    {
        myInteractable = GetComponent<Interactable>();
        playerInteractHandler = FindObjectOfType<PlayerInteractHandler>();
        rb = GetComponent<Rigidbody>();
        movement = FindObjectOfType<PlayerMovement>();
        
        // Store the default rotation constraint to restore it later.
        defaultFreezeRotation = rb.freezeRotation;

        //assign sound effects
        soundEffects = GameObject.Find("Sound effects").GetComponent<SoundEffects>();
    }

    /// <summary>
    /// The main entry point called by the 'Interactable' script. Toggles the drag state.
    /// </summary>
    public void OnInteraction()
    {
        if (grabbed)
        {
            Drop();
        }
        else
        {
            Drag();
        }
    }

    /// <summary>
    /// Initiates the drag behaviour by creating and configuring a joint.
    /// </summary>
    void Drag()
    {
        if(movement.grounded == true)
        {
            //play sound effect
            soundEffects.Meow();
            myInteractable.SetAwaitingFurtherInteraction(true);
            grabbed = true;
            OnDragObject?.Invoke(true); // Notify other systems that dragging has started.
            rb.freezeRotation = true; // Temporarily freeze rotation to prevent wild spinning.

            // Get the precise point where the player's interaction raycast hit the object.
            grabPoint = playerInteractHandler.hitPoint;

            // Create a new joint if one doesn't already exist.
            if (!joint)
            {
                joint = gameObject.AddComponent<ConfigurableJoint>();
                ConfigureJoint();
            }
        }
    }

    /// <summary>
    /// Sets all the physics parameters for the ConfigurableJoint.
    /// </summary>
    void ConfigureJoint()
    {
        // Set linear motion to limited, creating a "leash" effect.
        joint.xMotion = ConfigurableJointMotion.Limited;
        joint.yMotion = ConfigurableJointMotion.Limited;
        joint.zMotion = ConfigurableJointMotion.Limited;
        // Allow the object to be freely rotated by the player.
        joint.angularXMotion = ConfigurableJointMotion.Free;
        joint.angularYMotion = ConfigurableJointMotion.Free;
        joint.angularZMotion = ConfigurableJointMotion.Free;

        // Define the "leash" length.
        SoftJointLimit limit = new SoftJointLimit();
        limit.limit = jointLimit;
        joint.linearLimit = limit;

        // Define the spring-like force that pulls the object towards the player's grab point.
        JointDrive drive = new JointDrive();
        drive.positionSpring = jointSpring;
        drive.positionDamper = jointDamper;
        drive.maximumForce = Mathf.Infinity;
        joint.xDrive = drive;
        joint.yDrive = drive;
        joint.zDrive = drive;

        // Connect the joint to the player's Rigidbody.
        joint.connectedBody = playerInteractHandler.gameObject.GetComponent<Rigidbody>();
        // Set the joint's anchor to the specific point on the object that was grabbed.
        joint.anchor = transform.InverseTransformPoint(grabPoint);
        // Make the joint unbreakable.
        joint.breakForce = Mathf.Infinity;
    }

    /// <summary>
    /// Releases the object by destroying the joint and resetting its state.
    /// </summary>
    void Drop()
    {
        myInteractable.SetAwaitingFurtherInteraction(false);
        grabbed = false;
        OnDragObject?.Invoke(false); // Notify other systems that dragging has ended.
        rb.freezeRotation = defaultFreezeRotation; // Restore the original rotation constraint.

        // Destroy the joint component to sever the physical connection.
        if (joint)
        {
            Destroy(joint);
        }
    }
}