using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public ShootProjectile script;
    public Vector3 direction = Vector3.forward;
    public float moveSpeed = 1f;
    //lifespan
    private float lifespan = 5;
    // Start is called before the first frame update
    void Start()
    {
        script = GameObject.FindObjectOfType<ShootProjectile>();
        direction = direction.normalized;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(direction * moveSpeed * Time.deltaTime, Space.Self);

        lifespan -= Time.deltaTime;
        if(lifespan <= 0)
        {
            Destroy(gameObject);
            //script.projectileCount2--;
        }
    }

    void OnTriggerEnter(Collider collision)
    {
        if(collision.CompareTag("Destroyable wall"))
        {
            Destroy(gameObject);
            Destroy(collision.gameObject);
            //script.projectileCount2--;
        }
        else if(collision.CompareTag("Bounce"))
        {
            ChangeHorizontalDirection();
            lifespan = 5;
            Debug.Log("bouunce");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void ChangeHorizontalDirection()
    {
        direction = new Vector3 (-direction.x, direction.y, direction.z);
    }
    //void BounceDirection()
    //{
    //    direction = new Vector3 (-direction.x, direction.y, direction.z);
    //}
}
