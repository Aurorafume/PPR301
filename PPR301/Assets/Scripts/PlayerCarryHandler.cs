using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCarryHandler : MonoBehaviour
{
    [SerializeField] KeyCode pickUpButton = KeyCode.Mouse0;
    [SerializeField] float pickUpRadius = 0.5f;
    [SerializeField] float pickUpDistance = 1f;
    [SerializeField] int interactableLayerIndex;
    [SerializeField] Transform carryPositionLocator;

    Transform heldObject;
    Rigidbody heldObjectRB;
    Collider heldObjectCollider;

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        // Detect input and pick up or drop depending on whether something is already held.
        if (Input.GetKeyDown(pickUpButton))
        {
            if (heldObject == null)
            {
                PickUpObject();
            }
            else
            {
                DropObject();
            }
        }
    }

    void PickUpObject()
    {
        // Look for a collider with a suitable layer to be picked up within a small region in
        // front of the player.
        RaycastHit carriableHit;
        bool carriableDetected = Physics.SphereCast(transform.position, pickUpRadius, 
                                                    transform.forward, out carriableHit, 
                                                    pickUpDistance, 1 << interactableLayerIndex);
        if (carriableDetected)
        {
            // Get carriable object's transform and rigidbody.
            heldObject = carriableHit.transform;
            heldObjectRB = heldObject.GetComponent<Rigidbody>();
            if (heldObjectRB == null)
            {
                heldObject = null;
                return;
            }

            // Held objects are parented to the player, have physics disabled, and collider disabled.

            heldObject.position = carryPositionLocator.position;
            heldObject.parent = carryPositionLocator;

            heldObjectRB.isKinematic = true;

            heldObjectCollider = carriableHit.collider;
            heldObjectCollider.enabled = false;
        }
    }

    void DropObject()
    {
        // Drop the item by returning it to its original values/state.

        heldObject.parent = null;
        heldObject = null;
            
        heldObjectRB.isKinematic = false;
        heldObjectCollider.enabled = true;
    }
}
