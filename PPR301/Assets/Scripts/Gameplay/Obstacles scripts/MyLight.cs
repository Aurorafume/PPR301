using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MyLight : MonoBehaviour
{
    public int rayCount = 10;
    public float angleStep = 15f;
    public float rayLength = 10f;
    public bool hazard;
    public bool goThrough;

    public GameObject player;
    public OutOfBounds script;
    public EnemySpawning enemySpawning;
    public NoiseBar noiseBar; // Reference to the NoiseBar script

    private Mesh mesh;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    void Update()
    {
        lightRayCast();
    }

    void lightRayCast()
    {
        Vector3[] vertices = new Vector3[rayCount + 1]; // Light source + ray endpoints
        int[] triangles = new int[(rayCount - 1) * 3]; // Triangle indices

        vertices[0] = Vector3.zero; // Light source at center

        for (int i = 0; i < rayCount; i++)
        {
            float angle = (i - (rayCount - 1) / 2f) * angleStep;
            Vector3 direction = Quaternion.Euler(0, angle, 0) * transform.forward;

            RaycastHit hit;
            Vector3 endPoint = direction * rayLength; // Default to max range

            if (Physics.Raycast(transform.position, direction, out hit, rayLength))
            {
                //go through certain objects
                if (!goThrough)
                {
                    endPoint = direction * hit.distance; // Stop at collision
                }
                //detect player
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
            vertices[i + 1] = transform.InverseTransformPoint(transform.position + endPoint);
            Debug.DrawRay(transform.position, endPoint, Color.yellow);
        }

        // Create triangle indices
        for (int i = 0; i < rayCount - 1; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        // Apply mesh
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
}
