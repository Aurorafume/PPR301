using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Player movement parameters
    public float playerSpeed;
    public float groundDrag;
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    public float crouchHeight = 0.5f;
    public float crouchSpeed = 10f;
    public float playerHeight;

    // Player state variables
    public bool isCrouching = false;
    public bool readyToJump;
    public bool grounded;
    private bool wasGrounded;
    private float horizontalInput;
    private float verticalInput;
    private float lastNoiseTime;

    // Key bindings
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode crouchKey = KeyCode.LeftShift;

    // References
    public LayerMask whatIsGround;
    public Transform orientation;
    private Vector3 moveDirection;
    private Rigidbody rb;
    private Vector3 originalScale;
    public Camera playerCamera;
    public NoiseHandler noiseHandler;
    public MicrophoneInput microphoneInput;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;
        originalScale = transform.localScale;
        playerCamera = Camera.main;
    }

    private void Update()
    {
        // Store previous grounded state before updating
        wasGrounded = grounded;

        // Optimised ground detection using SphereCast for more stability
        CheckGroundStatus();

        // Play landing sound when reconnecting with ground after being airborne
        if (!wasGrounded && grounded)
        {
            GenerateLandingNoise();
        }

        MyInput();
        SpeedControl();
        HandleCrouch();

        // Apply drag only when grounded
        rb.drag = grounded ? groundDrag : 0;

        RotatePlayerToCamera();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void CheckGroundStatus()
    {
        // Use SphereCast for a broader ground detection area
        grounded = Physics.SphereCast(transform.position, 0.3f, Vector3.down, out RaycastHit hit, playerHeight * 0.5f, whatIsGround);

        //Visualise ground detection ray
        Debug.DrawRay(transform.position, Vector3.down * (playerHeight * 0.5f), grounded ? Color.green : Color.red);
    }

    private void GenerateLandingNoise()
    {
        // Prevent excessive noise generation by using a cooldown
        if (Time.time - lastNoiseTime > 0.5f)
        {
            noiseHandler.GenerateNoise(noiseHandler.jumpNoise);
            lastNoiseTime = Time.time;
        }
    }

    private void MyInput()
    {
        // Get player input
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // Jump input
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer()
    {
        // Correct movement direction (forward/back and left/right)
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        moveDirection.Normalize(); // Prevent faster diagonal movement

        // Apply movement speed based on grounded state
        if (grounded)
        {
            // More efficient movement when grounded
            Vector3 targetPosition = rb.position + moveDirection * playerSpeed * Time.fixedDeltaTime;
            rb.MovePosition(targetPosition);
        }
        else
        {
            float adjustedAirMultiplier = Mathf.Lerp(0.5f, airMultiplier, rb.velocity.magnitude / playerSpeed);
            rb.velocity = new Vector3(moveDirection.x * playerSpeed * adjustedAirMultiplier, rb.velocity.y, moveDirection.z * playerSpeed * adjustedAirMultiplier);
        }
    }

    private void SpeedControl()
    {
        // Limit player speed to playerSpeed
        Vector3 flatVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // Limit player speed if it exceeds playerSpeed
        if (flatVelocity.magnitude > playerSpeed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * playerSpeed;
            rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
        }
    }

    private void Jump()
    {
        // Apply jump force
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        // Delay ground detection after jumping to prevent instant re-grounding
        Invoke(nameof(EnableGroundDetection), 0.1f);
    }

    private void EnableGroundDetection()
    {
        // Recheck ground status after a short delay
        CheckGroundStatus();
    }

    private void ResetJump()
    {
        // Reset jump cooldown
        readyToJump = true;
    }

    private void HandleCrouch()
    {
        // Smoothly adjust player height when crouching
        float targetHeight = isCrouching ? crouchHeight : originalScale.y;
        transform.localScale = new Vector3(originalScale.x, Mathf.MoveTowards(transform.localScale.y, targetHeight, crouchSpeed * Time.deltaTime), originalScale.z);

        // Handle crouch input
        if (Input.GetKeyDown(crouchKey))
        {
            isCrouching = true;
        }
        else if (Input.GetKeyUp(crouchKey))
        {
            isCrouching = false;
        }
    }

    private void RotatePlayerToCamera()
    {
        // Rotate player to face camera direction
        if (playerCamera != null)
        {
            Vector3 cameraForward = playerCamera.transform.forward;
            cameraForward.y = 0;

            // Only rotate if there’s a significant change in direction
            if (Vector3.Angle(transform.forward, cameraForward) > 1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(cameraForward);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
            }
        }
    }
}
