using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public ShootProjectile script;
    public int projectileSpeed;
    public int verticalSpeed;
    public bool vertical;
    // Start is called before the first frame update
    void Start()
    {
        script = GameObject.FindObjectOfType<ShootProjectile>();
    }

    // Update is called once per frame
    void Update()
    {
        if(vertical == false)
        {
            transform.position += new Vector3(projectileSpeed,0,verticalSpeed) * Time.deltaTime;
        }
        else
        {
            transform.position += new Vector3(projectileSpeed,0,-verticalSpeed) * Time.deltaTime;
        }
        
    }
    void OnTriggerEnter(Collider collision)
    {
        if(collision.CompareTag("Wall"))
        {
            Debug.Log("Destroy");
            Destroy(gameObject);
            script.projectileCount2--;
        }
        else if(collision.CompareTag("Bounce"))
        {
            if(vertical)
            {
                vertical = false;
            }
            else
            {
                vertical = true;
            }
        }
    }
}
