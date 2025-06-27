using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class CodeManager : MonoBehaviour
{
    public List<string> codes = new List<string>();
    public Pedestal[] pedestals = new Pedestal[4];

    int codeIndex = 0;

    char[] answerCode;

    [SerializeField] PoemManager poemManager;
    void Start()
    {
        SetNewCode(codeIndex);
    }

    void SetNewCode(int codeIndex)
    {
        string codeString = codes[this.codeIndex];

        answerCode = codeString.ToCharArray();
    }

    public void CheckCode()
    {
        for (int i = 0; i < pedestals.Length; i++)
        {
            bool correctLetter = answerCode[i] == pedestals[i].GetGivenLetter();
            if (!correctLetter)
            {
                // Incorrect code
                return;
            }
        }

        // If it made it through the loop, code is correct.
        codeIndex++;

        if (codeIndex == codes.Count)
        {
            // Completed all codes.
            HandlePuzzleComplete();
        }
        else
        {
            // Completed code, move on to the next one.
            SetNewCode(codeIndex);
            poemManager.SetNewPoem(codeIndex);
        }
    }

    void HandlePuzzleComplete()
    {
        poemManager.PuzzleComplete();
    }
}
