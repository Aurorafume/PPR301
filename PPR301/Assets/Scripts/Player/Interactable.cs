// ==========================================================================
// Meowt of Tune - Interactable
// Owned and contributed to by: brookcoli (Itch.io) - Aurorafume (GitHub), donkzilla03 (Itch.io) - AliAK03 (GitHub), JaydenFielderTorrens (Itch.io) - Squib35 (GitHub), komorebimoriko (Itch.io), TheRealCrizz (GitHub), LordVGahn (GitHub)
// itch.io listing: https://brookcoli.itch.io/meowt-of-tune - GitHub Repository: https://github.com/Aurorafume/PPR301
// ==========================================================================
//
// WHAT DOES THIS DO:
// This is a universal component that makes any GameObject respond to player
// interaction. It acts as a bridge between the player's action and the
// object's specific reaction. It uses a UnityEvent to trigger custom
// behaviours defined on other scripts (like picking up an object, flipping a
// switch, or starting a dialogue).
//
// Core functionalities include:
// - A public UnityEvent ('OnInteraction') that can be configured in the
//   Inspector to call methods on any other component.
// - An 'Interact()' method, designed to be called by the 'PlayerInteractHandler',
//   which in turn triggers the event.
// - A method for other scripts to report their state (e.g. "in use") back
//   to the 'PlayerInteractHandler' to manage ongoing interactions.
//
// Dependencies:
// - Requires a 'PlayerInteractHandler' in the scene to call its 'Interact()' method.
// - Designed to work with other scripts (e.g. 'Carriable', 'Draggable') that
//   contain the actual interaction logic hooked into the 'OnInteraction' event.
// - The GameObject must have a Collider for the player's raycast to detect it.
//
// ==========================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A universal component that allows a GameObject to be interacted with by the player.
/// </summary>
public class Interactable : MonoBehaviour
{
    // A cached reference to the player's interaction handler.
    PlayerInteractHandler playerInteractHandler;

    /// <summary>
    /// A configurable event that is invoked when the player interacts with this object.
    /// Assign methods from other scripts to this event in the Unity Inspector.
    /// </summary>
    public UnityEvent OnInteraction;

    /// <summary>
    /// Caches a reference to the PlayerInteractHandler on startup.
    /// </summary>
    void Awake()
    {
        playerInteractHandler = FindObjectOfType<PlayerInteractHandler>();
    }

    /// <summary>
    /// This is the main entry point for an interaction, typically called by the PlayerInteractHandler.
    /// It invokes all functions assigned to the OnInteraction event.
    /// </summary>
    public void Interact()
    {
        OnInteraction.Invoke();
    }

    /// <summary>
    /// Allows other scripts to report back to the PlayerInteractHandler that this object
    /// is "in use" (e.g. being carried), which may require further interaction to release.
    /// </summary>
    /// <param name="waiting">True if the object is in an ongoing interaction, otherwise false.</param>
    public void SetAwaitingFurtherInteraction(bool waiting)
    {
        if (waiting)
        {
            // Tell the handler that this object is now the one in use.
            playerInteractHandler.SetInteractableInUse(this);
        }
        else
        {
            // Clear the "in use" status from the handler.
            playerInteractHandler.SetInteractableInUse(null);
        }
    }
}