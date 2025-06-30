using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawPrint2 : MonoBehaviour
{
    private PlayerMovement script;

    [Header("Paw Print Settings")]
    [Tooltip("Prefab of the paw print to instantiate under the mouse.")]
    public GameObject pawPrefab;

    [Tooltip("Time in seconds between each paw print step.")]
    public float stepInterval = 0.5f;

    [Tooltip("Horizontal offset between left and right paw prints.")]
    public float pawOffsetX = 0.1f;

    [Tooltip("Vertical offset to align paw print with the floor (adjust for height).")]
    public float negatePawHeight = 0.1f;

    [Header("Debug & Runtime")]
    [Tooltip("List of currently spawned paw prints (for fading and cleanup).")]
    public List<GameObject> spawnedPaws = new List<GameObject>();

    private float timeSinceLastStep = 0f;
    private int pawIndex = 0; // Used to alternate between left/right paw prints

    private Vector3 lastPosition;

    void Start()
    {
        lastPosition = transform.position;
        //assign script
        script = GetComponent<PlayerMovement>();
    }

    void Update()
    {   
        // Check if the mouse has moved significantly since the last step
        float movedDistance = Vector3.Distance(transform.position, lastPosition);
        if (movedDistance > 0.01f) // Adjust this threshold as needed
        {
            timeSinceLastStep += Time.deltaTime;
            if (timeSinceLastStep >= stepInterval) // Check if it's time to leave a paw print
            {
                LeavePawPrint();
                timeSinceLastStep = 0f;
                lastPosition = transform.position;
            }
        }

        FadePaws();
    }

    void LeavePawPrint()
    {
        if(script.grounded)
        {
            //Debug.Log("Is Grounded");
            // Alternate left/right paw positions
            float offsetX = (pawIndex % 2 == 0) ? pawOffsetX : -pawOffsetX;
            pawIndex++;

            // Calculate the spawn position based on the mouse's current position and the offset
            Vector3 offset = new Vector3(offsetX, -negatePawHeight, 0f);
            Vector3 spawnPosition = transform.position + transform.rotation * offset;

            // Instantiate the paw print at the calculated position
            GameObject newPaw = Instantiate(pawPrefab, spawnPosition, transform.rotation * Quaternion.Euler(90, 0, 0));
            SpriteRenderer spriteRenderer = newPaw.GetComponent<SpriteRenderer>();
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);
            spawnedPaws.Add(newPaw);
        }

    }
    void FadePaws()
    {   
        // Fade out and remove paw prints over time
        for (int i = spawnedPaws.Count - 1; i >= 0; i--)
        {
            GameObject paw = spawnedPaws[i];
            if (paw.TryGetComponent(out SpriteRenderer spriteRenderer))
            {
                Color color = spriteRenderer.color;
                color.a -= Time.deltaTime / 1.1f; // Fade over time
                spriteRenderer.color = color;

                if (color.a <= 0) // If the alpha is zero, destroy the paw print
                {
                    Destroy(paw);
                    spawnedPaws.RemoveAt(i);
                }
            }
        }
    }
}
