using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMovement : MonoBehaviour
{
    private float horizontalInput;
    private float verticalInput;
    public float speed = 5;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        movevent();
    }
    public void movevent()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (verticalInput != 0)
        {
            horizontalInput = 0;
        }
        // Create movement vector based on input
        Vector3 movement = new Vector3(horizontalInput, 0, verticalInput) * speed * Time.deltaTime;
        // Move the character
        transform.Translate(movement, Space.World);  // World space movement
        // If the player is moving, rotate to face the movement direction
        if (movement != Vector3.zero)
        {
            // Calculate the direction the character should face (normalized direction)
            Quaternion targetRotation = Quaternion.LookRotation(movement);

            // Smoothly rotate towards that direction
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }

}
