using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LetterCube : MonoBehaviour
{
    public string myLetter;
    bool held;

    Pedestal myPedestal;
    List<Pedestal> pedestalsInRange = new List<Pedestal>();

    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

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
            if (myPedestal)
            {
                myPedestal.SetLetterCube(null);
            }
            myPedestal = null;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Pedestal pedestalInRange = other.GetComponentInParent<Pedestal>();

        if (pedestalInRange)
        {
            pedestalsInRange.Add(pedestalInRange);
        }
    }

    void OnTriggerExit(Collider other)
    {
        Pedestal exitPedestal = other.GetComponentInParent<Pedestal>();

        if (!exitPedestal) return;
        if (pedestalsInRange.Count == 0) return;

        // Remove pedestal from list when exiting trigger.
        foreach (Pedestal pedestalInRange in pedestalsInRange)
        {
            if (exitPedestal == pedestalInRange)
            {
                pedestalsInRange.Remove(exitPedestal);
                return;
            }
        }
    }

    void TryPlaceOnPedestal()
    {
        if (pedestalsInRange.Count == 0) return;

        Pedestal snapPedestal = pedestalsInRange[0];

        // If box is inside more than 1 pedestal trigger, chose the closest to snap to.
        if (pedestalsInRange.Count > 1)
        {
            float closestDistance = float.MaxValue;

            foreach (Pedestal pedestalInRange in pedestalsInRange)
            {
                float distance = Vector3.Distance(transform.position, pedestalInRange.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    snapPedestal = pedestalInRange;
                }
            }
        }

        // Snap
        transform.position = snapPedestal.snapLocator.position;
        transform.rotation = snapPedestal.snapLocator.rotation;
        snapPedestal.SetLetterCube(this);
        rb.isKinematic = true;
        myPedestal = snapPedestal;
    }
}
