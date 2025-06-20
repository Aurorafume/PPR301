using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootProjectile : MonoBehaviour
{
    public GameObject projectilePrefab;
    public int projectileCount; //how many projectiles should be out at once
    public int projectileCount2;
    public Transform startingSpawnLocator;
    public Vector3 projectileDirection = Vector3.forward;
    public float dist;
    public Transform player;
    public GameObject PlayerMouseIcon;
    public Camera cam;

    void Update()
    {
        //if(Input.MouseButtonDown(0))
        //{
        //    
        //}
        dist = Vector3.Distance(transform.position, player.position);
        if(dist < 3)
        {
            Debug.Log("close");
            PlayerMouseIcon.SetActive(true);
            //IconBillboard();
            PlayerMouseIcon.transform.LookAt(Camera.main.transform.position, -Vector3.down);
            //click to activate projectile
        }
        else
        PlayerMouseIcon.SetActive(false);
        //if (projectileCount2 < projectileCount)
        //{
        //    GameObject projectileInstance = Instantiate(projectilePrefab, startingSpawnLocator.position, transform.rotation);
        //    Projectile projectile = projectileInstance.GetComponent<Projectile>();
        //    if (projectile)
        //    {
        //        projectile.direction = projectileDirection;
        //    }
        //    
        //    projectileCount2++;
        //}
    }
    public void IconBillboard()
    {
        // Make the icon face the camera
        Vector3 lookPos = cam.transform.position - transform.position; // Get direction
        //lookPos.y = 0; // Keep Y-axis fixed
        PlayerMouseIcon.transform.rotation = Quaternion.LookRotation(-lookPos);
    }
}
