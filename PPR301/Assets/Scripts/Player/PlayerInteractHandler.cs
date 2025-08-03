// ==========================================================================
// Meowt of Tune - Player Interact Handler
// Owned and contributed to by: brookcoli (Itch.io) - Aurorafume (GitHub), donkzilla03 (Itch.io) - AliAK03 (GitHub), JaydenFielderTorrens (Itch.io) - Squib35 (GitHub), komorebimoriko (Itch.io), TheRealCrizz (GitHub), LordVGahn (GitHub)
// itch.io listing: https://brookcoli.itch.io/meowt-of-tune - GitHub Repository: https://github.com/Aurorafume/PPR301
// ==========================================================================
//
// WHAT DOES THIS DO:
// This script is the primary controller for the player's ability to interact
// with objects in the world. It uses a sphere cast to detect nearby objects
// on a specific layer, handles player input to initiate an interaction, and
// manages visual feedback cues like an interaction icon. It is the central
// hub that triggers behaviours on 'Interactable' components.
//
// Core functionalities include:
// - Detecting nearby interactable objects using a forgiving Physics.SphereCast.
// - Handling player input to trigger an interaction.
// - Managing an "in use" state for ongoing interactions (e.g. carrying an object).
// - Showing and hiding a context-sensitive interaction icon on targeted objects.
// - Communicating with 'Interactable' components to start their behaviours.
// - A debug gizmo for visualising the interaction range in the editor.
//
// Dependencies:
// - Must be attached to the player character.
// - Requires an 'interactPointLocator' transform to define the sphere cast origin.
// - Relies on world objects having an 'Interactable' component and being on the
//   correct 'interactableLayer'.
// - Optionally works with 'ShowIconScript' on interactable objects for feedback.
//
// ==========================================================================

using UnityEngine;

/// <summary>
/// Manages player input and raycasting to detect and trigger interactions with objects.
/// </summary>
public class PlayerInteractHandler : MonoBehaviour
{
    [Header("Interact Parameters")]
    [Tooltip("The radius of the sphere cast used to detect interactables.")]
    [SerializeField] float grabRadius = 0.5f;
    [Tooltip("The distance the sphere cast travels forward to detect interactables.")]
    [SerializeField] float grabDistance = 1f;
    [Tooltip("If true, a debug gizmo will be drawn in the editor to show the grab area.")]
    [SerializeField] bool showGrabAreaGizmo;
    
    [Header("References")]
    [Tooltip("The key used to trigger an interaction.")]
    [SerializeField] KeyCode interactButton = KeyCode.Mouse0;
    [Tooltip("The physics layer that interactable objects belong to.")]
    [SerializeField] LayerMask interactableLayer;
    [Tooltip("The transform from which the interaction sphere cast originates.")]
    [SerializeField] Transform interactPointLocator;
    [Tooltip("The transform where carriable objects will be attached.")]
    public Transform mouth;

    // --- Private State Variables ---
    private RaycastHit hit; // Stores information about the detected object.
    private bool interactableDetected; // True if the sphere cast finds an interactable object.
    private Interactable interactableInUse; // A reference to the object currently being interacted with (e.g. held).
    private ShowIconScript activeClickIcon; // A reference to the currently visible interaction icon.

    /// <summary>
    /// The precise world-space point where the interaction raycast hit an object.
    /// </summary>
    [HideInInspector] public Vector3 hitPoint;

    /// <summary>
    /// Draws a debug gizmo in the Scene view to visualise the interaction detection area.
    /// </summary>
    void OnDrawGizmos()
    {
        if (!showGrabAreaGizmo || interactPointLocator == null) return;

        Gizmos.color = Color.cyan;
        Vector3 start = interactPointLocator.position;
        Vector3 end = start + transform.forward * grabDistance;

        // Draw spheres at the start and end of the cast.
        Gizmos.DrawWireSphere(start, grabRadius);
        Gizmos.DrawWireSphere(end, grabRadius);

        // Draw lines to approximate the shape of the cylinder.
        Gizmos.DrawLine(start + Vector3.up * grabRadius, end + Vector3.up * grabRadius);
        Gizmos.DrawLine(start - Vector3.up * grabRadius, end - Vector3.up * grabRadius);
        Gizmos.DrawLine(start + Vector3.right * grabRadius, end + Vector3.right * grabRadius);
        // This line is corrected to use 'grabRadius' instead of the non-existent 'radius'.
        Gizmos.DrawLine(start - Vector3.right * grabRadius, end - Vector3.right * grabRadius);
    }

    /// <summary>
    /// Main update loop. Orchestrates input detection and UI feedback every frame.
    /// </summary>
    void Update()
    {
        HandleInput();
        DetectInteractable();
        TryShowClickIcon();
    }

    /// <summary>
    /// Processes the interact key press and decides whether to start a new interaction or continue an existing one.
    /// </summary>
    void HandleInput()
    {
        if (Input.GetKeyDown(interactButton))
        {
            // If an object is already "in use" (e.g. being carried), interact with it again (e.g. to drop it).
            if (interactableInUse)
            {
                interactableInUse.Interact();
            }
            // Otherwise, try to start a new interaction with an object in front of the player.
            else
            {
                TryInteract();
            }
        }
    }

    /// <summary>
    /// Performs a sphere cast forward from the player to detect nearby interactable objects.
    /// </summary>
    void DetectInteractable()
    {
        interactableDetected = Physics.SphereCast(interactPointLocator.position, grabRadius,
                                                  transform.forward, out hit,
                                                  grabDistance, interactableLayer);
    }

    /// <summary>
    /// Manages the visibility of the interaction icon based on the detection state.
    /// </summary>
    void TryShowClickIcon()
    {
        // If an object is already in use, no new interaction icons should be shown.
        if (interactableInUse)
        {
            if (activeClickIcon)
            {
                activeClickIcon.SetIconActive(false);
                activeClickIcon = null;
            }
            return;
        }
        
        // If an interactable is detected in front of the player...
        if (interactableDetected)
        {
            ShowIconScript showIconScript = hit.transform.GetComponentInChildren<ShowIconScript>();

            // If a valid icon script is found, show it.
            if (showIconScript && !activeClickIcon)
            {
                showIconScript.SetIconActive(true);
                activeClickIcon = showIconScript;
            }
            else if (!showIconScript && activeClickIcon)
            {
                activeClickIcon.SetIconActive(false);
                activeClickIcon = null;
            }
        }
        // If nothing is detected, ensure no icon is active.
        else if (activeClickIcon)
        {
            activeClickIcon.SetIconActive(false);
            activeClickIcon = null;
        }
    }

    /// <summary>
    /// Attempts to initiate an interaction with a detected object.
    /// </summary>
    void TryInteract()
    {
        if (interactableDetected)
        {
            Interactable interactable = hit.transform.GetComponent<Interactable>();
            if (interactable)
            {
                // Store the exact point of contact for use by other scripts (e.g. Draggable).
                hitPoint = hit.point;
                // Call the 'Interact' method on the target object's 'Interactable' component.
                interactable.Interact();
            }
        }
    }

    /// <summary>
    /// A public method allowing other scripts to register themselves as the currently active interaction.
    /// </summary>
    /// <param name="myInteractable">The 'Interactable' component of the object now "in use".</param>
    public void SetInteractableInUse(Interactable myInteractable)
    {
        interactableInUse = myInteractable;
    }
}