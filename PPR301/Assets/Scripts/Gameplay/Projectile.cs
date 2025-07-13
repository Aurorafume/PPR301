using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public ShootProjectile script;
    public Vector3 direction = Vector3.forward;
    public float moveSpeed = 1f;
    //lifespan
    public float lifespan2;
    
    [Header("Effects")]
    public GameObject lingeringLight;

    void Start()
    {
    ShootProjectile closest = null;
    float closestDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach (ShootProjectile sp in GameObject.FindObjectsOfType<ShootProjectile>())
        {
            float distance = Vector3.Distance(sp.transform.position, currentPosition);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = sp;
                script = sp;
            }
        }

        
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
            Explode();
        }
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Destroyable wall"))
        {
            Destroy(collision.transform.parent.gameObject);
            
            Explode();
        }
        else if (collision.CompareTag("Bounce"))
        {
            ChangeHorizontalDirection();
            lifespan2 = script.projectileLifeSpan;
            Debug.Log("bouunce");
        }
        if(collision.CompareTag("Wall"))
        {
            Explode();
        }
    }

    void Explode()
    {
        if (lingeringLight != null)
        {
            Instantiate(lingeringLight, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    void ChangeHorizontalDirection()
    {
        direction = new Vector3 (-direction.x, direction.y, direction.z);
    }
}
