using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pedestal : MonoBehaviour
{
    public Transform snapLocator;

    char givenLetter;

    CodeManager codeManager;

    void Awake()
    {
        codeManager = FindObjectOfType<CodeManager>();
    }

    public void SetLetterCube(LetterCube letterCube)
    {
        // Passes first letter of the string into the char.
        givenLetter = letterCube.myLetter[0];

        if (codeManager)
        {
            codeManager.CheckCode();
        }
    }

    public char GetGivenLetter()
    {
        return givenLetter;
    }
}
