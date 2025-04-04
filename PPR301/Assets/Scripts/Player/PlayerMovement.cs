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
    [SerializeField] private float rotationSpeed = 5f;

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
    public Transform orientation;
    public Camera playerCamera;
    public Transform crouchScaleObject;
    public NoiseHandler noiseHandler;
    public Animator anim;

    private Vector3 moveDirection;
    private Rigidbody rb;
    private Vector3 originalScale;

    [Header("Custom Flags")]
    public bool noJumpMode; // Bridge mode: can't jump.

    [Header("Idle Animation")]
    public float idleTimeThreshold = 3f;
    private float lastMoveTime;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        readyToJump = true;
        originalScale = crouchScaleObject.localScale;
        //playerCamera = Camera.main;
        anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        wasGrounded = grounded;

        CheckGroundStatus();

        if (!wasGrounded && grounded)
            GenerateLandingNoise();

        GetPlayerInput();
        HandleCrouch();
        HandleIdleState();
        CheckForObjectContact();
        MovePlayer();
        RotatePlayer();
        SpeedControl();

        rb.drag = grounded ? groundDrag : 0;
    }

    private void FixedUpdate()
    {
        /*MovePlayer();
        RotatePlayer();
        SpeedControl();*/
    }

    private void GetPlayerInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (!noJumpMode && Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if (Input.GetKeyDown(crouchKey) && grounded)
        {
            isCrouching = true;
        }
        else if (Input.GetKeyUp(crouchKey))
        {
            isCrouching = false;
        }
    }

    private void MovePlayer()
    {
        Vector3 camForward = playerCamera.transform.forward;
        Vector3 camRight = playerCamera.transform.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        moveDirection = (camForward * verticalInput + camRight * horizontalInput).normalized;

        anim.SetBool("isWalking", moveDirection.magnitude > 0);

        if (grounded)
        {
            Vector3 targetPosition = rb.position + moveDirection * playerSpeed * Time.fixedDeltaTime;
            rb.MovePosition(targetPosition);
        }
        else
        {
            float adjustedAirMultiplier = Mathf.Lerp(5f, airMultiplier, rb.velocity.magnitude / playerSpeed);
            Vector3 airMove = moveDirection * playerSpeed * airMultiplier;
            Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            Vector3 velocityChange = airMove - horizontalVelocity;
            velocityChange = Vector3.ClampMagnitude(velocityChange, playerSpeed); // prevent too much

            rb.AddForce(velocityChange, ForceMode.Acceleration);
        }
    }

    private void RotatePlayer()
    {
        if (moveDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            Quaternion smoothedRotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
            rb.MoveRotation(smoothedRotation);
        }
    }

    private void SpeedControl()
    {
        /*Vector3 flatVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVelocity.magnitude > playerSpeed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * playerSpeed;
            rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
        }*/
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        Vector3 jumpBoost = moveDirection * playerSpeed * 1f; // tweak multiplier
        rb.AddForce(jumpBoost, ForceMode.Impulse);

        anim.SetBool("isJumping", true);

        Invoke(nameof(EnableGroundDetection), 0.1f);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    private void EnableGroundDetection()
    {
        CheckGroundStatus();
    }

    private void CheckGroundStatus()
    {
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

        Debug.DrawRay(transform.position, Vector3.down * (playerHeight * 0.5f), grounded ? Color.green : Color.red);
    }

    private void HandleCrouch()
    {
        float targetHeight = isCrouching ? crouchHeight : originalScale.y;
        crouchScaleObject.localScale = new Vector3(
            originalScale.x,
            Mathf.MoveTowards(crouchScaleObject.localScale.y, targetHeight, crouchSpeed * Time.deltaTime),
            originalScale.z
        );
    }

    private void CheckForObjectContact()
    {
        float detectionRadius = 0.5f;
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
        if (Time.time - lastNoiseTime > 0.5f)
        {
            noiseHandler.GenerateNoise(Mathf.Abs(noiseHandler.collisionNoise));
            lastNoiseTime = Time.time;
        }
    }

    private void GenerateLandingNoise()
    {
        if (Time.time - lastNoiseTime > 0.5f)
        {
            noiseHandler.GenerateNoise(Mathf.Abs(noiseHandler.jumpNoise));
            lastNoiseTime = Time.time;
        }
    }

    private void HandleIdleState()
    {
        bool isMoving = horizontalInput != 0 || verticalInput != 0 || rb.velocity.magnitude > 0.1f;

        if (isMoving)
        {
            lastMoveTime = Time.time;
            anim.SetBool("isIdle", false);
        }
        else if (Time.time - lastMoveTime > idleTimeThreshold)
        {
            anim.SetBool("isIdle", true);
        }
    }
}
