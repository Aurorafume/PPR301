// ==========================================================================
// Meowt of Tune - Score Manager
// Owned and contributed to by: brookcoli (Itch.io) - Aurorafume (GitHub), donkzilla03 (Itch.io) - AliAK03 (GitHub), JaydenFielderTorrens (Itch.io) - Squib35 (GitHub), komorebimoriko (Itch.io), TheRealCrizz (GitHub), LordVGahn (GitHub)
// itch.io listing: https://brookcoli.itch.io/meowt-of-tune - GitHub Repository: https://github.com/Aurorafume/PPR301
// ==========================================================================
//
// WHAT DOES THIS DO:
// This script is a central manager for the player's score and the level's
// countdown timer. It tracks the score from various in-game actions and
// calculates a time-based bonus score when the player successfully completes
// the level.
//
// Core functionalities include:
// - A countdown timer that initialises from a set number of minutes.
// - A public method for other scripts to add points to the total score.
// - A method to call upon level completion which stops the timer and awards
//   a bonus score based on the time remaining.
//
// Dependencies:
// - Relies on other game systems to call the 'AddScore' method for scoring.
// - Requires a game manager or level completion trigger to call the 'LevelWin'
//   method to finalise the score.
//
// ==========================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the game timer and player score, including a time-based bonus upon winning.
/// </summary>
public class ScoreManager : MonoBehaviour
{
    [Header("In-Game Values")]
    [Tooltip("The countdown timer, in seconds.")]
    public float timer;
    [Tooltip("The player's current score.")]
    public int score;

    [Header("Game Timer Scoring")]
    [Tooltip("The maximum time for the level, in minutes.")]
    public float maxTimeMinutes = 12f;
    [Tooltip("The number of bonus points awarded for each second remaining on the clock.")]
    public int bonusScorePerSecondRemaining;

    // A flag to stop the timer once the level has been won.
    private bool hasWon;

    /// <summary>
    /// Initialises the countdown timer at the start of the level.
    /// </summary>
    void Start()
    {
        // Convert the max time from minutes to seconds.
        timer = maxTimeMinutes * 60f;
    }

    /// <summary>
    /// Handles the countdown logic each frame.
    /// </summary>
    void Update()
    {
        // Do not count down if the level has already been won.
        if (hasWon) return;

        // If time is still remaining, decrease the timer.
        if (timer > 0f)
        {
            timer -= Time.deltaTime;
        }
        // Otherwise, clamp the timer at zero.
        else
        {
            timer = 0f;
        }
    }

    /// <summary>
    /// Adds a specified value to the total score.
    /// </summary>
    /// <param name="value">The number of points to add.</param>
    public void AddScore(int value)
    {
        score += value;
    }

    /// <summary>
    /// Called when the player wins the level to calculate and add the final time bonus.
    /// </summary>
    public void LevelWin()
    {
        hasWon = true;

        // Calculate the bonus score from the remaining time.
        int secondsRemaining = (int)timer;
        int bonusTimeScore = secondsRemaining * bonusScorePerSecondRemaining;

        // Add the bonus to the final score.
        AddScore(bonusTimeScore);
    }
}