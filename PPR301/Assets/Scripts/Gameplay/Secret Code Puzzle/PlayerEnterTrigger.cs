// ==========================================================================
// Meowt of Tune - Player Enter Trigger
// Owned and contributed to by: brookcoli (Itch.io) - Aurorafume (GitHub), donkzilla03 (Itch.io) - AliAK03 (GitHub), JaydenFielderTorrens (Itch.io) - Squib35 (GitHub), komorebimoriko (Itch.io), TheRealCrizz (GitHub), LordVGahn (GitHub)
// itch.io listing: https://brookcoli.itch.io/meowt-of-tune - GitHub Repository: https://github.com/Aurorafume/PPR301
// ==========================================================================
//
// WHAT DOES THIS DO:
// This is a versatile utility script that creates a trigger zone that fires a
// series of customisable events when the player character enters it. It is ideal
// for scripting level events like opening doors, starting cutscenes, or setting
// off traps without writing new code for each specific scenario.
//
// Core functionalities include:
// - Detecting when an object tagged "Player" enters its trigger volume.
// - Invoking a public UnityEvent, which can be configured in the Inspector.
// - An option to make the trigger a one-time event by destroying itself after use.
//
// Dependencies:
// - UnityEngine and UnityEngine.Events for core and event functionality.
// - A Collider component on the same GameObject, with "Is Trigger" enabled.
// - The player's GameObject must be tagged as "Player".
//
// ==========================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A generic trigger that fires a UnityEvent when the player enters its volume.
/// </summary>
public class PlayerEnterTrigger : MonoBehaviour
{
    [Header("Trigger Settings")]
    [Tooltip("The set of events to invoke when the player enters this trigger.")]
    public UnityEvent OnPlayerEnter;
    [Tooltip("The set of events to invoke when the player exits this trigger.")]
    public UnityEvent OnPlayerExit;
    [Tooltip("If true, this trigger will be destroyed after firing, making it a one-time event.")]
    public bool destroyOnEnter;

    /// <summary>
    /// Called by Unity when a collider enters the trigger volume.
    /// Checks if the entering object is the player.
    /// </summary>
    /// <param name="other">The collider that entered the trigger.</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerEnter();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerExit();
        }
    }

    /// <summary>
    /// Invokes the configured UnityEvent and handles the self-destruction of the trigger.
    /// </summary>
    private void PlayerEnter()
    {
        OnPlayerEnter.Invoke();

        if (destroyOnEnter)
        {
            Destroy(gameObject);
        }
    }

    private void PlayerExit()
    {
        OnPlayerExit.Invoke();
    }
}