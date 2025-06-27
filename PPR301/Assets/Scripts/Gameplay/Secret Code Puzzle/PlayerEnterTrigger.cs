using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerEnterTrigger : MonoBehaviour
{
    public UnityEvent OnPlayerEnter;
    public bool destroyOnEnter;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            PlayerEnter();
        }
    }

    void PlayerEnter()
    {
        OnPlayerEnter.Invoke();

        if (destroyOnEnter)
        {
            Destroy(gameObject);
        }
    }
}
