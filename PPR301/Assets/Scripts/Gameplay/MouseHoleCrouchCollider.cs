using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseHoleCrouchCollider : MonoBehaviour
{
    [SerializeField] Collider holeCollider;
    PlayerMovement playerMovement;
    bool isCrouching;

    void Awake()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();
    }

    void Start()
    {
        holeCollider.enabled = true;
    }
    bool playerInRange = false;
    public void SetPlayerInRange(bool value)
    {
        playerInRange = value;
    }

    void Update()
    {
        if (playerMovement != null)
        {
            isCrouching = playerMovement.isCrouching;
        }

        if (playerInRange && isCrouching)
        {
            holeCollider.enabled = false;
        }
        else
        {
            holeCollider.enabled = true;
        }
    }
}
