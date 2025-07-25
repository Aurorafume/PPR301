// ==========================================================================
// Lock
// Owned and contributed to by: brookcoli (Itch.io) - Aurorafume (GitHub), donkzilla03 (Itch.io) - AliAK03 (GitHub), JaydenFielderTorrens (Itch.io) - Squib35 (GitHub), komorebimoriko (Itch.io), TheRealCrizz (GitHub), LordVGahn (GitHub)
// itch.io listing: https://brookcoli.itch.io/meowt-of-tune - GitHub Repository: https://github.com/Aurorafume/PPR301
// ==========================================================================
//
// WHAT DOES THIS DO:
// This script controls a lock mechanism for a door in a Unity game. When the
// player brings the correct key object into the lock's trigger collider, the
// script deactivates the associated door, consumes the key, and removes the
// lock from the scene.
//
// Core functionalities include:
// - Detecting objects entering its trigger collider.
// - Identifying if the object is a carriable key.
// - Matching the key's type (via its tag) to the lock's required type.
// - Executing an unlock sequence to open a path for the player.
//
// Dependencies:
// - UnityEngine for component and physics functionality.
// - A "Carriable" custom script on the key object.
// - A trigger Collider component on the same GameObject as this script.
//
// ==========================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages a lock that can be opened by a specific key, which in turn deactivates a door.
/// </summary>
public class Lock : MonoBehaviour
{
    [Header("Lock & Door Configuration")]
    [Tooltip("The door GameObject that this lock will disable upon unlocking.")]
    public GameObject door;
    [Tooltip("The type of key required to open this lock.")]
    public LockType lockType;
    
    // A private reference to the key object that enters the trigger.
    private GameObject key;

    /// <summary>
    /// Defines the different types of locks and their corresponding keys.
    /// </summary>
    public enum LockType
    {
        GreenLock,
        BlueLock
    }

    /// <summary>
    /// Called when a collider enters the lock's trigger area. Checks for a valid key.
    /// </summary>
    /// <param name="collider">The collider that entered the trigger.</param>
    void OnTriggerEnter(Collider collider)
    {
        // Find the Carriable component on the root object that entered the trigger.
        Carriable carriable = collider.transform.root.GetComponentInChildren<Carriable>();

        if (carriable)
        {
            // Check if the carriable object is specifically a key.
            if (carriable.objectType == Carriable.ObjectType.key)
            {
                key = carriable.gameObject;

                // Check if the key's tag matches the required lock type.
                if (key.CompareTag("Blue Key") && lockType == LockType.BlueLock)
                {
                    Debug.Log("Correct Key: Opening Blue Door.");
                    Unlock();
                }
                else if (key.CompareTag("Green Key") && lockType == LockType.GreenLock)
                {
                    Debug.Log("Correct Key: Opening Green Door.");
                    Unlock();
                }
                else
                {
                    Debug.Log("Wrong key for this lock.");
                }
            }
        }
    }

    /// <summary>
    /// Performs the unlock sequence: destroys the key, deactivates the door, and removes the lock.
    /// </summary>
    void Unlock()
    {
        // Consume the key.
        Destroy(key);
        // "Open" the door by deactivating it.
        door.SetActive(false);
        // Remove the lock from the scene.
        Destroy(gameObject);
    }
}