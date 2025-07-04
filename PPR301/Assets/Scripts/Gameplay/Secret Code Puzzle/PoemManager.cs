using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PoemManager : MonoBehaviour
{
    public List<Poem> poems = new List<Poem>();
    public Poem finalText;

    public RevealReward revealReward;

    [Header("Fading")]
    public float fadeSpeed = 1f;
    public float delayBetweenLineFadesIn = 2f;
    public float delayBetweenLineFadesOut = 0.5f;
    public float delayBeforeFadeInNew = 1.5f;

    [Header("References")]
    public TextMeshProUGUI[] textLines = new TextMeshProUGUI[4];

    int poemIndex = 0;

    void Start()
    {
        Poem firstPoem = poems[0];

        DrawPoemText(firstPoem);

        SetLinesInstantlyFaded();
    }

    void SetLinesInstantlyFaded()
    {
        foreach (TextMeshProUGUI textLine in textLines)
        {
            textLine.alpha = 0f;
        }
    }

    public void StartPuzzle()
    {
        // Fade in first poem.
        SetFadePoem(false);
    }

    public void SetNewPoem(int poemIndex)
    {
        Poem poem = poems[poemIndex];

        StartCoroutine(NewPoemCoroutineSequence(poem));
    }

    // Handles the whole sequence when setting a new poem.
    IEnumerator NewPoemCoroutineSequence(Poem poem)
    {
        // Fade out current poem.
        SetFadePoem(true);

        // Wait the same time it takes to fade the poem.
        float fadeDelay = delayBetweenLineFadesOut * (textLines.Length - 1) + (1 / fadeSpeed);
        yield return new WaitForSeconds(fadeDelay);

        // Write the text for the new poem and wait.
        SetLinesInstantlyFaded();
        DrawPoemText(poem);
        yield return new WaitForSeconds(delayBeforeFadeInNew);

        // Fade in the new poem.
        SetFadePoem(false);
    }

    void DrawPoemText(Poem poem)
    {
        // Write each line of the poem in each line of the text boxes.
        for (int i = 0; i < textLines.Length; i++)
        {
            if (i < poem.peomLines.Count)
            {
                textLines[i].text = poem.peomLines[i];
            }
            else
            {
                textLines[i].text = "";
            }
        }
    }

    void SetFadePoem(bool fadeOut)
    {
        // Set target alpha (fade in or out).
        float targetAlpha;
        if (fadeOut)
        {
            targetAlpha = 0f;
        }
        else
        {
            targetAlpha = 1f;
        }

        StartCoroutine(FadePoemCoroutine(fadeOut, targetAlpha));
    }

    IEnumerator FadePoemCoroutine(bool fadeOut, float targetAlpha)
    {
        // Fade in/out each line with a delay from the top line to the bottom line.
        float delay;
        if (fadeOut)
        {
            delay = delayBetweenLineFadesOut;
        }
        else
        {
            delay = delayBetweenLineFadesIn;
        }

        foreach (TextMeshProUGUI textLine in textLines)
        {
            StartCoroutine(FadePoemLineCoroutine(targetAlpha, textLine));
            yield return new WaitForSeconds(delay);
        }
    }

    IEnumerator FadePoemLineCoroutine(float targetAlpha, TextMeshProUGUI textLine)
    {
        float currentAlpha = textLine.color.a;

        while (currentAlpha != targetAlpha)
        {
            // Move alpha towards target value.
            currentAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, Time.deltaTime * fadeSpeed);

            // Set the new alpha value to the text colour.
            textLine.alpha = currentAlpha;

            yield return new WaitForEndOfFrame();
        }
    }

    public void PuzzleComplete()
    {
        StartCoroutine(NewPoemCoroutineSequence(finalText));
        StartCoroutine(RevealKeyCoroutine());
    }

    IEnumerator RevealKeyCoroutine()
    {
        // Wait for text to fade out, and new final text to fade in.
        float fadeOutDelay = delayBetweenLineFadesOut * (textLines.Length - 1) + (1 / fadeSpeed);
        yield return new WaitForSeconds(fadeOutDelay);
        float fadeInDelay = delayBeforeFadeInNew + (1 / fadeSpeed); // Note: Reveals key after first line.
        yield return new WaitForSeconds(fadeInDelay);

        // Now reveal the reward.
        if (revealReward)
        {
            revealReward.Reveal();
        }

        // Hold the final words.
        yield return new WaitForSeconds(5f);

        // Wipe the text clean.
        SetFadePoem(true);
    }
}

// Allows to have a list of lists (List of poems with separated poem lines).
[System.Serializable]
public class Poem
{
    public List<string> peomLines = new List<string>();
}