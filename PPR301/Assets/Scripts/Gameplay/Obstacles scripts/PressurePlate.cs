using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    public bool activated;
    //door to unlock
    public GameObject door;
    //materials
    public Material offMaterial;
    public Material glowMaterial;
    //button type
    public ButtonType buttonType;
    //touched by player or object?
    public TouchType touchBy;
    public GameObject obj;

    public enum ButtonType
    {
        TouchButton,
        HoldButton
    }
    public enum TouchType
    {
        Player,
        Object
    }

    void OnTriggerEnter(Collider collider)
    {
        switch (touchBy)
        {
            case TouchType.Player:
            if(collider.gameObject.CompareTag("Player") && !activated)
            {
                Debug.Log("Touched button");
                activated = true;
                //change colour
                GetComponent<MeshRenderer>().material = glowMaterial;
                door.SetActive(false);
            }
            break;
            case TouchType.Object:
            if(collider.gameObject == obj && !activated)
            {
                Debug.Log("Touched object");
                activated = true;
                //change colour
                GetComponent<MeshRenderer>().material = glowMaterial;
                door.SetActive(false);
            }
            break;
        }
        
    }
    void OnTriggerExit(Collider collider)
    {
        if(buttonType == ButtonType.HoldButton)
        {
            switch (touchBy)
            {
                case TouchType.Player:
                if(collider.gameObject.CompareTag("Player"))
                {
                    Debug.Log("Letting go of button");
                    activated = false;
                    //change colour
                    GetComponent<MeshRenderer>().material = offMaterial;
                    door.SetActive(true);
                }
                break;
                case TouchType.Object:
                if(collider.gameObject == obj)
                {
                    Debug.Log("Object Letting go of button");
                    activated = false;
                    //change colour
                    GetComponent<MeshRenderer>().material = offMaterial;
                    door.SetActive(true);
                }
                break;
            }
        }
    }
}
