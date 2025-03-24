using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform player;
    public Vector3 offset = new Vector3(0, 2, -4);
    public float sensitivity = 3f;
    public float smoothSpeed = 10f;
    public float minPitch = -30f;
    public float maxPitch = 60f;
    public float zoomSpeed = 2f;
    public float minZoom = 2f;
    public float maxZoom = 8f;
    private float pitch = 0f;
    private float yaw = 0f;
    private float currentZoom = 4f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Get mouse input
        yaw += Input.GetAxis("Mouse X") * sensitivity;
        pitch -= Input.GetAxis("Mouse Y") * sensitivity;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // Handle zoom with scroll wheel
        currentZoom -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
    }

    void LateUpdate()
    {
        if (!player) return;

        // Compute rotation
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

        // Smoothly transition camera position
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * smoothSpeed);
    }
}
