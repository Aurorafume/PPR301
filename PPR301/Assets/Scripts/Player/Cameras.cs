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
    public Vector3 pos1;
    public GameObject pos2;
    public bool move;
    private Vector3 velocity = Vector3.zero;
    public float smoothSpeed;
    public bool robotFollow;
    public float number2;
    //followCamera
    public GameObject obj;
    public Camera followCamera;
    public List<Camera> cameraList = new List<Camera>();
    public float smoothingFactor;
    public int followCamArray;

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
        cameraList.Add(camera2);
        cameraList.Add(camera3);
        cameraList.Add(camera4);
        move = true;
    }
    void Number()
    {
        number2 -= Time.deltaTime * 0.1f;
    }

    // Update is called once per frame
    void Update()
    {
        keyCameras();
        //Number();
    }
    void keyCameras()
    {
        // Switch to Camera 1 when pressing the "1" key
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            move = true;
            //camera1.depth = 1;
            //KeyCamera.depth = 0;
        }
        // Switch to Camera 2 when pressing the "2" key
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            move = false;
            //obj.transform.position = pos2.transform.position;
            //camera1.depth = 0;
            //KeyCamera.depth = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            //obj.transform.position = pos2.transform.position;
            camera1.depth = 0;
            followCamera.depth = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            //obj.transform.position = pos2.transform.position;
            camera1.depth = 1;
            followCamera.depth = 0;
        }
        else if(move)
        {
            if(!robotFollow)
            {
                ////rotate
                //obj.transform.rotation = Quaternion.Slerp(obj.transform.rotation, camera1.transform.rotation, Time.deltaTime * 3);
                ////move
                //obj.transform.position = Vector3.SmoothDamp(obj.transform.position, camera1.transform.position, ref velocity, smoothSpeed);
                //smoothSpeed -= Time.deltaTime * 0.5f;
                // Calculate interpolation factor
                float t = 1 - Mathf.Exp(-smoothingFactor * Time.deltaTime);
                // Smoothly rotate
                obj.transform.rotation = Quaternion.Slerp(
                    obj.transform.rotation,
                    camera1.transform.rotation,
                    t
                );
                // Smoothly move
                obj.transform.position = Vector3.Lerp(
                    obj.transform.position,
                    camera1.transform.position,
                    t
                );
                smoothingFactor += Time.deltaTime * 100;
            }
            if (Vector3.Distance(obj.transform.position, camera1.transform.position) < 0.01f)
            {
                obj.transform.position = camera1.transform.position; // snap to final position
                robotFollow = true;
            }
            else if(robotFollow)
            {
                smoothingFactor = 0;
                obj.transform.position = camera1.transform.position;
                obj.transform.rotation = camera1.transform.rotation;
                //switch camera
                camera1.depth = 1;
                followCamera.depth = 0;
            }
            //Debug.Log("robotFollow");
        }
        if(!move)
        {
            //switch cams
            camera1.depth = 0;
            followCamera.depth = 1;
            //move cam
            robotFollow = false;
            smoothSpeed = 0.3f;
            obj.transform.position = Vector3.SmoothDamp(obj.transform.position, cameraList[followCamArray].transform.position, ref velocity, smoothSpeed);
            if (Vector3.Distance(obj.transform.position, cameraList[followCamArray].transform.position) < 0.01f)
            {
                obj.transform.position = cameraList[followCamArray].transform.position; // snap to final position
            }
            obj.transform.rotation = Quaternion.Slerp(obj.transform.rotation, cameraList[followCamArray].transform.rotation, Time.deltaTime * 3);
        }
    }
    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Area1"))
        {
            followCamArray = 0;
            move = false;
            script.noJumpMode = true;
            OnEnterTopDownCamera?.Invoke(true, topDownForwardDirection);
            BoundsScript.currentRespawnLocation = BoundsScript.spawnLocationsArray[0];
        }
        else if (collision.gameObject.CompareTag("Area2"))
        {
            followCamArray = 1;
            move = false;
            script.noJumpMode = true;
            OnEnterTopDownCamera?.Invoke(true, -90);
            BoundsScript.currentRespawnLocation = BoundsScript.spawnLocationsArray[1];
        }
        else if (collision.gameObject.CompareTag("Area3"))
        {
            followCamArray = 2;
            move = false;
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
        //else if (collision.gameObject.CompareTag("Area4"))
        //{
        //    camera1.depth = 0;
        //    camera5.depth = 1;
        //    script.noJumpMode = true;
        //    OnEnterTopDownCamera?.Invoke(true, -90);
        //    //BoundsScript.currentRespawnLocation = BoundsScript.spawnLocationsArray[2];
        //}
        //else if (collision.gameObject.CompareTag("Area5"))
        //{
        //    camera1.depth = 0;
        //    camera6.depth = 1;
        //    script.noJumpMode = true;
        //    OnEnterTopDownCamera?.Invoke(true, number);
        //    //BoundsScript.currentRespawnLocation = BoundsScript.spawnLocationsArray[2];
        //}
    }
    void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("Area1"))
        {
            move = true;
            script.noJumpMode = false;
            OnEnterTopDownCamera?.Invoke(false, topDownForwardDirection);
        }
        else if (collision.gameObject.CompareTag("Area2"))
        {
            move = true;
            script.noJumpMode = false;
            OnEnterTopDownCamera?.Invoke(false, -90);
        }
        else if (collision.gameObject.CompareTag("Area3"))
        {
            move = true;
            script.noJumpMode = false;
            OnEnterTopDownCamera?.Invoke(false, -90);
        }
        //else if (collision.gameObject.CompareTag("Area4"))
        //{
        //    camera3.depth = 0;
        //    camera1.depth = 1;
        //    script.noJumpMode = false;
        //    OnEnterTopDownCamera?.Invoke(false, -90);
        //}
        //else if (collision.gameObject.CompareTag("Area5"))
        //{
        //    camera6.depth = 0;
        //    camera1.depth = 1;
        //    script.noJumpMode = false;
        //    OnEnterTopDownCamera?.Invoke(false, -90);
        //}
    }
}
