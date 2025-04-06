using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float playerSpeed;
    public float groundDrag;
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    public float airControlSpeed = 5f;
    public float crouchHeight = 0.5f;
    public float crouchSpeed = 10f;
    public float playerHeight;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float topDownRotationSpeed = 50f;

    public bool isCrouching = false;
    public bool readyToJump;
    public bool grounded;
    private bool wasGrounded;
    private float horizontalInput;
    private float verticalInput;
    private float lastNoiseTime;

    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode crouchKey = KeyCode.LeftShift;

    public Transform orientation;
    public Camera playerCamera;
    public Transform crouchScaleObject;
    public NoiseHandler noiseHandler;
    public Animator anim;
    private Vector3 moveDirection;
    private Rigidbody rb;
    private Vector3 originalScale;

    public bool noJumpMode;
    public float idleTimeThreshold = 3f;
    private float lastMoveTime;

    private delegate void MoveBehaviour();
    private MoveBehaviour moveBehaviour;

    public bool draggingObject;
    public bool inTopDownMode;
    float forwardAngularOffsetFromWorldZ = 0;

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
        if (draggingObject)
        {
            moveBehaviour = DragMovement;
        }
        else if (inTopDownMode)
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
        }

        DefaultPlayerRotation();
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

    private void DragMovement()
    {
        verticalInput = Mathf.Clamp(verticalInput, float.MinValue, 0f);
        horizontalInput = 0f;

        moveDirection = (transform.forward * verticalInput + transform.right * horizontalInput).normalized;

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

    private void TopDownMovement()
    {
        // forwardAngularOffsetFromWorldZ is an angle in degrees determining which world direction is forwards,
        // with the default (0) being world positive Z.
        Vector3 forwardDirection = new Vector3(0f, forwardAngularOffsetFromWorldZ, 0f);

        moveDirection = new Vector3 (horizontalInput, 0f, verticalInput).normalized;

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
        if (Physics.SphereCast(transform.position, 0.3f, Vector3.down, out RaycastHit hit, playerHeight * 0.5f))
        {
            grounded = hit.collider.CompareTag("Ground") || hit.collider.CompareTag("Object");
        }
        else
        {
            grounded = false;
        }

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

    private void OnEnable()
    {
        Cameras.OnEnterTopDownCamera += SetInTopDownMode;
        PlayerInteractHandler.OnDragObject += SetDraggingObject;
    }

    private void OnDisable()
    {
        Cameras.OnEnterTopDownCamera -= SetInTopDownMode;
        PlayerInteractHandler.OnDragObject -= SetDraggingObject;
    }
}
