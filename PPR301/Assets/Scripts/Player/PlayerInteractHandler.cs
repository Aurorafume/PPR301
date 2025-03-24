using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractHandler : MonoBehaviour
{
    [Header("Interact Parameters")]
    [SerializeField] float grabRadius = 0.5f;
    [SerializeField] float grabDistance = 1f;
    
    [Header("Drag Parameters")]
    [SerializeField] float jointLimit = 0.1f;
    [SerializeField] float jointSpring = 100f;
    [SerializeField] float jointDamper = 5f;
    public bool isCarrying;

    [Header("References")]
    [SerializeField] KeyCode grabButton = KeyCode.Mouse0;
    [SerializeField] LayerMask interactableLayer;
    [SerializeField] Transform mouthPosition;

    Transform carryObject;
    Collider carryObjectCollider;
    Rigidbody grabbedObjectRB;
    ConfigurableJoint joint;
    Vector3 grabPoint;

    bool hasCarryObject;
    bool draggingObject;

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        // Detect input and pick up or drop depending on whether something is already held.
        if (Input.GetKeyDown(grabButton) && isCarrying == false)
        {
            TryGrabObject();
        }
        else if(Input.GetKeyDown(grabButton) && isCarrying == true)
        {
            if (hasCarryObject)
            {
                ReleaseCarryObject();
            }
            if (draggingObject)
            {
                ReleaseGrabbedObject();
            }
        }

        //if (Input.GetKeyUp(grabButton) )
        //{
        //    if (hasCarryObject)
        //    {
        //        ReleaseCarryObject();
        //    }
        //    if (draggingObject)
        //    {
        //        ReleaseGrabbedObject();
        //    }
        //}
    }

    void TryGrabObject()
    {
        // Look for a collider with interactable layer to be picked up within a small region in
        // front of the player.
        RaycastHit interactableHit;
        bool interactableDetected = Physics.SphereCast(transform.position, grabRadius, 
                                                    transform.forward, out interactableHit, 
                                                    grabDistance, interactableLayer);
        if (interactableDetected)
        {
            Carriable carriable = interactableHit.transform.GetComponent<Carriable>();
            if (carriable != null)
            {
                CarryObject(interactableHit);
                isCarrying = true;
            }
            Draggable draggable = interactableHit.transform.GetComponent<Draggable>();
            if (draggable != null)
            {
                DragObject(interactableHit);
                isCarrying = true;
            }
        }
    }

    void CarryObject(RaycastHit interactableHit)
    {
        hasCarryObject = true;

        // Get carriable object's transform and rigidbody.
        carryObject = interactableHit.transform;
        grabbedObjectRB = interactableHit.rigidbody;

        // Held objects are parented to the player, have physics disabled, and collider disabled.

        carryObject.position = mouthPosition.position;
        carryObject.parent = mouthPosition;

        grabbedObjectRB.isKinematic = true;

        carryObjectCollider = interactableHit.collider;
        carryObjectCollider.enabled = false;
    }

    void DragObject(RaycastHit interactableHit)
    {
        draggingObject = true;

        grabbedObjectRB = interactableHit.rigidbody;
        grabPoint = interactableHit.point;

        joint = grabbedObjectRB.gameObject.AddComponent<ConfigurableJoint>();
        joint = ConfigureJoint(joint);
    }

    ConfigurableJoint ConfigureJoint(ConfigurableJoint thisJoint)
    {
        thisJoint.xMotion = ConfigurableJointMotion.Limited;
        thisJoint.yMotion = ConfigurableJointMotion.Limited;
        thisJoint.zMotion = ConfigurableJointMotion.Limited;
        thisJoint.angularXMotion = ConfigurableJointMotion.Free;
        thisJoint.angularYMotion = ConfigurableJointMotion.Free;
        thisJoint.angularZMotion = ConfigurableJointMotion.Free;

        SoftJointLimit limit = new SoftJointLimit();
        limit.limit = jointLimit;
        thisJoint.linearLimit = limit;

        JointDrive drive = new JointDrive();
        drive.positionSpring = jointSpring;
        drive.positionDamper = jointDamper;
        drive.maximumForce = Mathf.Infinity;

        thisJoint.xDrive = drive;
        thisJoint.yDrive = drive;
        thisJoint.zDrive = drive;

        thisJoint.connectedBody = this.GetComponent<Rigidbody>();
        thisJoint.anchor = grabbedObjectRB.transform.InverseTransformPoint(grabPoint);
        thisJoint.breakForce = Mathf.Infinity;

        return thisJoint;
    }

    void ReleaseCarryObject()
    {
        // Drop the item by returning it to its original values/state.

        carryObject.parent = null;
        carryObject = null;
            
        grabbedObjectRB.isKinematic = false;
        carryObjectCollider.enabled = true;

        hasCarryObject = false;
        isCarrying = false;
    }

    void ReleaseGrabbedObject()
    {
        if (joint)
        {
            Destroy(joint);
        }

        if (grabbedObjectRB)
        {
            grabbedObjectRB = null;
        }

        draggingObject = false;
        isCarrying = false;
    }
}
