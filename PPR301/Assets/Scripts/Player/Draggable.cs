using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Draggable : MonoBehaviour
{
    [Header("Drag Parameters")]
    [SerializeField] float jointLimit = 0.1f;
    [SerializeField] float jointSpring = 100f;
    [SerializeField] float jointDamper = 5f;
    
    public bool grabbed;
    Vector3 grabPoint;
    ConfigurableJoint joint;

    Interactable myInteractable;
    PlayerInteractHandler playerInteractHandler;

    Rigidbody rb;

    bool defaultFreezeRotation;

    public static event Action<bool> OnDragObject;

    void Awake()
    {
        myInteractable = GetComponent<Interactable>();
        playerInteractHandler = FindObjectOfType<PlayerInteractHandler>();
        rb = GetComponent<Rigidbody>();
        defaultFreezeRotation = rb.freezeRotation;
    }

    public void OnInteraction()
    {
        if (grabbed)
        {
            Drop();
        }
        else
        {
            Drag();
        }
    }

    void Drag()
    {
        myInteractable.SetAwaitingFurtherInteraction(true);
        grabbed = true;
        OnDragObject?.Invoke(true);
        rb.freezeRotation = true;

        grabPoint = playerInteractHandler.hitPoint;

        if (!joint)
        {
            joint = gameObject.AddComponent<ConfigurableJoint>();
            ConfigureJoint();
        }
    }

    void ConfigureJoint()
    {
        joint.xMotion = ConfigurableJointMotion.Limited;
        joint.yMotion = ConfigurableJointMotion.Limited;
        joint.zMotion = ConfigurableJointMotion.Limited;
        joint.angularXMotion = ConfigurableJointMotion.Free;
        joint.angularYMotion = ConfigurableJointMotion.Free;
        joint.angularZMotion = ConfigurableJointMotion.Free;

        SoftJointLimit limit = new SoftJointLimit();
        limit.limit = jointLimit;
        joint.linearLimit = limit;

        JointDrive drive = new JointDrive();
        drive.positionSpring = jointSpring;
        drive.positionDamper = jointDamper;
        drive.maximumForce = Mathf.Infinity;

        joint.xDrive = drive;
        joint.yDrive = drive;
        joint.zDrive = drive;

        joint.connectedBody = playerInteractHandler.gameObject.GetComponent<Rigidbody>();
        joint.anchor = transform.InverseTransformPoint(grabPoint);
        joint.breakForce = Mathf.Infinity;
    }

    void Drop()
    {
        myInteractable.SetAwaitingFurtherInteraction(false);
        grabbed = false;
        OnDragObject?.Invoke(false);
        rb.freezeRotation = defaultFreezeRotation;

        if (joint)
        {
            Destroy(joint);
        }
    }
}
