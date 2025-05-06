using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [Header("In-Game Values")]
    public float timer;
    public int score;

    [Header("Game Timer Scoring")]
    public float maxTimeMinutes = 12f;
    public int bonusScorePerSecondRemaining;

    bool hasWon;

    void Start()
    {
        timer = maxTimeMinutes * 60f;
    }

    void Update()
    {
        if (hasWon) return;

        if (timer > 0f)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            timer = 0f;
        }
    }

    public void AddScore(int value)
    {
        score += value;
    }

    public void LevelWin()
    {
        hasWon = true;

        int secondsRemaining = (int)timer;
        int bonusTimeScore = secondsRemaining * bonusScorePerSecondRemaining;

        AddScore(bonusTimeScore);

        Debug.Log("CONGRATULATIONS!");
        Debug.Log("Bonus time score: " + bonusTimeScore);
        Debug.Log("Total Score: " + score);
    }
}
