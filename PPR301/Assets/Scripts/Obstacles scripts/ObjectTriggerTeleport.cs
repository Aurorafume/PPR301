using UnityEngine;

public class ObjectTriggerTeleport : MonoBehaviour
{
    public GameObject triggerObject;         
    public Transform objectToTeleport;       
    public Transform teleportTarget;         
    public GameObject objectToDisappear;      

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == triggerObject)
        {
            // Teleport the target object
            if (objectToTeleport != null && teleportTarget != null)
            {
                objectToTeleport.position = teleportTarget.position;
            }

            // Disable the object to disappear
            if (objectToDisappear != null)
            {
                objectToDisappear.SetActive(false);
            }

            // Disable the trigger object itself
            triggerObject.SetActive(false);
        }
    }
}