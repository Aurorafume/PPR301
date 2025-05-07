using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instrument : MonoBehaviour
{
    public AudioSource sound;
    public GameObject enemy;

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Hit Instrument!!!");
            sound.Play();
            enemy.SetActive(true);
        }
    }
}
