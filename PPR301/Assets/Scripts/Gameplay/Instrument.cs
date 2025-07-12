using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instrument : MonoBehaviour
{
    public AudioSource sound;

    void OnCollisionEnter(Collision collision)
    {
        void Start()
        {

        }

        if(collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Hit Instrument!!!"); 
            sound.Play();
        }
    }
}
