using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class X_Y_Push : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.x > -90)
        {
            transform.position = new Vector3(-78,transform.position.y,transform.position.z);
        }
    }
}
