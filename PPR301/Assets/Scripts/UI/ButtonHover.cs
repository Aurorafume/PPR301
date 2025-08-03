// ==========================================================================
// Meowt of Tune - Button Hover
// Owned and contributed to by: brookcoli (Itch.io) - Aurorafume (GitHub), donkzilla03 (Itch.io) - AliAK03 (GitHub), JaydenFielderTorrens (Itch.io) - Squib35 (GitHub), komorebimoriko (Itch.io), TheRealCrizz (GitHub), LordVGahn (GitHub)
// itch.io listing: https://brookcoli.itch.io/meowt-of-tune - GitHub Repository: https://github.com/Aurorafume/PPR301
// ==========================================================================
//
// WHAT DOES THIS DO:
// This is a UI component that creates a simple hover effect for a button. It
// works by disabling a "dull" version of the button's image when the mouse
// hovers over it. This is presumably to reveal a "bright" or highlighted
// version of the button image that sits underneath it in the UI hierarchy.
//
// Core functionalities include:
// - Implementing IPointerEnterHandler and IPointerExitHandler to detect mouse
//   hover events.
// - Managing a shared array of "dull" images for a menu.
// - Hiding the associated dull image on hover-enter and showing it on hover-exit.
//
// Dependencies:
// - Must be attached to a UI element with a Raycast Target enabled (e.g. a
//   Button or an Image).
// - Requires an Event System component to be present in the scene.
// - The 'dullImages' array and the 'dullImageIndex' for each button must be
//   configured correctly in the Inspector.
//
// ==========================================================================

using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Handles a hover effect for a UI button by hiding and showing a "dull" image overlay.
/// </summary>
public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI References")]
    [Tooltip("An array of all dull image overlays for the buttons in this menu (e.g. 0:Resume, 1:Settings, 2:Leave).")]
    public GameObject[] dullImages;

    [Header("Configuration")]
    [Tooltip("The index in the 'dullImages' array that corresponds to this specific button.")]
    public int dullImageIndex;

    /// <summary>
    /// Called by the Event System when the mouse cursor enters this button's bounds.
    /// </summary>
    /// <param name="eventData">Data associated with the pointer event.</param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Check for valid configuration before attempting to access the array.
        if (dullImages != null && dullImageIndex >= 0 && dullImageIndex < dullImages.Length)
        {
            // Disable this button's corresponding dull image to reveal the bright one underneath.
            dullImages[dullImageIndex].SetActive(false);
        }
    }

    /// <summary>
    /// Called by the Event System when the mouse cursor exits this button's bounds.
    /// </summary>
    /// <param name="eventData">Data associated with the pointer event.</param>
    public void OnPointerExit(PointerEventData eventData)
    {
        // Check for valid configuration before attempting to access the array.
        if (dullImages != null && dullImageIndex >= 0 && dullImageIndex < dullImages.Length)
        {
            // Re-enable the dull image when the mouse leaves.
            dullImages[dullImageIndex].SetActive(true);
        }
    }
}