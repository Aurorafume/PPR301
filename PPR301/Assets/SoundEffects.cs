using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffects : MonoBehaviour
{
    public List<AudioSource> gongsBreakDoor = new List<AudioSource>();
    public List<AudioSource> meowList = new List<AudioSource>();
    public List<AudioSource> instruments = new List<AudioSource>();
    public AudioSource raccoonLaugh;
    public AudioSource pickUpKey;
    public AudioSource unlockDoor;
    public AudioSource trumpetBang;
    public List<AudioSource> saxLicks;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Meow()
    {
        meowList[Random.Range(0,3)].Play();
    }
    public void Sax()
    {
        saxLicks[Random.Range(0,3)].Play();
    }
}
