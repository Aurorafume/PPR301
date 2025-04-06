using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractHandler : MonoBehaviour
{
    [Header("Interact Parameters")]
    [SerializeField] float grabRadius = 0.5f;
    [SerializeField] float grabDistance = 1f;
    
    [Header("References")]
    [SerializeField] KeyCode interactButton = KeyCode.Mouse0;
    [SerializeField] LayerMask interactableLayer;
    public Transform mouth;

    Interactable interactableInUse;

    [HideInInspector] public Vector3 hitPoint;

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        // Detect input and pick up or drop depending on whether something is already held.
        if (Input.GetKeyDown(interactButton))
        {
            if (interactableInUse)
            {
                interactableInUse.Interact();
            }
            else
            {
                TryInteract();
            }
            
        }
    }

    void TryInteract()
    {
        // Look for a collider with interactable layer to be picked up within a small region in
        // front of the player.
        RaycastHit hit;
        bool interactableDetected = Physics.SphereCast(transform.position, grabRadius, 
                                                    transform.forward, out hit, 
                                                    grabDistance, interactableLayer);
        if (interactableDetected)
        {
            Interactable interactable = hit.transform.GetComponent<Interactable>();
            if (interactable != null)
            {
                hitPoint = hit.point;
                interactable.Interact();
            }
        }
    }

    public void SetInteractableInUse(Interactable myInteractable)
    {
        interactableInUse = myInteractable;
    }
}
