using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    PlayerInteractHandler playerInteractHandler;

    public UnityEvent OnInteraction;

    void Awake()
    {
        playerInteractHandler = FindObjectOfType<PlayerInteractHandler>();
    }

    public void Interact()
    {
        OnInteraction.Invoke();
    }

    public void SetAwaitingFurtherInteraction(bool waiting)
    {
        if (waiting)
        {
            playerInteractHandler.SetInteractableInUse(this);
        }
        else
        {
            playerInteractHandler.SetInteractableInUse(null);
        }
    }
}
