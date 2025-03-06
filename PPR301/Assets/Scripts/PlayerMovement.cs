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
    private bool wasGrounded;  // Tracks previous ground state
    private float horizontalInput;
    private float verticalInput;

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

    // Crouch coroutine
    private Coroutine crouchRoutine;

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

        // Ground detection using Raycast
        grounded = Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, playerHeight * 0.5f + 0.3f, whatIsGround);

        // Debugging: Visualise ground detection ray
        Debug.DrawRay(transform.position, Vector3.down * (playerHeight * 0.5f + 0.3f), grounded ? Color.green : Color.red);

        // Play landing sound when reconnecting with ground after being airborne
        if (!wasGrounded && grounded)
        {
            noiseHandler.GenerateNoise(noiseHandler.jumpNoise);
        }

        MyInput();
        SpeedControl();
        HandleCrouch();

        rb.drag = grounded ? groundDrag : 0;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private IEnumerator DisableGroundDetectionTemporarily()
    {   
        // Disable ground detection for a short duration after jumping
        yield return new WaitForSeconds(0.1f);
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);
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
        // Move player based on input
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // Apply movement speed based on grounded state
        if (grounded)
        {
            rb.velocity = new Vector3(moveDirection.x * playerSpeed, rb.velocity.y, moveDirection.z * playerSpeed);
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
        StartCoroutine(DisableGroundDetectionTemporarily());
    }

    private void ResetJump()
    {   
        // Reset jump cooldown
        readyToJump = true;
    }

    private void HandleCrouch()
    {   
        // Handle crouch input
        if (Input.GetKeyDown(crouchKey) && !isCrouching)
        {
            isCrouching = true;
            if (crouchRoutine != null) StopCoroutine(crouchRoutine);
            crouchRoutine = StartCoroutine(Crouch());
        }
        else if (Input.GetKeyUp(crouchKey) && isCrouching)
        {
            isCrouching = false;
            if (crouchRoutine != null) StopCoroutine(crouchRoutine);
            crouchRoutine = StartCoroutine(Uncrouch());
        }
    }

    private IEnumerator Crouch()
    {   
        // Crouch coroutine
        Vector3 targetScale = new Vector3(originalScale.x, crouchHeight, originalScale.z);

        while (transform.localScale.y > crouchHeight)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, crouchSpeed * Time.deltaTime * 5f);
            yield return null;
        }

        transform.localScale = targetScale;
    }

    private IEnumerator Uncrouch()
    {   
        // Uncrouch coroutine
        Vector3 targetScale = originalScale;

        while (transform.localScale.y < originalScale.y)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, crouchSpeed * Time.deltaTime * 5f);
            yield return null;
        }

        transform.localScale = targetScale;
    }
}
