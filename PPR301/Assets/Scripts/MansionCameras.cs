using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MansionCameras : MonoBehaviour
{
    public Camera camera1;  // Reference to Camera 1
    public Camera camera2;  // Reference to Camera 2

    // Start is called before the first frame update
    void Start()
    {
        if (camera1) camera1.depth = 1;
        if (camera2) camera2.depth = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
