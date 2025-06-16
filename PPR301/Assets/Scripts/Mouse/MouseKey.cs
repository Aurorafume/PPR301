using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseKey : MonoBehaviour
{
    [Header("Key Settings")]
    [Tooltip("Reference to the key GameObject the mouse is carrying.")]
    public GameObject carriedKey;

    [Tooltip("Local position of the key in the mouse's mouth.")]
    public Vector3 mouthOffset = new Vector3(0, 0.2f, 0.3f);

    [Tooltip("Tag used to identify the player object.")]
    public string playerTag = "Player";

    private bool hasKey = true;
    //material
    public Material glowMaterial;
    public GameObject obj;
    //night vision
    public GameObject nightVision;

    void Start()
    {   
        // Ensure the carried key is set in the inspector
        if (carriedKey != null)
        {
            AttachKeyToMouth();
        }
    }

    private void OnTriggerEnter(Collider other)
    {   
        // Check if the mouse has the key and the other object is tagged as Player
        if (!hasKey) return;
        
        // If the other object is the player, drop the key
        if (other.CompareTag(playerTag))
        {
            DropKey();
        }
    }

    void AttachKeyToMouth()
    {   
        // Attach the key to the mouse's mouth position
        carriedKey.transform.SetParent(transform);
        carriedKey.transform.localPosition = mouthOffset;
        carriedKey.transform.localRotation = Quaternion.identity;

        // Disable physics while carried
        if (carriedKey.TryGetComponent(out Rigidbody rb))
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    void DropKey()
    {
        // Detach the key from the mouse
        carriedKey.transform.SetParent(null);

        // Enable physics
        if (carriedKey.TryGetComponent(out Rigidbody rb))
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        hasKey = false;
        Debug.Log("Mouse dropped the key!");
        //change material
        obj.GetComponent<MeshRenderer>().material = glowMaterial;
        //disable cat's night vision
        nightVision.SetActive(false);
    }
}
