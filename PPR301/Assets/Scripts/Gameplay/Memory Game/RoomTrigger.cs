using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    [Tooltip("Reference to the main game state manager.")]
    public States gameStates;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that entered is the player
        if (other.CompareTag("Player"))
        {
            if (gameStates != null)
            {
                Debug.Log("Player has entered the memory game room.");
                gameStates.InMemoryRoomGame = true;
                
                // Disable the collider so this only triggers once.
                gameObject.GetComponent<Collider>().enabled = false;
            }
            else
            {
                Debug.LogError("RoomTrigger is missing a reference to the States script!");
            }
        }
    }
}