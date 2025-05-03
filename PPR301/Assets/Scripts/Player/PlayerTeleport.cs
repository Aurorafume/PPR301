using UnityEngine;

public class PlayerTeleport : MonoBehaviour
{
    public Transform player;
    public Transform teleportTarget;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player.position = teleportTarget.position;
        }
    }
}