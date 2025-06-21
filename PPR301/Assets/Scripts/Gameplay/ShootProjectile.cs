using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public enum ProjectileType
    {
        Straight,
        Diagonal
    }

public class ShootProjectile : MonoBehaviour
{
    public GameObject projectilePrefab;
    public int projectileCount; //how many projectiles should be out at once
    public Transform startingSpawnLocator;
    public Vector3 projectileDirection = Vector3.forward;
    public float dist;
    public Transform player;
    public GameObject PlayerMouseIcon;
    public ProjectileType projectileType;
    //list of projectiles
    public List<GameObject> projectileList = new List<GameObject>();
    public static List<ShootProjectile> trumpetList = new List<ShootProjectile>();

    void Start()
    {
       //trumpetList.Add(this);
    }

    void Update()
    {
        bool isNearAny = false;
        foreach (ShootProjectile obj in trumpetList)
        {
            float dist = Vector3.Distance(player.position, obj.transform.position);
            if (dist <= 3f)
            {
                isNearAny = true;
                break; // Stop checking after finding one nearby
            }
        }
        if(isNearAny)
        {
            PlayerMouseIcon.SetActive(true);
            //icon faces camera
            PlayerMouseIcon.transform.LookAt(Camera.main.transform.position, Vector3.up);
            PlayerMouseIcon.transform.Rotate(0f, 180f, 0f);
        }
        else
        PlayerMouseIcon.SetActive(false);

        //next to spacific trumpet
        dist = Vector3.Distance(transform.position, player.position);
        if(dist < 3)
        {
            //click to activate projectile
            if(Input.GetMouseButtonDown(0))
            {
                projectileList.RemoveAll(item => item == null);
                if (projectileList.Count < projectileCount)
                {
                    GameObject projectileInstance = Instantiate(projectilePrefab, startingSpawnLocator.position, transform.rotation);
                    Projectile projectile = projectileInstance.GetComponent<Projectile>();
                    projectileList.Add(projectileInstance);
                    switch(projectileType)
                    {
                        case ProjectileType.Straight:
                        Debug.Log("Straight projectile");
                        break;
                        case ProjectileType.Diagonal:
                        Debug.Log("Diagonal projectile");
                        projectile.direction = projectileDirection;
                        break;
                    }
                }
            }
        }
    }
       void Awake()
    {
        trumpetList.Add(this);
    }
    void OnDestroy()
    {
        trumpetList.Remove(this);
    }
}
