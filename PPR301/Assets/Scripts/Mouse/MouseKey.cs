using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseKey : MonoBehaviour
{
    [Header("Key Settings")]
    [Tooltip("The key prefab that will be dropped when the mouse is touched.")]
    public GameObject keyPrefab;

    [Tooltip("Offset from mouse position where the key will be dropped.")]
    public Vector3 dropOffset = new Vector3(0, 0.5f, 0);

    [Tooltip("Whether the mouse is currently holding the key.")]
    public bool hasKey = true;

    [Header("Detection Settings")]
    [Tooltip("Tag used to identify the player object.")]
    public string playerTag = "Player";

    private bool keyDropped = false;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the mouse has the key and if the other object is tagged as Player
        if (!hasKey || keyDropped) return;

        // If the other object is the player, drop the key
        if (other.CompareTag(playerTag))
        {
            DropKey();
        }
    }

    void DropKey()
    {   
        // Instantiate the key at the mouse's position with an offset
        Vector3 dropPosition = transform.position + dropOffset;
        Instantiate(keyPrefab, dropPosition, Quaternion.identity);
        hasKey = false;
        keyDropped = true;
        Debug.Log("Mouse dropped the key!");
    }
}