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
    private List<AudioSource> allSounds = new List<AudioSource>();

    public Buttons buttons;
    // Start is called before the first frame update
void Start()
{
    buttons = GameObject.Find("Button Manager")?.GetComponent<Buttons>();
    // Add each AudioSource from each list to allSounds
    allSounds.AddRange(gongsBreakDoor);
    allSounds.AddRange(meowList);
    allSounds.AddRange(instruments);
    allSounds.AddRange(saxLicks);

    allSounds.Add(raccoonLaugh);
    allSounds.Add(pickUpKey);
    allSounds.Add(unlockDoor);
    allSounds.Add(trumpetBang);

}


    // Update is called once per frame
    void Update()
    {
        // Set all sound volumes to 0
        foreach (AudioSource sound in allSounds)
        {
            if (sound != null)
                sound.volume = buttons.audioSlider.value;
        } 
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
