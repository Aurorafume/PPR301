// ==========================================================================
// Meowt of Tune - Cursor Controller
// Owned and contributed to by: brookcoli (Itch.io) - Aurorafume (GitHub), donkzilla03 (Itch.io) - AliAK03 (GitHub), JaydenFielderTorrens (Itch.io) - Squib35 (GitHub), komorebimoriko (Itch.io), TheRealCrizz (GitHub), LordVGahn (GitHub)
// itch.io listing: https://brookcoli.itch.io/meowt-of-tune - GitHub Repository: https://github.com/Aurorafume/PPR301
// ==========================================================================
//
// WHAT DOES THIS DO:
// This script manages the game's mouse cursor. It sets a custom cursor
// texture and creates a temporary visual effect (like a ripple or sparkle) on
// the UI canvas every time the player clicks the mouse. The click effect
// smoothly fades out and is then destroyed.
//
// Core functionalities include:
// - Setting a custom hardware cursor with a centred hotspot.
// - Detecting mouse clicks to trigger a visual effect.
// - Spawning a UI prefab at the exact cursor position on a canvas.
// - Using a coroutine to smoothly fade out the click effect image before
//   destroying it.
//
// Dependencies:
// - Requires a 'cursorTextureDefault' texture to be assigned in the Inspector.
// - Requires a 'cursorClickImagePrefab' (a UI Image) and a reference to the
//   main UI 'canvas' to be assigned.
//
// ==========================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the custom cursor and spawns a visual effect on mouse clicks.
/// </summary>
public class CursorController : MonoBehaviour
{
    [Header("Cursor Settings")]
    [Tooltip("The texture to use for the default mouse cursor.")]
    public Texture2D cursorTextureDefault;

    [Header("Click Effect")]
    [Tooltip("The UI Image prefab to spawn at the cursor's location on click.")]
    public GameObject cursorClickImagePrefab;
    [Tooltip("The main UI canvas where the click effect will be spawned.")]
    public Canvas canvas;
    [Tooltip("How long, in seconds, the click effect takes to fade out.")]
    public float fadeDuration = 1f;

    /// <summary>
    /// Initialises the custom mouse cursor on startup.
    /// </summary>
    void Start()
    {
        // Calculate the centre of the texture to use as the "hotspot" (the actual click point).
        Vector2 centerHotspot = new Vector2(cursorTextureDefault.width / 2, cursorTextureDefault.height / 2);
        // Set the system's cursor to our custom texture.
        Cursor.SetCursor(cursorTextureDefault, centerHotspot, CursorMode.Auto);
    }

    /// <summary>
    /// Listens for mouse clicks to spawn the visual effect.
    /// </summary>
    void Update()
    {
        // Check for a left mouse button click.
        if (Input.GetMouseButtonDown(0))
        {
            // Convert the mouse's screen position to a local position within the canvas's RectTransform.
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                Input.mousePosition,
                canvas.worldCamera,
                out Vector2 spawnPos);

            // Instantiate the click effect prefab as a child of the canvas.
            GameObject clickImage = Instantiate(cursorClickImagePrefab, canvas.transform);
            RectTransform rect = clickImage.GetComponent<RectTransform>();
            // Set the anchored position of the new UI element to the calculated spawn position.
            rect.anchoredPosition = spawnPos;

            // Get the image component and start the fade-out process.
            Image img = clickImage.GetComponent<Image>();
            StartCoroutine(FadeAndDestroy(img, fadeDuration));
        }
    }

    /// <summary>
    /// A coroutine that handles the lifecycle of the click effect, fading it out and destroying it.
    /// </summary>
    /// <param name="image">The UI Image component to fade.</param>
    /// <param name="duration">The duration of the fade in seconds.</param>
    IEnumerator FadeAndDestroy(Image image, float duration)
    {
        float elapsed = 0f;
        Color originalColour = image.color;

        // Loop over the duration of the fade.
        while (elapsed < duration)
        {
            // Calculate the new alpha value by interpolating from 1 (opaque) to 0 (transparent).
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            image.color = new Color(originalColour.r, originalColour.g, originalColour.b, alpha);
            
            elapsed += Time.deltaTime;
            yield return null; // Wait for the next frame.
        }

        // Ensure the image is fully transparent before destroying it.
        image.color = new Color(originalColour.r, originalColour.g, originalColour.b, 0f);
        Destroy(image.gameObject);
    }
}