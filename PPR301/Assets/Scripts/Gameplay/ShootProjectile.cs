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

    public void OnInteraction()
    {
        if (projectileCount2 < projectileCount)
        {
            GameObject projectileInstance = Instantiate(projectilePrefab, startingSpawnLocator.position, transform.rotation);
            Projectile projectile = projectileInstance.GetComponent<Projectile>();
            if (projectile)
            {
                projectile.direction = projectileDirection;
            }
            
            projectileCount2++;
        }
    }
}
