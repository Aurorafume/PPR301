using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


using UnityEngine;

public class Breakable : MonoBehaviour
{
    [Header("Break Settings")]
    public float breakForceThreshold = 10f;
    public GameObject objectToReveal; // The key or other object to move into place
    public Transform revealLocation;  // Where to move the object when revealed

    private Rigidbody rb;
    private bool hasBroken = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (hasBroken) return;

        float impactForce = collision.relativeVelocity.magnitude;

        if (impactForce >= breakForceThreshold)
        {
            BreakObject();
        }
    }

    void BreakObject()
    {
        hasBroken = true;

        if (objectToReveal != null && revealLocation != null)
        {
            Instantiate(objectToReveal, revealLocation.position, Quaternion.identity);
        }

        Destroy(gameObject); // destroy the jar or whatever object this script is on
    }
}