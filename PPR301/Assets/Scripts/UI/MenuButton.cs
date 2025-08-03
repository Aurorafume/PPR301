// ==========================================================================
// Meowt of Tune - Menu Button
// Owned and contributed to by: brookcoli (Itch.io) - Aurorafume (GitHub), donkzilla03 (Itch.io) - AliAK03 (GitHub), JaydenFielderTorrens (Itch.io) - Squib35 (GitHub), komorebimoriko (Itch.io), TheRealCrizz (GitHub), LordVGahn (GitHub)
// itch.io listing: https://brookcoli.itch.io/meowt-of-tune - GitHub Repository: https://github.com/Aurorafume/PPR301
// ==========================================================================
//
// WHAT DOES THIS DO:
// This is a UI component that creates a simple colour-changing hover effect for
// a TextMeshPro text element that functions as a button. It uses the Unity
// Event System to efficiently detect when the mouse pointer enters and exits
// the button's clickable area.
//
// Core functionalities include:
// - Implementing IPointerEnterHandler and IPointerExitHandler to detect hover events.
// - Changing the text's colour to a "hover" colour when the mouse enters.
// - Reverting the text's colour to its "normal" colour when the mouse exits.
//
// Dependencies:
// - Must be attached to a UI element with a Raycast Target enabled.
// - Requires an Event System component to be present in the scene.
// - Requires a 'theText' (TextMeshPro) component to be assigned in the Inspector.
//
// ==========================================================================

using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// Handles a colour-change hover effect for a TextMeshPro UI button using the Event System.
/// </summary>
public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI References")]
    [Tooltip("The TextMeshPro text element to apply the effect to.")]
    public TMP_Text theText;

    [Header("Colour States")]
    [Tooltip("The colour of the text in its normal, non-hovered state.")]
    public Color normalColour;
    [Tooltip("The colour of the text when the mouse pointer is hovering over it.")]
    public Color hoverColour;

    /// <summary>
    /// Called by the Event System when the mouse cursor enters this UI element's bounds.
    /// </summary>
    /// <param name="eventData">Data associated with the pointer event.</param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        theText.color = hoverColour;
    }

    /// <summary>
    /// Called by the Event System when the mouse cursor exits this UI element's bounds.
    /// </summary>
    /// <param name="eventData">Data associated with the pointer event.</param>
    public void OnPointerExit(PointerEventData eventData)
    {
        theText.color = normalColour;
    }
}