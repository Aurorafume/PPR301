using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootProjectile : MonoBehaviour
{
    public GameObject projectile;
    private GameObject newProjectile;
    public GameObject player;
    public float detectionRange; //detect player
    public int projectileCount; //how many projectiles should be out at once
    public int projectileCount2;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(transform.position, player.transform.position) < detectionRange)
        {
            if(Input.GetMouseButtonDown(0) && projectileCount2 < projectileCount)
            {
                Debug.Log("Hello");
                newProjectile = Instantiate(projectile, new Vector3(transform.position.x,transform.position.y,transform.position.z), Quaternion.identity);
                projectileCount2++;
            }
        }
    }
}
