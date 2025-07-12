using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lock : MonoBehaviour
{
    public GameObject door;
    public LockType lockType;
    GameObject key;
    public enum LockType
    {
        GreenLock,
        BlueLock
    }

    void OnTriggerEnter(Collider collider)
    {
        Carriable carriable = collider.transform.root.GetComponentInChildren<Carriable>();
        if (carriable)
        {
            if (carriable.objectType == Carriable.ObjectType.key)
            {
                key = carriable.gameObject;
                if(key.CompareTag("Blue Key") && lockType == LockType.BlueLock)
                {
                    Debug.Log("OPENING BLUE DOOR");
                    Unlock();
                }
                else if(key.CompareTag("Green Key") && lockType == LockType.GreenLock)
                {
                    Debug.Log("OPENING Green DOOR");
                    Unlock();
                }
                else
                {
                    Debug.Log("not key");
                }
            }
        }
    }

    void Unlock()
    {
        Destroy(key);
        door.SetActive(false);
        Destroy(gameObject);
        //Debug.Log("OPEN DOOR!!");
    }
}
