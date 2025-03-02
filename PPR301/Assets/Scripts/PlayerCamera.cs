using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public float sensX; // X sensitivity
    public float sensY; // Y sensitivity

    public Transform orientation; // Player's orientation

    float xRotation; // X rotation
    float yRotation; // Y rotation

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor
        Cursor.visible = false; // Hide the cursor
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensX * Time.deltaTime; // Get mouse X input
        float mouseY = Input.GetAxis("Mouse Y") * sensY * Time.deltaTime; // Get mouse Y input
        xRotation -= mouseY; // Subtract mouseY from xRotation
        yRotation += mouseX; // Add mouseX to yRotation
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Clamp xRotation between -90 and 90
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f); // Set the camera's rotation
        orientation.rotation = Quaternion.Euler(0f, yRotation, 0f); // Set the player's rotation
    }
}
