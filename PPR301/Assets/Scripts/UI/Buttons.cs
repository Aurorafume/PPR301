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
    //stylus animator
    public Animator stylusAnimator;
    //slider
    public Slider musicSlider;
    void Start()
    {
        musicList[musicIndex].Play();
        //slider
        musicSlider.value = musicList[musicIndex].volume;
        musicSlider.onValueChanged.AddListener(SetVolume);
    }
    void Update()
    {
        //musicSlider.onValueV
    }
    void Awake()
    {
        DontDestroyOnLoad(musicObj);
    }
    public void SetVolume(float volume)
    {
        musicList[musicIndex].volume = volume;
        if(musicList[musicIndex].volume == 0)
        {
            mute = true;
            stylusAnimator.SetBool("Mute", true);
            audioButton.sprite = mutedUI;
        }
        else if(musicList[musicIndex].volume > 0)
        {
            mute = false;
            stylusAnimator.SetBool("Mute", false);
            audioButton.sprite = audioUI;
        }
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
        if(!mute)
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
    }
    public void MuteMusic()
    {
        Debug.Log("Muted!!!");
        if(!mute)
        {
            musicList[musicIndex].mute = true;
            mute = true;
            stylusAnimator.SetBool("Mute", true);
            audioButton.sprite = mutedUI;
        }
        else
        {
            if(musicList[musicIndex].volume > 0)
            {
                musicList[musicIndex].mute = false;
                mute = false;
                stylusAnimator.SetBool("Mute", false);
                audioButton.sprite = audioUI;
            }
        }
    }
}
