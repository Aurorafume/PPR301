using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instrument : MonoBehaviour
{
    public AudioSource sound;
    public NoiseHandler noiseHandler;

    void OnCollisionEnter(Collision collision)
    {
        void Start()
        {
            //noiseHandler = FindObjectOfType<NoiseHandler>();
        }

        if(collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Hit Instrument!!!");
            //noiseHandler.TrySpawnEnemyManager(); 
            sound.Play();
        }
    }
}
