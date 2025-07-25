// ==========================================================================
// Meowt of Tune - Pedestal
// Owned and contributed to by: brookcoli (Itch.io) - Aurorafume (GitHub), donkzilla03 (Itch.io) - AliAK03 (GitHub), JaydenFielderTorrens (Itch.io) - Squib35 (GitHub), komorebimoriko (Itch.io), TheRealCrizz (GitHub), LordVGahn (GitHub)
// itch.io listing: https://brookcoli.itch.io/meowt-of-tune - GitHub Repository: https://github.com/Aurorafume/PPR301
// ==========================================================================
//
// WHAT DOES THIS DO:
// This script represents a single pedestal or socket designed to hold a
// "LetterCube" as part of a larger word puzzle. It acts as a receiver for the
// letter cubes and communicates with the main puzzle manager.
//
// Core functionalities include:
// - Providing a precise "snap" location for a LetterCube to attach to.
// - Storing the letter of the cube currently placed on it.
// - Notifying the central CodeManager to check the puzzle's solution whenever a
//   new cube is placed or a cube is removed.
//
// Dependencies:
// - UnityEngine for core component functionality.
// - LetterCube and CodeManager custom scripts for puzzle logic.
//
// ==========================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a socket for a LetterCube, communicating with the CodeManager to solve a puzzle.
/// </summary>
public class Pedestal : MonoBehaviour
{
    [Header("Pedestal Configuration")]
    [Tooltip("The Transform defining the exact position and rotation for a placed cube.")]
    public Transform snapLocator;

    // The letter provided by the cube currently on this pedestal.
    private char givenLetter;
    // A cached reference to the main puzzle logic controller.
    private CodeManager codeManager;
    // A reference to the LetterCube instance currently on this pedestal.
    private LetterCube myLetterCube;

    /// <summary>
    /// Called by Unity on startup to cache a reference to the CodeManager.
    /// </summary>
    void Awake()
    {
        codeManager = FindObjectOfType<CodeManager>();
    }

    /// <summary>
    /// Assigns a LetterCube to this pedestal or clears it if the provided cube is null.
    /// After assignment, it triggers a check of the puzzle's code.
    /// </summary>
    /// <param name="letterCube">The LetterCube instance to place on this pedestal.</param>
    public void SetLetterCube(LetterCube letterCube)
    {
        // If a null cube is passed, it means a cube is being removed.
        if (letterCube == null)
        {
            myLetterCube = null;
            givenLetter = '\0'; // Set to null character.
            return;
        }

        // If a cube is already on this pedestal, handle the swap.
        if (myLetterCube != null)
        {
            SwapLetterCubes();
        }

        // Assign the new cube and store its letter.
        myLetterCube = letterCube;
        givenLetter = letterCube.myLetter[0]; // Gets the first character of the cube's string.

        // Notify the CodeManager to check the new arrangement.
        if (codeManager != null)
        {
            codeManager.CheckCode();
        }
    }

    /// <summary>
    /// Handles the logic to release the currently held cube when a new one is placed.
    /// </summary>
    private void SwapLetterCubes()
    {
        // This simulates an interaction to "pick up" the old cube, freeing the pedestal.
        // Assumes the old cube has a Carriable component for player interaction.
        Carriable carriable = myLetterCube.GetComponent<Carriable>();
        if (carriable)
        {
            carriable.OnInteraction();
        }
        myLetterCube.OnInteraction();
    }

    /// <summary>
    /// Public getter that allows the CodeManager to retrieve the letter on this pedestal.
    /// </summary>
    /// <returns>The character of the cube on this pedestal, or a null character if empty.</returns>
    public char GetGivenLetter()
    {
        return givenLetter;
    }
}