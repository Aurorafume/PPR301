using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TunaScript : MonoBehaviour
{
    public GameObject VICTORYUI;
    // Start is called before the first frame update
    void Start()
    {
    
    }
    void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.CompareTag("Player"))
        {
            VICTORYUI.SetActive(true);
        }
    }
}
