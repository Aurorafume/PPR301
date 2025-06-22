using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowIconScript : MonoBehaviour
{

    public Transform player;
    public GameObject PlayerMouseIcon;
    private float dist;
    public float howFarShowIcon;
    public static List<ShowIconScript> objectList = new List<ShowIconScript>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bool isNearAny = false;
        foreach (ShowIconScript obj in objectList)
        {
            dist = Vector3.Distance(player.position, obj.transform.position);
            if (dist <= howFarShowIcon)
            {
                isNearAny = true;
                break; // Stop checking after finding one nearby
            }
        }
        if(isNearAny)
        {
            PlayerMouseIcon.SetActive(true);
            //icon faces camera
            PlayerMouseIcon.transform.LookAt(Camera.main.transform.position, Vector3.up);
            PlayerMouseIcon.transform.Rotate(0f, 180f, 0f);
        }
        else
        PlayerMouseIcon.SetActive(false);
    }
    void Awake()
    {
        objectList.Add(this);
    }
    void OnDestroy()
    {
        objectList.Remove(this);
    }

}
