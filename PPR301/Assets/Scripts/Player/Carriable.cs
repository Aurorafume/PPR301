using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class Carriable : MonoBehaviour
{
    [Header("Object Type")]
    [SerializeField] ObjectType objectType;

    [Header("Hold Parameters")]
    [SerializeField] Vector3 holdPositionOffset; [Tooltip("Offset from the position of the player's mouth when held")]
    [SerializeField] Vector3 holdOrientation; [Tooltip("Orientation of the object when held")]

    enum ObjectType
    {
        ordinary,
        key,
    }

    bool held;

    Transform mouth;
    Collider[] colliders;

    Interactable myInteractable;
    PlayerInteractHandler playerInteractHandler;
    Rigidbody rb;

    void Awake()
    {
        myInteractable = GetComponent<Interactable>();
        playerInteractHandler = FindObjectOfType<PlayerInteractHandler>();
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        colliders = GetComponentsInChildren<Collider>();
        mouth = playerInteractHandler.mouth;
    }

    public void OnInteraction()
    {
        if (held)
        {
            Drop();
        }
        else
        {
            Carry();
        }
    }

    void Carry()
    {

        myInteractable.SetAwaitingFurtherInteraction(true);
        held = true;

        Vector3 holdPosition = mouth.position + holdPositionOffset;
        transform.position = holdPosition;
        transform.parent = mouth;
        transform.localEulerAngles = holdOrientation;

        if (objectType == ObjectType.ordinary)
        {
            SetOrdinaryChanges();
        }
        if (objectType == ObjectType.key)
        {
            SetKeyChanges();
        }

        if (rb)
        {
            rb.isKinematic = true;
        }
    }

    void SetOrdinaryChanges()
    {
        foreach(Collider collider in colliders)
        {
            collider.enabled = false;
        }
    }

    void SetKeyChanges()
    {
        foreach(Collider collider in colliders)
        {
            collider.isTrigger = true;
        }
    }

    void Drop()
    {
        myInteractable.SetAwaitingFurtherInteraction(false);
        held = false;

        transform.parent = null;

        foreach(Collider collider in colliders)
        {
            collider.enabled = true;
            collider.isTrigger = false;
        }

        if (rb)
        {
            rb.isKinematic = false;
        }
    }
}
