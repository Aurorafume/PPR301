using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RecordRotation : MonoBehaviour
{
    public float rotationSpeed = 100f; // Adjust this to change the rotation speed
    public int coinValue = 1; // The value of the coin

    void Update()
    {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0); // Rotate around the Y-axis
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
           {
               Destroy(gameObject); // Destroy the coin after collection
           }
    }
}
