using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movement_3d : MonoBehaviour
{
    public float moveSpeed = 5f;  // Movement speed
    public float jumpForce = 5f;  // Movement speed
    public bool isGrounded = true;  // Movement speed

    private float jumpTimeCounter;
    public  float jumpTime;
    private bool isJumping;
    private Rigidbody rb;

    public void Start()
    {
        // Get the Rigidbody component to apply physics
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        handleMovement();
        HandleJump();
    }
    void handleMovement()
    {
        // Get input for horizontal and vertical movement (X and Z)
        float horizontal = Input.GetAxis("Horizontal"); // A/D or Left/Right Arrow keys (X-axis)
        float vertical = Input.GetAxis("Vertical");     // W/S or Up/Down Arrow keys (Z-axis)

        // Create a movement vector based on input
        Vector3 moveDirection = new Vector3(horizontal, 0, vertical); // We keep Y as 0 to prevent vertical movement

        // Apply movement by modifying position directly (based on moveSpeed)
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
    }
    void HandleJump()
    {
        if(isGrounded == true && Input.GetKeyDown(KeyCode.Space))
        {
            isJumping = true;
            jumpTimeCounter = jumpTime;
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
            //rb.AddForce(Vector3.up * jumpForce, ForceMode.Force);
        }
        if(Input.GetKey(KeyCode.Space) && isJumping == true)
        {
            if(jumpTimeCounter > 0)
            {
                rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }
        if(Input.GetKeyUp(KeyCode.Space))
        {
            isJumping = false;
        }
    }
    // Check if the player is grounded using a simple raycast
    private void OnCollisionStay(Collision collision)
    {
        // If the player collides with the ground, they are grounded
        isGrounded = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        // If the player leaves the ground, they are no longer grounded
        isGrounded = false;
    }
}
