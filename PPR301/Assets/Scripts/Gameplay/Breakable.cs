// ==========================================================================
// Meowt of Tune - Breakable
// Owned and contributed to by: brookcoli (Itch.io) - Aurorafume (GitHub), donkzilla03 (Itch.io) - AliAK03 (GitHub), JaydenFielderTorrens (Itch.io) - Squib35 (GitHub), komorebimoriko (Itch.io), TheRealCrizz (GitHub), LordVGahn (GitHub)
// itch.io listing: https://brookcoli.itch.io/meowt-of-tune - GitHub Repository: https://github.com/Aurorafume/PPR301
// ==========================================================================
//
// WHAT DOES THIS DO:
// This script makes a GameObject destructible based on the force of a physical
// collision. When the object is hit hard enough, it destroys itself and can
// optionally spawn another object in its place, such as a key hidden inside a
// breakable pot.
//
// Core functionalities include:
// - Detecting collision forces.
// - Breaking when the impact force exceeds a defined threshold.
// - Spawning a "revealed" object at a specific location upon breaking.
// - Ensuring the object only breaks once.
//
// Dependencies:
// - UnityEngine for core and physics functionality.
// - A Rigidbody component to detect and measure collision forces.
// - A Collider component to define its physical shape.
//
// ==========================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Makes an object destructible upon receiving a high-force impact.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Breakable : MonoBehaviour
{
    [Header("Break Settings")]
    [Tooltip("The minimum collision force (relative velocity magnitude) required to break this object.")]
    public float breakForceThreshold = 10f;
    [Tooltip("(Optional) The GameObject prefab to spawn when this object breaks.")]
    public GameObject objectToReveal;
    [Tooltip("(Optional) The Transform defining the position where the revealed object will be spawned.")]
    public Transform revealLocation;

    // A cached reference to this object's Rigidbody.
    private Rigidbody rb;
    // A flag to ensure the break logic only runs once.
    private bool hasBroken = false;

    /// <summary>
    /// Called by Unity on startup to cache the Rigidbody component.
    /// </summary>
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Called by Unity's physics engine on collision. Checks if the impact force is sufficient to break the object.
    /// </summary>
    /// <param name="collision">Collision data from the physics engine.</param>
    void OnCollisionEnter(Collision collision)
    {
        // If the object has already been broken, do nothing.
        if (hasBroken) return;

        // Approximate the impact force using the magnitude of the relative velocity.
        float impactForce = collision.relativeVelocity.magnitude;

        // If the force meets or exceeds the threshold, break the object.
        if (impactForce >= breakForceThreshold)
        {
            BreakObject();
        }
    }

    /// <summary>
    /// Handles the object's destruction, reveals a hidden item, and removes itself from the scene.
    /// </summary>
    void BreakObject()
    {
        // Set the flag to prevent this from running multiple times from a single impact event.
        hasBroken = true;

        // If an object and location are specified, spawn the revealed object.
        if (objectToReveal != null && revealLocation != null)
        {
            Instantiate(objectToReveal, revealLocation.position, Quaternion.identity);
        }

        // Destroy this breakable object.
        Destroy(gameObject);
    }
}