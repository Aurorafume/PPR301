// ==========================================================================
// Meowt of Tune - Mouse Key Carrier
// Owned and contributed to by: brookcoli (Itch.io) - Aurorafume (GitHub), donkzilla03 (Itch.io) - AliAK03 (GitHub), JaydenFielderTorrens (Itch.io) - Squib35 (GitHub), komorebimoriko (Itch.io), TheRealCrizz (GitHub), LordVGahn (GitHub)
// itch.io listing: https://brookcoli.itch.io/meowt-of-tune - GitHub Repository: https://github.com/Aurorafume/PPR301
// ==========================================================================
//
// WHAT DOES THIS DO:
// This script allows a mouse character to carry a key object. When the mouse
// is touched by the player, it drops the key, making it available for the
// player to collect. Dropping the key also triggers other specific game
// events, such as changing another object's material and disabling the
// player's night vision effect.
//
// Core functionalities include:
// - Attaching a specified key object to a "mouth" position on the mouse.
// - Disabling the key's physics while carried to prevent unwanted movement.
// - Using a trigger collider to detect when the player makes contact.
// - Dropping the key by detaching it and re-enabling its physics.
// - Triggering secondary gameplay consequences when the key is dropped.
//
// Dependencies:
// - Must be attached to the mouse character that carries the key.
// - Requires several GameObjects to be assigned in the Inspector: 'carriedKey',
//   'obj' (for the material swap), and 'nightVision'.
// - The 'carriedKey' GameObject should have a Rigidbody component.
// - The player GameObject must have a Collider and be tagged correctly.
//
// ==========================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages a key carried by a mouse, dropping it upon player contact
/// and triggering related game events.
/// </summary>
public class MouseKey : MonoBehaviour
{
    [Header("Key Settings")]
    [Tooltip("Reference to the key GameObject the mouse is carrying.")]
    public GameObject carriedKey;

    [Tooltip("Local position offset for the key in the mouse's mouth.")]
    public Vector3 mouthOffset = new Vector3(0, 0.2f, 0.3f);

    [Tooltip("Tag used to identify the player object for interaction.")]
    public string playerTag = "Player";

    [Header("Game Event Objects")]
    [Tooltip("The material to apply to the 'obj' GameObject when the key is dropped.")]
    public Material glowMaterial;
    [Tooltip("The GameObject whose material will be changed.")]
    public GameObject obj;
    [Tooltip("The night vision effect GameObject to be disabled.")]
    public GameObject nightVision;

    private bool hasKey = true; // Tracks whether the mouse is currently holding the key.

    /// <summary>
    /// Initialises the component by attaching the key to the mouse.
    /// </summary>
    void Start()
    {
        // Ensure a key has been assigned in the inspector before trying to attach it.
        if (carriedKey != null)
        {
            AttachKeyToMouth();
        }
    }

    /// <summary>
    /// Called when another collider enters this object's trigger volume.
    /// </summary>
    /// <param name="other">The collider that entered the trigger.</param>
    private void OnTriggerEnter(Collider other)
    {
        // Do nothing if the mouse has already dropped the key.
        if (!hasKey) return;
        
        // If the colliding object is the player, drop the key.
        if (other.CompareTag(playerTag))
        {
            DropKey();
        }
    }

    /// <summary>
    /// Attaches the key to the mouse, parenting it and disabling its physics.
    /// </summary>
    void AttachKeyToMouth()
    {
        // Parent the key to the mouse so it moves with it.
        carriedKey.transform.SetParent(transform);
        // Position and orient the key correctly in the "mouth".
        carriedKey.transform.localPosition = mouthOffset;
        carriedKey.transform.localRotation = Quaternion.identity;

        // Disable the key's physics so it doesn't fall or collide while being carried.
        if (carriedKey.TryGetComponent(out Rigidbody rb))
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    /// <summary>
    /// Detaches the key from the mouse and triggers associated game events.
    /// </summary>
    void DropKey()
    {
        // Un-parent the key, making it an independent object in the scene.
        carriedKey.transform.SetParent(null);

        // Re-enable the key's physics so it can fall to the ground and be interacted with.
        if (carriedKey.TryGetComponent(out Rigidbody rb))
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        // Update state to prevent this from running again.
        hasKey = false;
        Debug.Log("Mouse dropped the key!");

        // --- Trigger secondary game events ---
        // Change the material of the target object.
        obj.GetComponent<MeshRenderer>().material = glowMaterial;
        // Disable the cat's night vision effect.
        nightVision.SetActive(false);
    }
}