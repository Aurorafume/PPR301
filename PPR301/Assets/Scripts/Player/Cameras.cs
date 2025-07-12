using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cameras : MonoBehaviour
{
    public Camera camera1;  // Reference to Camera 1
    public Camera camera2;  // Reference to Camera 2
    public Camera camera3;  // Reference to Camera 3
    public Camera camera4;  
    public Camera camera5;
    public Camera camera6;
    public PlayerMovement script;
    public PlayerCamera camera;
    public OutOfBounds BoundsScript;
    public Camera KeyCamera;
    public int number = -180;
    //smooth camera
    public GameObject obj;
    public Vector3 pos1;
    public GameObject pos2;
    public bool move;
    private Vector3 velocity = Vector3.zero;

    public static event Action<bool, float> OnEnterTopDownCamera;
    public float topDownForwardDirection = -90;//demo 2 is -90
    //private float num2 = 0;

    void Start()
    {
        if (camera1) camera1.depth = 1;
        if (camera2) camera2.depth = 0;
        if (camera3) camera3.depth = 0;
        if (camera4) camera4.depth = 0;

        pos1 = obj.transform.position;
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
            move = false;
            //camera1.depth = 1;
            //KeyCamera.depth = 0;
        }
        // Switch to Camera 2 when pressing the "2" key
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            move = true;
            //obj.transform.position = pos2.transform.position;
            //camera1.depth = 0;
            //KeyCamera.depth = 1;
        }
        if(!move)
        {
            obj.transform.position = pos1;
        }
        else if(move)
        {
            obj.transform.position = Vector3.SmoothDamp(obj.transform.position, pos2.transform.position, ref velocity, 0.3f);
        }

    }
    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Area1"))
        {
            camera1.depth = 0;
            camera2.depth = 1;
            script.noJumpMode = true;
            OnEnterTopDownCamera?.Invoke(true, topDownForwardDirection);
            BoundsScript.currentRespawnLocation = BoundsScript.spawnLocationsArray[0];
        }
        else if (collision.gameObject.CompareTag("Area2"))
        {
            camera1.depth = 0;
            camera3.depth = 1;
            script.noJumpMode = true;
            OnEnterTopDownCamera?.Invoke(true, -90);
            BoundsScript.currentRespawnLocation = BoundsScript.spawnLocationsArray[1];
        }
        else if (collision.gameObject.CompareTag("Area3"))
        {
            camera1.depth = 0;
            camera4.depth = 1;
            script.noJumpMode = true;
            OnEnterTopDownCamera?.Invoke(true, -90);
            BoundsScript.currentRespawnLocation = BoundsScript.spawnLocationsArray[1];
        }
        //else if (collision.gameObject.CompareTag("Area3"))
        //{//
        //    //camera1.depth = 0;
        //    //camera3.depth = 0;
        //    //camera4.depth = 1;
        //    //script.noJumpMode = true;
        //    //OnEnterTopDownCamera?.Invoke(true, -90f);
        //    BoundsScript.currentRespawnLocation = BoundsScript.spawnLocationsArray[2];
        //    Debug.Log("RESPAWN");
        //}
        else if (collision.gameObject.CompareTag("Area4"))
        {
            camera1.depth = 0;
            camera5.depth = 1;
            script.noJumpMode = true;
            OnEnterTopDownCamera?.Invoke(true, -90);
            //BoundsScript.currentRespawnLocation = BoundsScript.spawnLocationsArray[2];
        }
        else if (collision.gameObject.CompareTag("Area5"))
        {
            camera1.depth = 0;
            camera6.depth = 1;
            script.noJumpMode = true;
            OnEnterTopDownCamera?.Invoke(true, number);
            //BoundsScript.currentRespawnLocation = BoundsScript.spawnLocationsArray[2];
        }
    }
    void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("Area1"))
        {
            camera2.depth = 0;
            camera1.depth = 1;
            script.noJumpMode = false;
            OnEnterTopDownCamera?.Invoke(false, topDownForwardDirection);
        }
        else if (collision.gameObject.CompareTag("Area2"))
        {
            camera3.depth = 0;
            camera1.depth = 1;
            script.noJumpMode = false;
            OnEnterTopDownCamera?.Invoke(false, -90);
        }
        else if (collision.gameObject.CompareTag("Area3"))
        {
            camera4.depth = 0;
            camera1.depth = 1;
            script.noJumpMode = false;
            OnEnterTopDownCamera?.Invoke(false, -90);
        }
        else if (collision.gameObject.CompareTag("Area4"))
        {
            camera3.depth = 0;
            camera1.depth = 1;
            script.noJumpMode = false;
            OnEnterTopDownCamera?.Invoke(false, -90);
        }
        else if (collision.gameObject.CompareTag("Area5"))
        {
            camera6.depth = 0;
            camera1.depth = 1;
            script.noJumpMode = false;
            OnEnterTopDownCamera?.Invoke(false, -90);
        }
    }
}
