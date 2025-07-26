using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public void ActivateCheckpoint()
    {
        Quaternion playerRotation = FindObjectOfType<PlayerMovement>().transform.rotation;
        CheckpointManager.Instance.SetCheckpoint(transform.position, playerRotation);
    }
}
