using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Buttons : MonoBehaviour
{
    public States states;
    //music
    public int musicIndex;
    public List<AudioSource> musicList = new List<AudioSource>();
    public GameObject musicObj;
    public bool mute;
    public Image audioButton;
    public Sprite audioUI;
    public Sprite mutedUI;
    void Start()
    {
        musicList[musicIndex].Play();
    }
    void Awake()
    {
        DontDestroyOnLoad(musicObj);
    }
    public void Play()
    {
        SceneManager.LoadScene("MainDemoA3");
    }
    public void message()
    {
        Debug.Log("hello");
    }
    public void PlayDemo()
    {   
        // Reset the game state
        states.ResetGameState();
        // Load the demo scene
        SceneManager.LoadScene("MainDemoA3");
    }
    public void PlayDemo2()
    {
        // Load the demo scene
        SceneManager.LoadScene("Demo_2");
    }
    public void ChangeMusic()
    {
        Debug.Log("Music changed!!");
        if(musicIndex < musicList.Count - 1)
        {
            musicList[musicIndex].Stop();
            musicIndex++;
        }
        else
        {
            musicList[musicIndex].Stop();
            musicIndex = 0;
        }
        musicList[musicIndex].Play();
    }
    public void MuteMusic()
    {
        Debug.Log("Muted!!!");
        if(!mute)
        {
            musicList[musicIndex].mute = true;
            mute = true;
            audioButton.sprite = mutedUI;
        }
        else
        {
            musicList[musicIndex].mute = false;
            mute = false;
            audioButton.sprite = audioUI;
        }
    }
}
