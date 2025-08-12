// ==========================================================================
// Meowt of Tune - Player Movement
// Owned and contributed to by: brookcoli (Itch.io) - Aurorafume (GitHub), donkzilla03 (Itch.io) - AliAK03 (GitHub), JaydenFielderTorrens (Itch.io) - Squib35 (GitHub), komorebimoriko (Itch.io), TheRealCrizz (GitHub), LordVGahn (GitHub)
// itch.io listing: https://brookcoli.itch.io/meowt-of-tune - GitHub Repository: https://github.com/Aurorafume/PPR301
// ==========================================================================
//
// WHAT DOES THIS DO:
// This is a comprehensive, state-based character controller script. It manages
// all aspects of player movement, interaction, and physical state within the game
// world, including a system to switch between different control schemes.
//
// Core functionalities include:
// - Standard third-person movement (walking, jumping, air control).
// - A secondary top-down movement mode with world-relative controls.
// - Crouching mechanics that adjust the player's scale and collider.
// - A noise system that generates sound on landing to alert enemies.
// - A state machine for handling different behaviors (e.g. default vs. top-down).
// - Animation control for walking, jumping, falling, and idling.
// - Special interaction handlers for stairs and dragging objects.
//
// Dependencies:
// - UnityEngine for core and physics functionality.
// - NoiseHandler, Cameras, Draggable, and ContactNoise custom scripts.
// - Animator, Rigidbody, Camera, and SphereCollider components.
//
// ==========================================================================

using System.Collections;
using UnityEngine;

/// <summary>
/// A comprehensive character controller handling multiple movement states, animations, and interactions.
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("How fast the player moves while grounded.")]
    public float playerSpeed;
    [Tooltip("Amount of drag applied to the player when grounded to prevent sliding.")]
    public float groundDrag;
    [Tooltip("Upward force applied when the player jumps.")]
    public float jumpForce;
    [Tooltip("Cooldown time in seconds between jumps.")]
    public float jumpCooldown;
    [Tooltip("Additional force applied when falling.")]
    public float fallMultiplier = 2.5f;
    [Tooltip("How much control the player has over movement while in the air.")]
    public float airControlSpeed = 5f;

    [Header("Crouch Settings")]
    [Tooltip("The player's vertical scale when crouching (e.g. 0.5 for half height).")]
    public float crouchHeight = 0.5f;
    [Tooltip("The speed of the transition animation between crouching and standing.")]
    public float crouchSpeed = 10f;
    [Tooltip("The standing height of the player, used for ground detection raycasts.")]
    public float playerHeight;
    [Tooltip("Reference to the player's main SphereCollider.")]
    public SphereCollider playerCollider;

    [Header("Rotation Settings")]
    [Tooltip("How quickly the player rotates to face the movement direction in 3D mode.")]
    [SerializeField] private float rotationSpeed = 5f;

    [Header("Audio Settings")]
    [Tooltip("The AudioSource component used for playing the object dragging sound.")]
    public AudioSource draggingAudioSource;
    [Tooltip("The sound clip that loops while the player is dragging a moving object.")]
    public AudioClip draggingSoundClip;
    [Tooltip("The volume of the dragging sound.")]
    [Range(0f, 1f)]
    public float draggingSoundVolume = 0.5f;

    [Header("Key Bindings")]
    [Tooltip("The key used to initiate a jump.")]
    public KeyCode jumpKey = KeyCode.Space;
    [Tooltip("The key used to toggle crouching.")]
    public KeyCode crouchKey = KeyCode.LeftShift;

    [Header("Component References")]
    [Tooltip("The Transform that defines the forward direction for movement calculations.")]
    public Transform orientation;
    [Tooltip("A reference to the main player camera.")]
    public Camera playerCamera;
    [Tooltip("The child Transform that will be scaled vertically for the crouch effect.")]
    public Transform crouchScaleObject;
    [Tooltip("A reference to the scene's NoiseHandler for generating sound events.")]
    public NoiseHandler noiseHandler;
    [Tooltip("The Animator component controlling the player's animations.")]
    public Animator anim;
    [Tooltip("A secondary transform used for ground detection to help with uneven terrain.")]
    public Transform secondGroundCastLocator;

    [Header("Advanced State Settings")]
    [Tooltip("If true, the player's ability to jump is disabled.")]
    public bool noJumpMode;
    [Tooltip("Time in seconds of inactivity before the player is considered idle.")]
    public float idleTimeThreshold = 3f;
    
    [Header("State Flags (Read-Only)")]
    [Tooltip("Is the player currently crouching?")]
    public bool isCrouching = false;
    [Tooltip("Is the player ready to jump (i.e., not on cooldown)?")]
    public bool readyToJump;
    [Tooltip("Is the player currently on the ground?")]
    public bool grounded;
    [Tooltip("A flag to force the grounded state, used for handling stairs.")]
    public bool groundedAlwaysTrue = false;
    [Tooltip("Is the player currently dragging an object?")]
    public bool draggingObject;
    [Tooltip("Is the player currently in the top-down camera mode?")]
    public bool inTopDownMode;

    // Private state variables
    private float horizontalInput;
    private float verticalInput;
    private Vector3 moveDirection;
    private Rigidbody rb;
    public bool jumpPressed;

    // Delegate for switching movement behavior
    private delegate void MoveBehaviour();
    private MoveBehaviour moveBehaviour;

    // Internal timers and original values
    private float lastNoiseTime;
    private float lastMoveTime;
    private Vector3 originalScale;
    private float originalColliderRadius;
    private float forwardAngularOffsetFromWorldZ = 0;

    #region Unity Lifecycle Methods

    /// <summary>
    /// Called on startup to initialise components and default states.
    /// </summary>
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        readyToJump = true;
        anim = GetComponentInChildren<Animator>();
        
        // Cache original values for crouching.
        if (crouchScaleObject != null)
        {
            originalScale = crouchScaleObject.localScale;
        }
        if (playerCollider != null)
        {
            originalColliderRadius = playerCollider.radius;
        }

        // Set the default movement behavior.
        moveBehaviour = DefaultMovement;
    }

    /// <summary>
    /// Called every frame. Handles input, state checks, and non-physics updates.
    /// </summary>
    private void Update()
    {
        bool wasGrounded = grounded;
        CheckGroundStatus();

        // Generate noise only on the frame the player lands.
        if (!wasGrounded && grounded && !groundedAlwaysTrue)
            GenerateLandingNoise();

        GetPlayerInput();
        HandleCrouch();
        HandleIdleAndSoundStates();
        
        // Apply drag only when grounded.
        rb.drag = grounded ? groundDrag : 0;
    }

    /// <summary>
    /// Called at a fixed time interval. Handles physics-based updates like movement.
    /// </summary>
    private void FixedUpdate()
    {
        // Execute the current movement behavior (Default or TopDown).
        moveBehaviour();

        // Apply jump force in FixedUpdate for consistent physics.
        if (jumpPressed)
        {
            Jump();
            jumpPressed = false;
        }

        HandleJumpDownwardForce();
    }

    private void HandleJumpDownwardForce()
    {
        // Apply a downward force to increase fall speed when not holding the jump key.
        if (!grounded && rb.velocity.y > 0 && !Input.GetKey(jumpKey))
        {
            rb.AddForce(Vector3.up * Physics.gravity.y * fallMultiplier * rb.mass);
        }

        // Apply fall multiplier to increase downward force when falling.
        if (!grounded && rb.velocity.y < 0)
        {
            rb.AddForce(Vector3.up * Physics.gravity.y * fallMultiplier * rb.mass);
        }
    }

    /// <summary>
    /// Subscribes to events when the component is enabled.
    /// </summary>
    private void OnEnable()
    {
        Cameras.OnEnterTopDownCamera += SetInTopDownMode;
        Draggable.OnDragObject += SetDraggingObject;
    }

    /// <summary>
    /// Unsubscribes from events when the component is disabled to prevent memory leaks.
    /// </summary>
    private void OnDisable()
    {
        Cameras.OnEnterTopDownCamera -= SetInTopDownMode;
        Draggable.OnDragObject -= SetDraggingObject;
    }

    /// <summary>
    /// Handles collision enter events for special surfaces like stairs.
    /// </summary>
    void OnCollisionEnter(Collision collision)
    {
        // Check for custom noise components on the object we hit.
        ContactNoise contactNoise = collision.gameObject.GetComponent<ContactNoise>();
        if (contactNoise != null)
        {
            contactNoise.GenerateContactNoise();
        }
        
        // Special handling for stairs to prevent bouncing.
        if (collision.gameObject.CompareTag("Stairs"))
        {
            rb.useGravity = false;
            groundedAlwaysTrue = true;
        }
    }

    /// <summary>
    /// Handles collision exit events for special surfaces like stairs.
    /// </summary>
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Stairs"))
        {
            rb.useGravity = true;
            groundedAlwaysTrue = false;
        }
    }

    #endregion

    #region Input and State Management

    /// <summary>
    /// Gathers player input for movement and actions each frame.
    /// </summary>
    private void GetPlayerInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // Handle jump input.
        if (!noJumpMode && Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            jumpPressed = true;
            readyToJump = false;
            //Invoke(nameof(ResetJump), jumpCooldown);
        }
        if (Input.GetKeyUp(jumpKey))
        {
            readyToJump = true;
        }
        
        // Handle crouch input.
            if (Input.GetKeyDown(crouchKey) && grounded)
            {
                isCrouching = true;
            }
            else if (Input.GetKeyUp(crouchKey))
            {
                isCrouching = false;
            }
    }
    
    /// <summary>
    /// Smoothly transitions the player model's scale and collider size for crouching.
    /// </summary>
    private void HandleCrouch()
    {
        if (crouchScaleObject == null || playerCollider == null) return;

        // Animate the vertical scale of the crouch object.
        float targetHeight = isCrouching ? crouchHeight : originalScale.y;
        crouchScaleObject.localScale = new Vector3(
            originalScale.x,
            Mathf.MoveTowards(crouchScaleObject.localScale.y, targetHeight, crouchSpeed * Time.deltaTime),
            originalScale.z
        );

        // Animate the radius of the collider to match the crouch state.
        float heightScalar = crouchHeight / originalScale.y;
        float targetRadius = isCrouching ? originalColliderRadius * heightScalar : originalColliderRadius;
        playerCollider.radius = Mathf.MoveTowards(playerCollider.radius, targetRadius, crouchSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Checks for movement to update the idle animation state and handles the dragging sound.
    /// </summary>
    private void HandleIdleAndSoundStates()
    {
        bool isMoving = horizontalInput != 0 || verticalInput != 0 || rb.velocity.magnitude > 0.1f;
        HandleDraggingSound(isMoving);

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

    /// <summary>
    /// Updates the movement behavior delegate based on the current camera mode.
    /// </summary>
    public void UpdateMoveBehaviour()
    {
        if (inTopDownMode)
        {
            StopAllCoroutines();
            StartCoroutine(SetMoveBehaviourCoroutine(TopDownMovement));
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(SetMoveBehaviourCoroutine(DefaultMovement));
        }
    }
    
    /// <summary>
    /// Coroutine to switch the movement delegate after a short delay.
    /// </summary>
    private IEnumerator SetMoveBehaviourCoroutine(MoveBehaviour newBehaviour)
    {
        yield return new WaitForSeconds(0.5f);
        moveBehaviour = newBehaviour;
    }

    #endregion

    #region Movement Behaviors

    /// <summary>
    /// Standard third-person movement logic, relative to the camera's direction.
    /// </summary>
    private void DefaultMovement()
    {
        // Get camera's forward and right vectors, ignoring vertical rotation.
        Vector3 camForward = playerCamera.transform.forward;
        Vector3 camRight = playerCamera.transform.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        moveDirection = (camForward * verticalInput + camRight * horizontalInput).normalized;
        anim.SetBool("isWalking", moveDirection.magnitude > 0);

        if (grounded || groundedAlwaysTrue)
        {
            // Apply movement force when grounded.
            Vector3 targetPosition = rb.position + moveDirection * playerSpeed * Time.fixedDeltaTime;
            rb.MovePosition(targetPosition);
        }
        else
        {
            // Apply air control force when airborne.
            Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            Vector3 airMove = moveDirection * airControlSpeed;
            Vector3 velocityChange = Vector3.ClampMagnitude(airMove - horizontalVelocity, airControlSpeed);
            rb.AddForce(velocityChange, ForceMode.Acceleration);
        }
        
        // Rotate the player unless they are dragging an object.
        if (!draggingObject)
        {
            DefaultPlayerRotation();
        }
    }

    /// <summary>
    /// Handles the smooth rotation of the player to face the direction of movement.
    /// </summary>
    private void DefaultPlayerRotation()
    {
        if (moveDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            Quaternion smoothedRotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
            rb.MoveRotation(smoothedRotation);
        }
    }

    /// <summary>
    /// Top-down movement logic, relative to world axes with an angular offset.
    /// </summary>
    private void TopDownMovement()
    {
        Vector3 forwardDirection = new Vector3(0f, forwardAngularOffsetFromWorldZ, 0f);
        moveDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;
        moveDirection = Quaternion.Euler(forwardDirection) * moveDirection;

        anim.SetBool("isWalking", moveDirection.magnitude > 0);
        
        Vector3 targetPosition = rb.position + moveDirection * playerSpeed * Time.fixedDeltaTime;
        rb.MovePosition(targetPosition);

        DefaultPlayerRotation(); // Uses the same rotation logic.
    }

    /// <summary>
    /// Applies an upward force to the Rigidbody to make the player jump.
    /// </summary>
    private void Jump()
    {
        if(!draggingObject)
        {
            // Reset vertical velocity to ensure consistent jump height.
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            
            // Add a small forward boost to the jump.
            rb.AddForce(moveDirection * playerSpeed * 1f, ForceMode.Impulse);
            
            anim.SetBool("isJumping", true);
            Invoke(nameof(EnableGroundDetection), 0.1f);
        }
    }
    
    #endregion

    #region Ground Check and Noise

    /// <summary>
    /// Resets the jump cooldown, allowing the player to jump again.
    /// </summary>
    private void ResetJump()
    {
        readyToJump = true;
    }

    /// <summary>
    /// A small delay method called after a jump to re-enable ground detection.
    /// </summary>
    private void EnableGroundDetection()
    {
        CheckGroundStatus();
    }

    /// <summary>
    /// Checks for ground beneath the player using spherecasts and updates the animation state.
    /// </summary>
    private void CheckGroundStatus()
    {
        grounded = SphereCastDetectsGround(transform.position) || SphereCastDetectsGround(secondGroundCastLocator.position);
        anim.SetBool("isJumping", !grounded);
        
        // Set falling animation if airborne and moving downwards.
        if (!grounded && rb.velocity.y < -0.1f)
        {
            anim.SetBool("isFalling", true);
        }
        else if (grounded)
        {
            anim.SetBool("isFalling", false);
        }
    }

    /// <summary>
    /// Performs a spherecast downwards from a given position to detect ground.
    /// </summary>
    /// <returns>True if ground is detected, false otherwise.</returns>
    private bool SphereCastDetectsGround(Vector3 castPosition)
    {
        if (Physics.SphereCast(castPosition, 0.3f, Vector3.down, out RaycastHit hit, playerCollider.radius))
        {
            return hit.collider.CompareTag("Ground") || hit.collider.CompareTag("Object") || hit.collider.CompareTag("Stairs");
        }
        return false;
    }
    
    /// <summary>
    /// Generates a noise event when the player lands after a jump or fall.
    /// </summary>
    private void GenerateLandingNoise()
    {
        if (noiseHandler != null && Time.time - lastNoiseTime > 0.5f)
        {
            noiseHandler.GenerateNoise(Mathf.Abs(noiseHandler.jumpNoise));
            lastNoiseTime = Time.time;
        }
    }

    #endregion
    
    #region Audio
    
    /// <summary>
    /// Plays or stops the looping dragging sound based on player state.
    /// </summary>
    /// <param name="isMoving">Is the player currently in motion?</param>
    private void HandleDraggingSound(bool isMoving)
    {
        if (draggingAudioSource == null || draggingSoundClip == null) return;

        if (draggingObject && isMoving)
        {
            if (!draggingAudioSource.isPlaying)
            {
                draggingAudioSource.clip = draggingSoundClip;
                draggingAudioSource.volume = draggingSoundVolume;
                draggingAudioSource.loop = true;
                draggingAudioSource.Play();
            }
        }
        else
        {
            if (draggingAudioSource.isPlaying)
            {
                draggingAudioSource.Stop();
            }
        }
    }
    
    #endregion
    
    #region Event Handlers
    
    /// <summary>
    /// Event handler to set the dragging state.
    /// </summary>
    void SetDraggingObject(bool state)
    {
        draggingObject = state;
        UpdateMoveBehaviour();
    }

    /// <summary>
    /// Event handler to set the top-down mode state and orientation.
    /// </summary>
    void SetInTopDownMode(bool state, float forwardAngularDirection)
    {
        inTopDownMode = state;
        forwardAngularOffsetFromWorldZ = forwardAngularDirection;
        UpdateMoveBehaviour();
    }

    #endregion
}