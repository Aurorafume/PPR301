using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    public GameObject door;
    public bool touchButton;
    private bool activated;
    //materials
    public Material offMaterial;
    public Material glowMaterial;

    void OnTriggerEnter(Collider collider)
    {
        if(activated == false)
        {
            activated = true;
            transform.localScale = new Vector3(1,0.26f,1);
            GetComponent<Renderer>().material = glowMaterial;
            door.SetActive(false);
        }
        else
        {
            activated = false;
            transform.localScale = new Vector3(1,1,1);
            GetComponent<Renderer>().material = offMaterial;
            door.SetActive(true);
        }
    }
    void OnTriggerExit(Collider collider)
    {
        if(touchButton == false)
        {
            activated = false;
            transform.localScale = new Vector3(1,1,1);
            GetComponent<Renderer>().material = offMaterial;
            door.SetActive(true);
        }
    }
}
