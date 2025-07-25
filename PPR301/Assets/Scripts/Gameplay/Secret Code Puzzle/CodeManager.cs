// ==========================================================================
// Meowt of Tune - Code Manager
// Owned and contributed to by: brookcoli (Itch.io) - Aurorafume (GitHub), donkzilla03 (Itch.io) - AliAK03 (GitHub), JaydenFielderTorrens (Itch.io) - Squib35 (GitHub), komorebimoriko (Itch.io), TheRealCrizz (GitHub), LordVGahn (GitHub)
// itch.io listing: https://brookcoli.itch.io/meowt-of-tune - GitHub Repository: https://github.com/Aurorafume/PPR301
// ==========================================================================
//
// WHAT DOES THIS DO:
// This script serves as the central logic for a multi-stage code-breaking or
// word puzzle in a Unity game. It manages a sequence of correct codes and
// compares them against player input received from a set of "Pedestal" objects.
//
// Core functionalities include:
// - Managing a list of puzzle solutions (codes).
// - Checking player-submitted answers from pedestals against the current solution.
// - Advancing the puzzle to the next stage when a code is correctly solved.
// - Interacting with a PoemManager to update clues and handle the final puzzle
//   completion event.
//
// Dependencies:
// - UnityEngine for core component functionality.
// - Pedestal and PoemManager custom scripts for puzzle interaction and feedback.
//
// ==========================================================================

using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

/// <summary>
/// Manages a sequence of code-based puzzles, checking answers and progressing the state.
/// </summary>
public class CodeManager : MonoBehaviour
{
    [Header("Puzzle Configuration")]
    [Tooltip("The list of correct codes for the puzzle sequence.")]
    public List<string> codes = new List<string>();
    [Tooltip("An array of the Pedestal objects that the player interacts with.")]
    public Pedestal[] pedestals = new Pedestal[4];

    [Header("Dependencies")]
    [SerializeField, Tooltip("Reference to the PoemManager for updating puzzle clues.")]
    private PoemManager poemManager;

    // The index of the current code being checked from the 'codes' list.
    private int codeIndex = 0;
    // The current correct code, stored as an array of characters for easy comparison.
    private char[] answerCode;
    // A flag to prevent further checks after the entire puzzle sequence is complete.
    private bool puzzleComplete;

    /// <summary>
    /// Called by Unity on startup to initialise the puzzle with the first code.
    /// </summary>
    void Start()
    {
        SetNewCode(codeIndex);
    }

    /// <summary>
    /// Sets the current 'answerCode' based on the provided index from the list of codes.
    /// </summary>
    /// <param name="newCodeIndex">The index of the new code to set.</param>
    void SetNewCode(int newCodeIndex)
    {
        if (newCodeIndex < codes.Count)
        {
            string codeString = codes[this.codeIndex];
            answerCode = codeString.ToCharArray();
        }
    }

    /// <summary>
    /// Checks the letters on the pedestals against the current answer code.
    /// If correct, it advances the puzzle to the next stage.
    /// </summary>
    public void CheckCode()
    {
        // Don't process if the puzzle is already finished.
        if (puzzleComplete) return;
        
        // Iterate through each pedestal and compare its letter to the answer.
        for (int i = 0; i < pedestals.Length; i++)
        {
            bool correctLetter = answerCode[i] == pedestals[i].GetGivenLetter();
            
            // If any letter is incorrect, the code is wrong. Stop checking.
            if (!correctLetter)
            {
                return;
            }
        }

        // If the loop completes, the code is correct.
        codeIndex++;

        // Check if all codes in the sequence have been completed.
        if (codeIndex == codes.Count)
        {
            // All codes solved.
            HandlePuzzleComplete();
        }
        else
        {
            // Current code solved, move to the next one.
            SetNewCode(codeIndex);
            poemManager.SetNewPoem(codeIndex);
        }
    }

    /// <summary>
    /// Handles the final puzzle completion state by notifying the PoemManager.
    /// </summary>
    void HandlePuzzleComplete()
    {
        poemManager.PuzzleComplete();
        puzzleComplete = true;
    }
}