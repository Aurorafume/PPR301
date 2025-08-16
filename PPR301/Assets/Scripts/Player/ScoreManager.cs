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

using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Manages the game timer and player score, including a time-based bonus upon winning.
/// </summary>
public class ScoreManager : MonoBehaviour
{
    [Header("Time Settings")]
    public float perfectTimeThreshold = 300f; // 5 minutes
    [Header("References")]
    public GameObject victoryMenu;
    public GameObject noiseBar;

    [Header("UI References")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI deathCountText;
    public TextMeshProUGUI recordCountText;
    public Color perfectTextColour;

    float gameStartTime;
    float elapsedGameTime;
    int collectedRecords;
    int deaths;

    int totalNumRecords;


    void Start()
    {
        gameStartTime = Time.time;
        RecordRotation[] records = FindObjectsOfType<RecordRotation>();
        totalNumRecords = records.Length;
    }

    public void CollectGoldenRecord()
    {
        collectedRecords++;
    }

    public void AddDeathCount()
    {
        deaths++;
    }

    public void HandleGameComplete()
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        elapsedGameTime = Time.time - gameStartTime;

        DrawVictoryMenuInfo();

        if (victoryMenu != null)
        {
            victoryMenu.SetActive(true);
        }

        if (noiseBar != null)
        {
            noiseBar.SetActive(false);
        }        
    }

    void DrawVictoryMenuInfo()
    {
        if (timerText != null)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(elapsedGameTime);
            timerText.text = string.Format("{0:D2} : {1:D2} : {2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
            if (elapsedGameTime < perfectTimeThreshold)
            {
                timerText.color = perfectTextColour;
                timerText.fontStyle = FontStyles.Bold;
            }
        }

        if (deathCountText != null)
        {
            deathCountText.text = deaths.ToString();
            if (deaths == 0)
            {
                deathCountText.color = perfectTextColour;
                deathCountText.fontStyle = FontStyles.Bold;
            }
        }

        if (recordCountText != null)
        {
            recordCountText.text = collectedRecords.ToString();
            if (collectedRecords == totalNumRecords)
            {
                recordCountText.color = perfectTextColour;
                recordCountText.fontStyle = FontStyles.Bold;
            }
        }
    }
}