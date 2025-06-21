using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pedestal : MonoBehaviour
{
    public Transform snapLocator;

    char givenLetter;

    CodeManager codeManager;

    LetterCube myLetterCube;

    void Awake()
    {
        codeManager = FindObjectOfType<CodeManager>();
    }

    public void SetLetterCube(LetterCube letterCube)
    {
        if (!letterCube)
        {
            myLetterCube = null;
            givenLetter = "\0"[0];
            return;
        }

        if (myLetterCube)
        {
            SwapLetterCubes();
        }

        myLetterCube = letterCube;
        // Passes first letter of the string into the char.
        givenLetter = letterCube.myLetter[0];

        if (codeManager)
        {
            codeManager.CheckCode();
        }
    }

    void SwapLetterCubes()
    {
        Carriable carriable = myLetterCube.GetComponent<Carriable>();
        if (carriable)
        {
            carriable.OnInteraction();
        }
        myLetterCube.OnInteraction();
    }

    public char GetGivenLetter()
    {
        return givenLetter;
    }

    
}
