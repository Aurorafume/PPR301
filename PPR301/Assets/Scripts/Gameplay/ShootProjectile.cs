using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootProjectile : MonoBehaviour
{
    public GameObject projectile;
    public int projectileCount; //how many projectiles should be out at once
    public int projectileCount2;
    public int startingSpawn;

    public void OnInteraction()
    {
        if (projectileCount2 < projectileCount)
        {
            Instantiate(projectile, new Vector3(transform.position.x - startingSpawn ,transform.position.y,transform.position.z), Quaternion.identity);
            projectileCount2++;
        }
    }
}
