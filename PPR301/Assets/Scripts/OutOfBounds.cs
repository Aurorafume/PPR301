using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfBounds : MonoBehaviour
{
    public Vector3 respawnLocation;

    void OnCollisionEnter(Collision collision)
{
    if (collision.gameObject.CompareTag("Out of bounds"))
    {
        Debug.Log("Respawn");
        transform.position = respawnLocation;
    }
}
}
