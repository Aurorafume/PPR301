using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance { get; private set; }

    private Vector3 lastCheckpointPosition;
    private Quaternion lastCheckpointRotation;
    private bool hasCheckpoint = false;

    private void Awake()
    {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetCheckpoint(Vector3 position, Quaternion rotation)
    {
        lastCheckpointPosition = position;
        lastCheckpointRotation = rotation;
        hasCheckpoint = true;
    }

    public void SendPlayerToLastCheckpoint()
    {
        GameObject player = FindObjectOfType<PlayerMovement>().gameObject;
        player.transform.position = lastCheckpointPosition;
        player.transform.rotation = lastCheckpointRotation;
    }

    public bool HasCheckpoint()
    {
        return hasCheckpoint;
    }
}
