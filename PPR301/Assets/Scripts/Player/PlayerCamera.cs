// ==========================================================================
// Meowt of Tune - Player Camera
// Owned and contributed to by: brookcoli (Itch.io) - Aurorafume (GitHub), donkzilla03 (Itch.io) - AliAK03 (GitHub), JaydenFielderTorrens (Itch.io) - Squib35 (GitHub), komorebimoriko (Itch.io), TheRealCrizz (GitHub), LordVGahn (GitHub)
// itch.io listing: https://brookcoli.itch.io/meowt-of-tune - GitHub Repository: https://github.com/Aurorafume/PPR301
// ==========================================================================
//
// WHAT DOES THIS DO:
// This script implements a smooth third-person orbit camera. It follows a
// target player, rotates based on mouse input, supports zooming with the
// scroll wheel, and includes collision detection to prevent the camera from
// clipping through walls or other objects in the environment.
//
// Core functionalities include:
// - Following a target 'player' transform smoothly.
// - Mouse-controlled yaw (left/right) and pitch (up/down) rotation with angle limits.
// - Scroll-wheel controlled zoom with minimum and maximum distance limits.
// - Smooth interpolation for both camera movement and rotation.
// - Raycast-based collision detection to pull the camera forward when obstructed.
// - Locking and hiding the mouse cursor for a seamless gameplay experience.
//
// Dependencies:
// - Requires a 'player' Transform to be assigned in the Inspector.
// - The core movement logic is placed in 'LateUpdate' to ensure it runs
//   after the player has moved for the frame.
// - The collision detection ignores objects tagged as "Spawn Collider".
//
// ==========================================================================

using UnityEngine;

/// <summary>
/// A third-person orbit camera that follows the player, with smooth movement and collision detection.
/// </summary>
public class PlayerCamera : MonoBehaviour
{
    [Header("Target & Offsets")]
    [Tooltip("The player transform that the camera will follow.")]
    public Transform player;
    [Tooltip("The camera's offset from the player. Only the Y-value is used for height adjustment.")]
    public Vector3 heightOffset = new Vector3(0, 2, -4);
    [Tooltip("The distance the camera should maintain from colliders.")]
    public float collisionOffset = 0.25f;

    [Header("Control Settings")]
    [Tooltip("How sensitive the camera rotation is to mouse movement.")]
    public float sensitivity = 3f;
    [Tooltip("How quickly the camera moves and rotates to its target position.")]
    public float smoothSpeed = 10f;
    [Tooltip("The minimum up/down angle of the camera.")]
    public float minPitch = -30f;
    [Tooltip("The maximum up/down angle of the camera.")]
    public float maxPitch = 60f;

    [Header("Zoom Settings")]
    [Tooltip("How sensitive the zoom is to the mouse scroll wheel.")]
    public float zoomSpeed = 2f;
    [Tooltip("The closest the camera can zoom in to the player.")]
    public float minZoom = 2f;
    [Tooltip("The farthest the camera can zoom out from the player.")]
    public float maxZoom = 8f;

    // --- Private State Variables ---
    private float pitch = 0f;       // The current up/down rotation angle.
    private float yaw = 0f;         // The current left/right rotation angle.
    private float currentZoom = 4f; // The current distance from the player.

    /// <summary>
    /// Initialises the cursor state for gameplay.
    /// </summary>
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    /// <summary>
    /// Handles all player input for camera control (rotation and zoom) each frame.
    /// </summary>
    void Update()
    {
        // Get mouse input for rotation and apply sensitivity.
        yaw += Input.GetAxis("Mouse X") * sensitivity;
        pitch -= Input.GetAxis("Mouse Y") * sensitivity;
        // Clamp the pitch to prevent the camera from flipping over.
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // Handle zoom with the scroll wheel.
        currentZoom -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        // Clamp the zoom level to the defined min/max distances.
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
    }

    /// <summary>
    /// Calculates and applies the final camera position and rotation after the player has moved.
    /// Using LateUpdate ensures the camera tracks the player's updated position for the frame.
    /// </summary>
    void LateUpdate()
    {
        if (!player) return;

        // Compute the target rotation from the yaw and pitch angles.
        Quaternion targetRotation = Quaternion.Euler(pitch, yaw, 0);

        // Smoothly rotate the camera towards the target rotation.
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * smoothSpeed);

        // Calculate the camera's desired position based on rotation, zoom, and height offset.
        Vector3 desiredPosition = player.position - transform.forward * currentZoom + Vector3.up * heightOffset.y;

        // Perform a collision check to prevent the camera from moving through objects.
        RaycastHit hit;
        // Cast a ray from the player towards the camera's desired position.
        if (Physics.Raycast(player.position, desiredPosition - player.position, out hit, currentZoom + 1f) && hit.collider.tag != "Spawn Collider")
        {
            // If the ray hits an obstacle, move the desired position to the collision point.
            desiredPosition = hit.point + hit.normal * collisionOffset;
        }

        // Smoothly transition the camera's actual position to the final desired position.
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * smoothSpeed);
    }
}