using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInteractHandler : MonoBehaviour
{
    [Header("Interact Parameters")]
    [SerializeField] float grabRadius = 0.5f;
    [SerializeField] float grabDistance = 1f;
    
    [Header("References")]
    [SerializeField] KeyCode interactButton = KeyCode.Mouse0;
    [SerializeField] LayerMask interactableLayer;
    [SerializeField] Transform interactPointLocator;
    public Transform mouth;

    RaycastHit hit;
    bool interactableDetected;

    Interactable interactableInUse;
    ShowIconScript activeClickIcon;

    [HideInInspector] public Vector3 hitPoint;

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Instrument"))
        {
            Debug.Log("HIT INSTRUMENT!!!");
        }
    }

    void Update()
    {
        HandleInput();
        DetectInteractable();
        TryShowClickIcon();
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

    void DetectInteractable()
    {
        // Look for a collider with interactable layer to be picked up within a small region in
        // front of the player.
        interactableDetected = Physics.SphereCast(interactPointLocator.position, grabRadius,
                                                    transform.forward, out hit,
                                                    grabDistance, interactableLayer);
    }

    void TryShowClickIcon()
    {
        if (interactableInUse)
        {
            if (activeClickIcon)
            {
                activeClickIcon.SetIconActive(false);
                activeClickIcon = null;
            }
            return;
        }
        
        if (interactableDetected)
        {
            ShowIconScript showIconScript = hit.transform.GetComponentInChildren<ShowIconScript>();

            // If there is no active icon, but the icon script detected, set it active
            if (showIconScript && !activeClickIcon)
            {
                showIconScript.SetIconActive(true);
                activeClickIcon = showIconScript;
            }
            // If there is an active icon, but the icon script is not detected, set it inactive.
            else if (!showIconScript && activeClickIcon)
            {
                activeClickIcon.SetIconActive(false);
                activeClickIcon = null;
            }
        }
        // If no interactable is detected but an icon was active, set it inactive.
        else if (!interactableDetected && activeClickIcon)
        {
            activeClickIcon.SetIconActive(false);
            activeClickIcon = null;
        }
    }

    void TryInteract()
    {

        if (interactableDetected)
        {
            Interactable interactable = hit.transform.GetComponent<Interactable>();
            if (interactable)
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
