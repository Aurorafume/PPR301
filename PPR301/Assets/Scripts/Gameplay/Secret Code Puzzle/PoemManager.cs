// ==========================================================================
// Meowt of Tune - Poem Manager
// Owned and contributed to by: brookcoli (Itch.io) - Aurorafume (GitHub), donkzilla03 (Itch.io) - AliAK03 (GitHub), JaydenFielderTorrens (Itch.io) - Squib35 (GitHub), komorebimoriko (Itch.io), TheRealCrizz (GitHub), LordVGahn (GitHub)
// itch.io listing: https://brookcoli.itch.io/meowt-of-tune - GitHub Repository: https://github.com/Aurorafume/PPR301
// ==========================================================================
//
// WHAT DOES THIS DO:
// This script manages the display and animation of a sequence of poems that act
// as clues for a puzzle. It controls the visual presentation of the text,
// handling complex fade-in and fade-out animations for each line to create a
// dramatic effect as the player progresses through the puzzle.
//
// Core functionalities include:
// - Displaying multi-line poems from a configurable list.
// - Animating text with staggered line-by-line fades.
// - Sequencing the transition between different poems.
// - Handling the final puzzle completion state, displaying a final message,
//   and triggering a reward.
//
// Dependencies:
// - UnityEngine and TMPro for core functionality and text display.
// - A RevealReward script to trigger when the puzzle is complete.
// - A CodeManager script to signal puzzle progression.
//
// ==========================================================================

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Manages the display and animated fading of puzzle poems.
/// </summary>
public class PoemManager : MonoBehaviour
{
    [Header("Poem Content")]
    [Tooltip("The sequence of poems to display as puzzle clues.")]
    public List<Poem> poems = new List<Poem>();
    [Tooltip("The final text to display after the puzzle is completed.")]
    public Poem finalText;

    [Header("Fading Animation Settings")]
    [Tooltip("The speed of the alpha fade for each text line.")]
    public float fadeSpeed = 1f;
    [Tooltip("The delay in seconds between each line fading IN.")]
    public float delayBetweenLineFadesIn = 2f;
    [Tooltip("The delay in seconds between each line fading OUT.")]
    public float delayBetweenLineFadesOut = 0.5f;
    [Tooltip("The delay in seconds after fading out the old poem before fading in the new one.")]
    public float delayBeforeFadeInNew = 1.5f;

    [Header("Component References")]
    [Tooltip("An array of the four TextMeshProUGUI elements used to display the poem lines.")]
    public TextMeshProUGUI[] textLines = new TextMeshProUGUI[4];
    [Tooltip("Reference to the script that reveals the puzzle's final reward.")]
    public RevealReward revealReward;

    /// <summary>
    /// Called by Unity on startup. Initialises the display with the first poem, but keeps it invisible.
    /// </summary>
    void Start()
    {
        if (poems.Count > 0)
        {
            Poem firstPoem = poems[0];
            DrawPoemText(firstPoem);
        }
        SetLinesInstantlyFaded();
    }

    /// <summary>
    /// Instantly sets the alpha of all text lines to zero.
    /// </summary>
    private void SetLinesInstantlyFaded()
    {
        foreach (TextMeshProUGUI textLine in textLines)
        {
            textLine.alpha = 0f;
        }
    }

    /// <summary>
    /// Begins the puzzle by fading in the first poem.
    /// </summary>
    public void StartPuzzle()
    {
        // Fade in the first poem.
        SetFadePoem(false);
    }

    /// <summary>
    /// Transitions to a new poem, handling the fade-out of the old and fade-in of the new.
    /// </summary>
    /// <param name="poemIndex">The index of the new poem to display from the list.</param>
    public void SetNewPoem(int poemIndex)
    {
        if (poemIndex < poems.Count)
        {
            Poem poem = poems[poemIndex];
            StartCoroutine(NewPoemCoroutineSequence(poem));
        }
    }

    /// <summary>
    /// Coroutine that manages the entire sequence of transitioning between poems.
    /// </summary>
    private IEnumerator NewPoemCoroutineSequence(Poem poem)
    {
        // 1. Fade out the current poem.
        SetFadePoem(true);

        // 2. Wait for the fade-out animation to complete.
        float fadeOutDuration = delayBetweenLineFadesOut * (textLines.Length - 1) + (1 / fadeSpeed);
        yield return new WaitForSeconds(fadeOutDuration);

        // 3. Update the text content to the new poem and wait.
        SetLinesInstantlyFaded();
        DrawPoemText(poem);
        yield return new WaitForSeconds(delayBeforeFadeInNew);

        // 4. Fade in the new poem.
        SetFadePoem(false);
    }

    /// <summary>
    /// Populates the TextMeshProUGUI elements with the lines from a given poem.
    /// </summary>
    private void DrawPoemText(Poem poem)
    {
        for (int i = 0; i < textLines.Length; i++)
        {
            // If the poem has a line for this text element, display it. Otherwise, clear it.
            if (i < poem.poemLines.Count)
            {
                textLines[i].text = poem.poemLines[i];
            }
            else
            {
                textLines[i].text = "";
            }
        }
    }

    /// <summary>
    /// Initiates the fade sequence for the entire poem.
    /// </summary>
    /// <param name="fadeOut">True to fade out, false to fade in.</param>
    private void SetFadePoem(bool fadeOut)
    {
        float targetAlpha = fadeOut ? 0f : 1f;
        StartCoroutine(FadePoemCoroutine(fadeOut, targetAlpha));
    }

    /// <summary>
    /// Coroutine that applies a staggered fade to each line of the poem.
    /// </summary>
    private IEnumerator FadePoemCoroutine(bool fadeOut, float targetAlpha)
    {
        float delay = fadeOut ? delayBetweenLineFadesOut : delayBetweenLineFadesIn;

        // Start a fade coroutine for each line with a delay between them.
        foreach (TextMeshProUGUI textLine in textLines)
        {
            StartCoroutine(FadePoemLineCoroutine(targetAlpha, textLine));
            yield return new WaitForSeconds(delay);
        }
    }

    /// <summary>
    /// Coroutine that fades a single TextMeshProUGUI line to a target alpha value.
    /// </summary>
    private IEnumerator FadePoemLineCoroutine(float targetAlpha, TextMeshProUGUI textLine)
    {
        float currentAlpha = textLine.color.a;

        // Continue until the target alpha is reached.
        while (!Mathf.Approximately(currentAlpha, targetAlpha))
        {
            currentAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, Time.deltaTime * fadeSpeed);
            textLine.alpha = currentAlpha;
            yield return null; // Wait for the next frame.
        }
    }

    /// <summary>
    /// Called when the entire puzzle is solved. Displays the final text and reveals the reward.
    /// </summary>
    public void PuzzleComplete()
    {
        StartCoroutine(NewPoemCoroutineSequence(finalText));
        StartCoroutine(RevealKeyCoroutine());
    }

    /// <summary>
    /// Coroutine that waits for the final text to appear before revealing the puzzle reward.
    /// </summary>
    private IEnumerator RevealKeyCoroutine()
    {
        // Wait for the old text to fade out.
        float fadeOutDelay = delayBetweenLineFadesOut * (textLines.Length - 1) + (1 / fadeSpeed);
        yield return new WaitForSeconds(fadeOutDelay);
        
        // Wait for the new text to start fading in.
        float fadeInDelay = delayBeforeFadeInNew + (1 / fadeSpeed); // Reveals after the first line starts fading.
        yield return new WaitForSeconds(fadeInDelay);

        // Reveal the reward.
        if (revealReward)
        {
            revealReward.Reveal();
        }

        // Keep the final words on screen for a moment.
        yield return new WaitForSeconds(5f);

        // Fade out the final text.
        SetFadePoem(true);
    }
}

/// <summary>
/// A data container for a single poem, holding a list of its lines.
/// The [System.Serializable] attribute allows this class to be edited in the Unity Inspector.
/// </summary>
[System.Serializable]
public class Poem
{
    [Tooltip("The individual lines of text that make up this poem.")]
    public List<string> poemLines = new List<string>();
}