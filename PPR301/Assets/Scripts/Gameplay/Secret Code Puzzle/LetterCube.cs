using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterCube : MonoBehaviour
{
    public bool held;

    public void OnInteraction()
    {
        if (held) // Drop
        {
            held = false;
        }
        else // Pick Up
        {
            held = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Pedestal pedestal = other.GetComponentInParent<Pedestal>();

        if (pedestal)
        {
            // Snap
            transform.position = pedestal.snapLocator.position;
        }
    }
}
