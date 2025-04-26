using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lock : MonoBehaviour
{
    public GameObject door;

    GameObject key;

    void OnTriggerEnter(Collider collider)
    {
        Carriable carriable = collider.transform.root.GetComponentInChildren<Carriable>();
        if (carriable)
        {
            if (carriable.objectType == Carriable.ObjectType.key)
            {
                key = carriable.gameObject;
                Unlock();
            }
        }
    }

    void Unlock()
    {
        Destroy(key);
        door.SetActive(false);
        Destroy(gameObject);
    }
}
