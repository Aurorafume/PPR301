// ==========================================================================
// Meowt of Tune - Hover Effect Text
// Owned and contributed to by: brookcoli (Itch.io) - Aurorafume (GitHub), donkzilla03 (Itch.io) - AliAK03 (GitHub), JaydenFielderTorrens (Itch.io) - Squib35 (GitHub), komorebimoriko (Itch.io), TheRealCrizz (GitHub), LordVGahn (GitHub)
// itch.io listing: https://brookcoli.itch.io/meowt-of-tune - GitHub Repository: https://github.com/Aurorafume/PPR301
// ==========================================================================
//
// WHAT DOES THIS DO:
// This is a UI component that creates a hover effect for a TextMeshPro text
// element. It changes the text's font size and colour when the mouse pointer
// moves over the UI element this script is attached to.
//
// Core functionalities include:
// - Implementing IPointerEnterHandler and IPointerExitHandler to detect hover events.
// - Allowing customisation of font size and colour for both normal and hover states.
// - Parsing hex colour codes into Unity 'Color' objects with safe fallbacks.
//
// Dependencies:
// - Must be attached to a UI element with a Raycast Target enabled.
// - Requires an Event System component to be present in the scene.
// - Requires a TextMeshPro text component to be either assigned in the Inspector
//   or exist as a child of this GameObject.
//
// ==========================================================================

using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

/// <summary>
/// Creates a hover effect on a TextMeshPro UI element by changing its font size and colour.
/// </summary>
public class HoverEffectText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI Target")]
    [Tooltip("The TextMeshPro text element to apply the effect to. Will find one in children if not set.")]
    public TMP_Text targetText; 

    [Header("Style Properties")]
    [Tooltip("The font size of the text in its normal state.")]
    public int normalFontSize = 80;
    [Tooltip("The font size of the text when hovered over.")]
    public int hoverFontSize = 90;
    [Tooltip("The hex colour code for the text in its normal state (e.g. #96A85F).")]
    public string normalHex = "#96A85F";
    [Tooltip("The hex colour code for the text when hovered over (e.g. #536322).")]
    public string hoverHex = "#536322";

    // --- Private State Variables ---
    private Color normalColour; // The parsed Color object for the normal state.
    private Color hoverColour;  // The parsed Color object for the hover state.

    /// <summary>
    /// Initialises the component, finds the text object, parses colours, and sets the initial state.
    /// </summary>
    private void Start()
    {
        // If no text component is assigned, try to find one in the children of this object.
        if (targetText == null)
        {
            targetText = GetComponentInChildren<TMP_Text>();
        }

        // Parse the hex string into a Color object, using white as a fallback.
        if (!ColorUtility.TryParseHtmlString(normalHex, out normalColour))
        {
            normalColour = Color.white;
        }
            
        // Parse the hex string into a Color object, using yellow as a fallback.
        if (!ColorUtility.TryParseHtmlString(hoverHex, out hoverColour))
        {
            hoverColour = Color.yellow;
        }

        // Set the initial appearance to the normal state.
        ApplyNormal();
    }

    /// <summary>
    /// Called by the Event System when the mouse cursor enters this UI element's bounds.
    /// </summary>
    /// <param name="eventData">Data associated with the pointer event.</param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        ApplyHover();
    }

    /// <summary>
    /// Called by the Event System when the mouse cursor exits this UI element's bounds.
    /// </summary>
    /// <param name="eventData">Data associated with the pointer event.</param>
    public void OnPointerExit(PointerEventData eventData)
    {
        ApplyNormal();
    }

    /// <summary>
    /// Applies the visual properties for the hover state to the target text.
    /// </summary>
    void ApplyHover()
    {
        if (targetText != null)
        {
            targetText.fontSize = hoverFontSize;
            targetText.color = hoverColour;
        }
    }

    /// <summary>
    /// Applies the visual properties for the normal (non-hover) state to the target text.
    /// </summary>
    void ApplyNormal()
    {
        if (targetText != null)
        {
            targetText.fontSize = normalFontSize;
            targetText.color = normalColour;
        }
    }
}