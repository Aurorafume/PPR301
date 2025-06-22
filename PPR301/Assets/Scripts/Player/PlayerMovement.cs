using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("How fast the player moves while grounded.")]
    public float playerSpeed;

    [Tooltip("Amount of drag applied to the player when grounded.")]
    public float groundDrag;

    [Tooltip("Upward force applied when the player jumps.")]
    public float jumpForce;

    [Tooltip("Cooldown time between jumps (in seconds).")]
    public float jumpCooldown;

    [Tooltip("Multiplier applied to movement while in the air.")]
    public float airMultiplier;

    [Tooltip("Air control speed when the player is airborne.")]
    public float airControlSpeed = 5f;

    [Header("Crouch Settings")]
    [Tooltip("Height scale applied when the player is crouching.")]
    public float crouchHeight = 0.5f;

    [Tooltip("Speed at which the player transitions between crouching and standing.")]
    public float crouchSpeed = 10f;

    [Tooltip("Height of the player used for grounding and raycasts.")]
    public float playerHeight;

    [Header("Rotation Settings")]
    [Tooltip("Speed of player rotation in 3D mode.")]
    [SerializeField] private float rotationSpeed = 5f;

    [Tooltip("Speed of player rotation in top-down mode.")]
    [SerializeField] private float topDownRotationSpeed = 50f;

    [Header("State Flags (Runtime Only)")]
    [Tooltip("Is the player currently crouching?")]
    public bool isCrouching = false;

    [Tooltip("Is the player ready to jump (not on cooldown)?")]
    public bool readyToJump;

    [Tooltip("Is the player currently grounded?")]
    public bool grounded;

    private bool wasGrounded;
    private float horizontalInput;
    private float verticalInput;
    private float lastNoiseTime;

    [Header("Key Bindings")]
    [Tooltip("Key used to jump.")]
    public KeyCode jumpKey = KeyCode.Space;

    [Tooltip("Key used to crouch.")]
    public KeyCode crouchKey = KeyCode.LeftShift;

    [Header("Component References")]
    [Tooltip("Transform used to determine movement direction.")]
    public Transform orientation;

    [Tooltip("Reference to the player's camera.")]
    public Camera playerCamera;

    [Tooltip("Transform that scales vertically for crouching.")]
    public Transform crouchScaleObject;

    [Tooltip("Reference to the NoiseHandler script.")]
    public NoiseHandler noiseHandler;

    [Tooltip("Animator component controlling the player's animations.")]
    public Animator anim;

    public Transform secondGroundCastLocator;

    private Vector3 moveDirection;
    private Rigidbody rb;
    private Vector3 originalScale;

    [Header("Advanced State Settings")]
    [Tooltip("Disables the ability to jump when true.")]
    public bool noJumpMode;

    [Tooltip("Time in seconds before the player is considered idle.")]
    public float idleTimeThreshold = 3f;

    private float lastMoveTime;

    private delegate void MoveBehaviour();
    private MoveBehaviour moveBehaviour;

    [Tooltip("Is the player currently dragging an object?")]
    public bool draggingObject;

    [Tooltip("Is the player currently in top-down mode?")]
    public bool inTopDownMode;

    [Tooltip("Directional offset (in degrees) from world forward in top-down mode.")]
    float forwardAngularOffsetFromWorldZ = 0;

    public bool groundedAlwaysTrue;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        readyToJump = true;
        originalScale = crouchScaleObject.localScale;
        //playerCamera = Camera.main;
        anim = GetComponentInChildren<Animator>();

        moveBehaviour = DefaultMovement;
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
        SpeedControl();

        rb.drag = grounded ? groundDrag : 0;
    }

    private void FixedUpdate()
    {
        moveBehaviour();
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

    void SetDraggingObject(bool state)
    {
        draggingObject = state;

        UpdateMoveBehaviour();
    }

    void SetInTopDownMode(bool state, float forwardAngularDirection)
    {
        inTopDownMode = state;

        forwardAngularOffsetFromWorldZ = forwardAngularDirection;

        UpdateMoveBehaviour();
    }
    IEnumerator MyCoroutine()
    {
        // suspend execution for 5 seconds
        yield return new WaitForSeconds(0.5f);
        moveBehaviour = TopDownMovement;
        Debug.Log("Changing walk mode");
    }
    IEnumerator MyCoroutine2()
    {
        // suspend execution for 5 seconds
        yield return new WaitForSeconds(0.5f);
        moveBehaviour = DefaultMovement;
        Debug.Log("Changing walk mode");
    }

    public void UpdateMoveBehaviour()
    {
        if (inTopDownMode)
        {
            //moveBehaviour = TopDownMovement;
            StartCoroutine(MyCoroutine());
        }
        else
        {
            //moveBehaviour = DefaultMovement;
            StartCoroutine(MyCoroutine2());
        }
    }

    private void DefaultMovement()
    {
        Vector3 camForward = playerCamera.transform.forward;
        Vector3 camRight = playerCamera.transform.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        moveDirection = (camForward * verticalInput + camRight * horizontalInput).normalized;

        anim.SetBool("isWalking", moveDirection.magnitude > 0);

        if (groundedAlwaysTrue)
        {
            grounded = true;
        }

        if (grounded)
            {
                Vector3 targetPosition = rb.position + moveDirection * playerSpeed * Time.fixedDeltaTime;
                rb.MovePosition(targetPosition);
            }
            else
            {
                Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
                Vector3 airMove = moveDirection * airControlSpeed;
                Vector3 velocityChange = airMove - horizontalVelocity;
                velocityChange = Vector3.ClampMagnitude(velocityChange, airControlSpeed);
                rb.AddForce(velocityChange, ForceMode.Acceleration);
                //rb.velocity = new Vector3 (velocityChange.x, rb.velocity.y, velocityChange.z);
            }

        if (!draggingObject)
        {
            DefaultPlayerRotation();
        }
    }

    private void DefaultPlayerRotation()
    {
        if (moveDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            Quaternion smoothedRotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
            rb.MoveRotation(smoothedRotation);
        }
    }

    private void TopDownMovement()
    {
        // forwardAngularOffsetFromWorldZ is an angle in degrees determining which world direction is forwards,
        // with the default (0) being world positive Z.
        Vector3 forwardDirection = new Vector3(0f, forwardAngularOffsetFromWorldZ, 0f);

        moveDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        moveDirection = Quaternion.Euler(forwardDirection) * moveDirection;

        anim.SetBool("isWalking", moveDirection.magnitude > 0);


        Vector3 targetPosition = rb.position + moveDirection * playerSpeed * Time.fixedDeltaTime;
        rb.MovePosition(targetPosition);

        DefaultPlayerRotation();
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
        bool groundDetected = false;
        if(SphereCastDetectsGround(transform.position))
        {
            groundDetected = true;
        }

        Vector3 secondCastPosition = secondGroundCastLocator.position;
        if(SphereCastDetectsGround(secondCastPosition))
        {
            groundDetected = true;
        }

        grounded = groundDetected;

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

    private bool SphereCastDetectsGround(Vector3 castPosition)
    {
        if (Physics.SphereCast(castPosition, 0.3f, Vector3.down, out RaycastHit hit, playerHeight * 0.5f))
        {
            return hit.collider.CompareTag("Ground") || hit.collider.CompareTag("Object") || hit.collider.CompareTag("Stairs");
        }
        else
        {
            return false;
        }
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

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Stairs")
        {
            rb.useGravity = false;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Stairs")
        {
            rb.useGravity = true;
        }
    }

    private void OnEnable()
    {
        Cameras.OnEnterTopDownCamera += SetInTopDownMode;
        Draggable.OnDragObject += SetDraggingObject;
    }

    private void OnDisable()
    {
        Cameras.OnEnterTopDownCamera -= SetInTopDownMode;
        Draggable.OnDragObject -= SetDraggingObject;
    }
}
