using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfBounds : MonoBehaviour
{
    public Vector3 respawnLocation;
    public Vector3 respawnLocation2;
    //public Vector3 originalLocation;

    public Vector3[] spawnLocationsArray;
    public Vector3 currentRespawnLocation;

    void Start()
    {
        //originalLocation = transform.position;
        //currentRespawnLocation = originalLocation;
        currentRespawnLocation =  transform.position;
        //assign array of spawning locations
        spawnLocationsArray = new Vector3[2];
        spawnLocationsArray[0] = respawnLocation;
        spawnLocationsArray[1] = respawnLocation2;
    }
    void Update()
    {
        fallOffMap();
    }
    void fallOffMap()
    {
        if(transform.position.y < -8)
        {
            transform.position = currentRespawnLocation;
        }
    }

    void OnCollisionEnter(Collision collision)
{
    if (collision.gameObject.CompareTag("Out of bounds"))
    {
        Debug.Log("Respawn");
        transform.position = currentRespawnLocation;
    }
    if (collision.gameObject.CompareTag("Light"))
    {
        Debug.Log("Respawn");
        transform.position = currentRespawnLocation;
    }
}
}
