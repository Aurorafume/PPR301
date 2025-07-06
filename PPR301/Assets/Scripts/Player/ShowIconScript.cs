using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowIconScript : MonoBehaviour
{

    public GameObject playerMouseIcon;

    // Update is called once per frame
    void Update()
    {
        if (playerMouseIcon.activeInHierarchy)
        {
            ContinuouslyFaceCamera();
        }
    }

    void ContinuouslyFaceCamera()
    {
        // Make sure icon always faces the camera.
        playerMouseIcon.transform.LookAt(Camera.main.transform.position, Vector3.up);
        playerMouseIcon.transform.Rotate(0f, 180f, 0f);
    }

    // Called by input manager to turn on/off icon.
    public void SetIconActive(bool active)
    {
        if (active)
        {
            playerMouseIcon.SetActive(true);
        }
        else
        {
            playerMouseIcon.SetActive(false);
        }
    }
}
