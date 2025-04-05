using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfBounds : MonoBehaviour
{
    public Vector3 respawnLocation;
    public Vector3 respawnLocation2;
    public Vector3 currentRespawnLocation;
    public Vector3 spawnLocationsArray;

    void OnCollisionEnter(Collision collision)
{
    if (collision.gameObject.CompareTag("Out of bounds"))
    {
        Debug.Log("Respawn");
        transform.position = respawnLocation;
    }
    if (collision.gameObject.CompareTag("Light"))
    {
        Debug.Log("Respawn");
        transform.position = respawnLocation2;
    }
}
}
