// ==========================================================================
// Meowt of Tune - My Light
// Owned and contributed to by: brookcoli (Itch.io) - Aurorafume (GitHub), donkzilla03 (Itch.io) - AliAK03 (GitHub), JaydenFielderTorrens (Itch.io) - Squib35 (GitHub), komorebimoriko (Itch.io), TheRealCrizz (GitHub), LordVGahn (GitHub)
// itch.io listing: https://brookcoli.itch.io/meowt-of-tune - GitHub Repository: https://github.com/Aurorafume/PPR301
// ==========================================================================
//
// WHAT DOES THIS DO:
// This script procedurally generates a dynamic, cone-shaped mesh to visualise a
// light source in the game world. It uses a fan of raycasts to detect
// obstacles and adjusts the mesh shape in real-time to conform to the
// environment.
//
// Core functionalities include:
// - Creating a visible light cone effect with customisable angle, range, and detail.
// - Detecting when the light hits the player.
// - Optionally acting as a hazard that triggers a gameplay event (a noise spike)
//   when the player is detected.
// - Option for the light to pass through or be blocked by scene geometry.
//
// Dependencies:
// - UnityEngine for mesh generation and physics.
// - Requires MeshFilter and MeshRenderer components on the same GameObject.
// - NoiseBar custom script for the hazard interaction logic.
//
// ==========================================================================

using UnityEngine;

/// <summary>
/// Generates a dynamic light cone mesh using raycasting and detects the player.
/// </summary>
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MyLight : MonoBehaviour
{
    [Header("Light Cone Settings")]
    [Tooltip("The number of rays to cast. More rays create a smoother cone.")]
    public int rayCount = 10;
    [Tooltip("The angle in degrees between each ray.")]
    public float angleStep = 15f;
    [Tooltip("The maximum length of the light rays and the cone.")]
    public float rayLength = 10f;

    [Header("Behavior Settings")]
    [Tooltip("If true, the light will trigger a noise spike when it hits the player.")]
    public bool hazard;
    [Tooltip("If true, the light rays will pass through all objects.")]
    public bool goThrough;

    [Header("Component & Object References")]
    [Tooltip("Reference to the player GameObject (currently unused in this script's logic).")]
    public GameObject player;
    [Tooltip("Reference to the OutOfBounds script (currently unused in this script's logic).")]
    public OutOfBounds script;
    [Tooltip("Reference to the EnemySpawning script (currently unused in this script's logic).")]
    public EnemySpawning enemySpawning;
    [Tooltip("Reference to the NoiseBar script to trigger a noise spike.")]
    public NoiseBar noiseBar;

    // The procedural mesh for the light cone.
    private Mesh mesh;

    /// <summary>
    /// Initialises the mesh and assigns it to the MeshFilter on startup.
    /// </summary>
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    /// <summary>
    /// Calls the raycasting logic every frame to keep the light cone mesh updated.
    /// </summary>
    void Update()
    {
        GenerateLightConeMesh();
    }

    /// <summary>
    /// Procedurally generates a light cone mesh by casting a fan of rays. It updates vertices based on
    /// raycast hits and triggers hazard logic if the player is detected.
    /// </summary>
    void GenerateLightConeMesh()
    {
        // An extra vertex is needed for the origin point of the light.
        Vector3[] vertices = new Vector3[rayCount + 1];
        // The number of triangles needed to form the cone mesh.
        int[] triangles = new int[(rayCount - 1) * 3];

        // The first vertex is the light's origin point.
        vertices[0] = Vector3.zero;

        for (int i = 0; i < rayCount; i++)
        {
            // Calculate the angle for the current ray to create a centered fan shape.
            float angle = (i - (rayCount - 1) / 2f) * angleStep;
            Vector3 direction = Quaternion.Euler(0, angle, 0) * transform.forward;

            RaycastHit hit;
            Vector3 endPoint = direction * rayLength; // Default to max length.

            if (Physics.Raycast(transform.position, direction, out hit, rayLength))
            {
                // If the 'goThrough' flag is false, the ray stops at the first collision.
                if (!goThrough)
                {
                    endPoint = direction * hit.distance;
                }

                // If this is a hazard light and it hits the player, trigger the noise spike.
                if (hit.collider.CompareTag("Player") && hazard)
                {
                    if (noiseBar != null)
                    {
                        noiseBar.ForceNoiseSpikeFromLight();
                    }
                    else
                    {
                        Debug.LogWarning("NoiseBar reference is missing on light: " + gameObject.name);
                    }
                }
            }

            // Set the vertex position in local space.
            vertices[i + 1] = transform.InverseTransformPoint(transform.position + endPoint);

            // For debugging purposes, draw the ray in the scene view.
            Debug.DrawRay(transform.position, endPoint, Color.yellow);
        }

        // Create the triangle indices for the mesh.
        for (int i = 0; i < rayCount - 1; i++)
        {
            triangles[i * 3] = 0;           // Origin point
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        // Clear previous mesh data and apply the new geometry.
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
}