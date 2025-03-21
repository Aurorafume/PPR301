using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cameras : MonoBehaviour
{
    public Camera camera1;  // Reference to Camera 1
    public Camera camera2;  // Reference to Camera 2
    public Camera camera3;  // Reference to Camera 2
    public PlayerMovement script;
    // Start is called before the first frame update
    void Start()
    {
        camera1.depth = 1;
        camera2.depth = 0;
        camera3.depth = 0;
    }

    // Update is called once per frame
    void Update()
    {
        keyCameras();
    }
    void keyCameras()
    {
        // Switch to Camera 1 when pressing the "1" key
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            camera1.depth = 1;
            camera2.depth = 0;
        }
        // Switch to Camera 2 when pressing the "2" key
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            camera1.depth = 0;
            camera2.depth = 1;
        }
    }
    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Bridge"))
        {
            camera1.depth = 0;
            camera2.depth = 1;
            script.noJumpMode = true;
        }
        else if (collision.gameObject.CompareTag("Maze"))
        {
            camera1.depth = 0;
            camera3.depth = 1;
            script.noJumpMode = true;
        }
    }
    void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("Bridge"))
        {
            camera2.depth = 0;
            camera1.depth = 1;
            script.noJumpMode = false;
        }
        else if (collision.gameObject.CompareTag("Maze"))
        {
            camera3.depth = 0;
            camera1.depth = 1;
            script.noJumpMode = false;
        }
    }
}
