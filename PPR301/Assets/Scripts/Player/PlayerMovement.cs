using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Movement Parameters")]
    public float playerSpeed;
    public float groundDrag;
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    public float crouchHeight = 0.5f;
    public float crouchSpeed = 10f;
    public float playerHeight;

    [Header("Player State Variables")]
    public bool isCrouching = false;
    public bool readyToJump;
    public bool grounded;
    private bool wasGrounded;
    private float horizontalInput;
    private float verticalInput;
    private float lastNoiseTime;

    [Header("Key Bindings")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode crouchKey = KeyCode.LeftShift;

    [Header("References")]
    //public LayerMask whatIsGround;
    public Transform orientation;
    private Vector3 moveDirection;
    private Rigidbody rb;
    private Vector3 originalScale;
    public Camera playerCamera;
    public NoiseHandler noiseHandler;
    public Transform crouchScaleObject;
    private Animator anim;
    //Ali's code
    public bool noJumpMode; //Bridge mode cant jump.

    [Header("Idle Animation")]
    public float idleTimeThreshold = 3f; // Time before setting isIdle to true
    private float lastMoveTime;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;
        originalScale = crouchScaleObject.localScale;
        playerCamera = Camera.main;
        anim = GetComponentInChildren<Animator>();
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
        CheckForObjectContact();

        // Apply drag only when grounded
        rb.drag = grounded ? groundDrag : 0;

        RotatePlayerToCamera();
        
        HandleIdleState();
    }
    private void HandleIdleState()
    {
    // Check if player is moving (either by input or velocity)
        bool isMoving = horizontalInput != 0 || verticalInput != 0 || rb.velocity.magnitude > 0.1f;

        if (isMoving)
        {
            lastMoveTime = Time.time; // Reset idle timer
            anim.SetBool("isIdle", false);
        }
        else if (Time.time - lastMoveTime > idleTimeThreshold)
        {
            anim.SetBool("isIdle", true);
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void CheckGroundStatus()
    {
        // Use SphereCast for a broader ground detection area // Changed using tag.
        grounded = Physics.SphereCast(transform.position, 0.3f, Vector3.down, out RaycastHit hit, playerHeight * 0.5f) && hit.collider.CompareTag("Ground");

        anim.SetBool("isJumping", !grounded);

    if (!grounded && rb.velocity.y < -0.1f)
    {
        anim.SetBool("isFalling", true);
    }
    else if (grounded)
    {
        anim.SetBool("isFalling", false);
    }

        //Visualise ground detection ray
        Debug.DrawRay(transform.position, Vector3.down * (playerHeight * 0.5f), grounded ? Color.green : Color.red);
    }

    private void CheckForObjectContact()
    {
        // Define the detection radius (adjust as needed)
        float detectionRadius = 0.5f;

        // Check for objects within the radius
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius);

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Object"))
            {
                GenerateObjectNoise();
                break; 
            }
        }
    }

    private void GenerateObjectNoise()
    {   
        // Generate noise when colliding with objects 
        if (Time.time - lastNoiseTime > 0.5f)
        {   
            noiseHandler.GenerateNoise(Mathf.Abs(noiseHandler.collisionNoise));
            lastNoiseTime = Time.time;
        }
    }

    private void GenerateLandingNoise()
    {   
        // Generate noise when landing on the ground
        if (Time.time - lastNoiseTime > 0.5f)
        {
            noiseHandler.GenerateNoise(Mathf.Abs(noiseHandler.jumpNoise));
            lastNoiseTime = Time.time;
        }
    }

    private void MyInput()
    {
        // Get player input
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        if(noJumpMode == false)
        {
            // Jump input
            if (Input.GetKey(jumpKey) && readyToJump && grounded)
            {
                readyToJump = false;
                Jump();
                Invoke(nameof(ResetJump), jumpCooldown);
            }
        }
    }

    private void MovePlayer()
    {
        // Calculate movement direction based on camera orientation
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        moveDirection.Normalize();

        //Check if walking and animate
        bool isWalking = moveDirection.magnitude > 0;
        anim.SetBool("isWalking", isWalking);

        // Apply movement speed based on grounded state
        if (grounded)
        {
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
        anim.SetBool("isJumping", true);

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
        crouchScaleObject.localScale = new Vector3(originalScale.x, Mathf.MoveTowards(crouchScaleObject.localScale.y, targetHeight, crouchSpeed * Time.deltaTime), originalScale.z);

        // Handle crouch input
        if (Input.GetKeyDown(crouchKey) && grounded)
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
