using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform player; // Assign the player transform
    public Vector3 offset = new Vector3(0, 2, -4); // Default camera position relative to the player
    public float sensitivity = 3f; // Mouse sensitivity
    public float smoothSpeed = 10f; // Camera follow smoothing speed

    public float minPitch = -30f; // Min vertical rotation
    public float maxPitch = 60f;  // Max vertical rotation
    public float zoomSpeed = 2f;
    public float minZoom = 2f;
    public float maxZoom = 8f;

    private float pitch = 0f;
    private float yaw = 0f;
    private float currentZoom = 4f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Lock cursor for camera control
        Cursor.visible = false; // Hide cursor
    }

    void Update()
    {
        // Get mouse input
        yaw += Input.GetAxis("Mouse X") * sensitivity;
        pitch -= Input.GetAxis("Mouse Y") * sensitivity;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch); // Clamp vertical rotation

        // Handle zoom with scroll wheel
        currentZoom -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
    }

    void LateUpdate()
    {
        if (!player) return;

        // Compute rotation (apply smoothing here instead of only position)
        Quaternion targetRotation = Quaternion.Euler(pitch, yaw, 0);

        // Smoothly rotate the camera
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * smoothSpeed);

        // Compute desired position
        Vector3 desiredPosition = player.position - transform.forward * currentZoom + Vector3.up * offset.y;

        // Collision handling
        RaycastHit hit;
        if (Physics.Raycast(player.position, desiredPosition - player.position, out hit, currentZoom + 1f))
        {
            desiredPosition = hit.point;
        }

        // Smoothly transition camera position (reduce smoothSpeed slightly if needed)
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * smoothSpeed);
    }
}
