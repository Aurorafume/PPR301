// ==========================================================================
// Meowt of Tune - Scroller
// Owned and contributed to by: brookcoli (Itch.io) - Aurorafume (GitHub), donkzilla03 (Itch.io) - AliAK03 (GitHub), JaydenFielderTorrens (Itch.io) - Squib35 (GitHub), komorebimoriko (Itch.io), TheRealCrizz (GitHub), LordVGahn (GitHub)
// itch.io listing: https://brookcoli.itch.io/meowt-of-tune - GitHub Repository: https://github.com/Aurorafume/PPR301
// ==========================================================================
//
// WHAT DOES THIS DO:
// This is a simple utility script that creates a continuously scrolling
// texture effect on a UI 'RawImage' component. It achieves this by modifying
// the image's UV coordinates each frame, which creates the illusion of a
// moving background or texture.
//
// Core functionalities include:
// - Modifying a RawImage's 'uvRect' property over time.
// - Public fields to control the horizontal and vertical scroll speed
//   independently.
//
// Dependencies:
// - Must be attached to a GameObject that has a RawImage component.
// - The 'scrollingImage' field must be assigned in the Inspector.
//
// ==========================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Continuously scrolls the texture of a RawImage component.
/// </summary>
public class Scroller : MonoBehaviour
{
    [Header("UI Component")]
    [Tooltip("The RawImage component whose texture will be scrolled.")]
    [SerializeField] private RawImage scrollingImage;

    [Header("Scroll Speed")]
    [Tooltip("The speed of the horizontal scroll.")]
    [SerializeField] private float scrollSpeedX;
    [Tooltip("The speed of the vertical scroll.")]
    [SerializeField] private float scrollSpeedY;

    /// <summary>
    /// Updates the UV coordinates of the RawImage each frame to create the scrolling effect.
    /// </summary>
    void Update()
    {
        // Calculate the new UV position by adding a small offset based on speed and time.
        Vector2 newPosition = scrollingImage.uvRect.position + new Vector2(scrollSpeedX, scrollSpeedY) * Time.deltaTime;
        
        // Apply the new position to the uvRect while keeping its size the same.
        scrollingImage.uvRect = new Rect(newPosition, scrollingImage.uvRect.size);
    }
}