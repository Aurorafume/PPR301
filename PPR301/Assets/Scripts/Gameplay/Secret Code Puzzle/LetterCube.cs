using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LetterCube : MonoBehaviour
{
    public string myLetter;
    bool held;

    Pedestal pedestalInRange;

    void OnValidate()
    {
        TextMeshProUGUI[] letterTexts = GetComponentsInChildren<TextMeshProUGUI>();
        foreach (TextMeshProUGUI lettertext in letterTexts)
        {
            lettertext.text = myLetter;
        }
    }

    public void OnInteraction()
    {
        if (held) // Drop
        {
            held = false;
            TryPlaceOnPedestal();
        }
        else // Pick Up
        {
            held = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        pedestalInRange = other.GetComponentInParent<Pedestal>();
    }

    void OnTriggerExit(Collider other)
    {
        Debug.Log("Exited trigger");
        Pedestal exitPedestal = other.GetComponentInParent<Pedestal>();
        if (exitPedestal && exitPedestal == pedestalInRange)
        {
            pedestalInRange = null;
        }
    }

    void TryPlaceOnPedestal()
    {
        if (pedestalInRange)
        {
            // Snap
            transform.position = pedestalInRange.snapLocator.position;
            transform.rotation = pedestalInRange.snapLocator.rotation;
            pedestalInRange.SetLetterCube(this);
        }
    }
}
