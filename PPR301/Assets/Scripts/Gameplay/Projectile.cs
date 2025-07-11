using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public ShootProjectile script;
    public Vector3 direction = Vector3.forward;
    public float moveSpeed = 1f;
    //lifespan
    private float lifespan2;
    // Start is called before the first frame update
    void Start()
    {
        script = GameObject.FindObjectOfType<ShootProjectile>();
        direction = direction.normalized;
        lifespan2 = script.projectileLifeSpan;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(direction * moveSpeed * Time.deltaTime, Space.Self);

        lifespan2 -= Time.deltaTime;
        if(lifespan2 <= 0)
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
            //Destroy(collision.gameObject);
            Destroy(collision.transform.parent.gameObject);
            //script.projectileCount2--;
        }
        else if(collision.CompareTag("Bounce"))
        {
            ChangeHorizontalDirection();
            lifespan2 = script.projectileLifeSpan;
            Debug.Log("bouunce");
        }
        if(collision.CompareTag("Wall"))
        {
            //Debug.Log("Destroy");
            Destroy(gameObject);
            //script.projectileCount2--;
        }
    }

    void ChangeHorizontalDirection()
    {
        direction = new Vector3 (-direction.x, direction.y, direction.z);
    }
}
