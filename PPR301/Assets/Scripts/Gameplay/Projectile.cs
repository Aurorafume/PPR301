using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public ShootProjectile script;
    public Vector3 direction = Vector3.forward;
    public float moveSpeed = 1f;
    public bool vertical;
    // Start is called before the first frame update
    void Start()
    {
        script = GameObject.FindObjectOfType<ShootProjectile>();

        direction = direction.normalized;
    }

    // Update is called once per frame
    void Update()
    {
        if(!vertical)
        {
            transform.Translate(direction * moveSpeed * Time.deltaTime, Space.Self);
            // transform.position += new Vector3(direction.x, 
            //                                   direction.y, 
            //                                   direction.z) * moveSpeed * Time.deltaTime;
        }
        else
        {
            transform.position += new Vector3(direction.x, 
                                              direction.y, 
                                              -direction.z) * moveSpeed * Time.deltaTime;
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
        if(collision.CompareTag("Destroyable wall"))
        {
            Destroy(gameObject);
            Destroy(collision.gameObject);
            script.projectileCount2--;
        }
        else if(collision.CompareTag("Bounce"))
        {
            ChangeHorizontalDirection();
        }
    }

    void ChangeHorizontalDirection()
    {
        direction = new Vector3 (-direction.x, direction.y, direction.z);
    }
}
