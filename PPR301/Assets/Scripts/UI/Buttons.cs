using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Buttons : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Play()
    {
        SceneManager.LoadScene(1);
    }
    public void message()
    {
        Debug.Log("hello");
    }

    public void PlayDemo()
    {
        // Load the demo scene
        SceneManager.LoadScene("MainDemoA2");
    }
}
