using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstrumentCircle : MonoBehaviour
{
    [Header("Instruments in the scene")]
    [Tooltip("List of instrument GameObjects the player can interact with.")]
    public GameObject[] instruments;

    private NoiseHandler noiseHandler;
    private GameObject player;
    public Collider playerCollider;

    private void Start()
    {
        noiseHandler = FindObjectOfType<NoiseHandler>();
        if (noiseHandler == null)
        {
            Debug.LogError("InstrumentCircle: NoiseHandler not found in scene.");
        }

        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("InstrumentCircle: No GameObject with tag 'Player' found.");
        }
    }

    private void Update()
    {
        Debug.Log("Update called in InstrumentCircle");
        if (noiseHandler == null || player == null) return;

        foreach (GameObject instrument in instruments)
        {
            if (instrument != null && IsTouchingInstrument(instrument))
            {
                noiseHandler.TrySpawnEnemyManager(); 
                Debug.Log("Triggered spawn from instrument: " + instrument.name);
                break; // Only spawn once
            }
        }
    }

    private bool IsTouchingInstrument(GameObject instrument)
    {
        Collider instrumentCollider = instrument.GetComponent<Collider>();

        if (instrumentCollider == null)
        {
            Debug.LogWarning($"Instrument '{instrument.name}' has no collider.");
            return false;
        }

        if (playerCollider == null)
        {
            Debug.LogError("Player has no collider.");
            return false;
        }

        return instrumentCollider.bounds.Intersects(playerCollider.bounds);
    }
}