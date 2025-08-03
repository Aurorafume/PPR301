// ==========================================================================
// Meowt of Tune - Projectile
// Owned and contributed to by: brookcoli (Itch.io) - Aurorafume (GitHub), donkzilla03 (Itch.io) - AliAK03 (GitHub), JaydenFielderTorrens (Itch.io) - Squib35 (GitHub), komorebimoriko (Itch.io), TheRealCrizz (GitHub), LordVGahn (GitHub)
// itch.io listing: https://brookcoli.itch.io/meowt-of-tune - GitHub Repository: https://github.com/Aurorafume/PPR301
// ==========================================================================
//
// WHAT DOES THIS DO:
// This script controls the behavior of a projectile after it has been fired.
// It manages the projectile's movement, its limited lifespan, and its
// interactions with various types of surfaces upon collision.
//
// Core functionalities include:
// - Moving in a straight line at a constant speed.
// - Destroying itself after a set duration (lifespan).
// - Interacting with different objects based on their tags, allowing it to
//   destroy walls, bounce off surfaces, or simply explode on impact.
// - Spawning a visual effect upon destruction.
//
// Dependencies:
// - UnityEngine for core and physics functionality.
// - A trigger Collider and a Rigidbody on this GameObject.
// - A ShootProjectile script in the scene to provide settings.
// - A LingeringLight prefab for the explosion effect.
//
// ==========================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages a projectile's movement, lifespan, and collision interactions.
/// </summary>
public class Projectile : MonoBehaviour
{
    [Header("Behavior")]
    [Tooltip("A reference to the script that fired this projectile.")]
    public ShootProjectile script;
    [Tooltip("The direction the projectile will travel.")]
    public Vector3 direction = Vector3.forward;
    [Tooltip("The speed at which the projectile moves.")]
    public float moveSpeed = 1f;

    [Header("Effects")]
    [Tooltip("The effect to spawn when the projectile explodes.")]
    public GameObject lingeringLight;

    // The remaining time before the projectile destroys itself.
    private float lifespanTimer;

    public AudioSource bounceSound;


    /// <summary>
    /// Called on startup. Finds the closest projectile shooter to inherit its settings.
    /// NOTE: This search can be performance-intensive if many shooters exist.
    /// </summary>
    void Start()
    {
        ShootProjectile closestShooter = null;
        float closestDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        // Find all projectile shooters and identify the closest one.
        foreach (ShootProjectile sp in FindObjectsOfType<ShootProjectile>())
        {
            float distance = Vector3.Distance(sp.transform.position, currentPosition);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestShooter = sp;
            }
        }
        
        script = closestShooter;
        if (script != null)
        {
            lifespanTimer = script.projectileLifeSpan;
        }

        // Ensure the direction vector is a unit vector.
        direction = direction.normalized;
    }

    /// <summary>
    /// Called every frame. Handles movement and the lifespan countdown.
    /// </summary>
    void Update()
    {
        // Move the projectile in its set direction.
        transform.Translate(direction * moveSpeed * Time.deltaTime, Space.Self);

        // Countdown the lifespan and explode if it runs out.
        lifespanTimer -= Time.deltaTime;
        if (lifespanTimer <= 0)
        {
            Explode();
        }
    }

    /// <summary>
    /// Called by Unity when the projectile's trigger collides with another collider.
    /// </summary>
    /// <param name="other">The collider this projectile hit.</param>
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Destroyable wall"))
        {
            // Destroy the wall's parent object (assuming the collider is a child).
            Destroy(other.transform.parent.gameObject);
            Explode();
        }
        else if (other.CompareTag("Bounce"))
        {
            // Reverse direction and reset lifespan upon hitting a bounceable surface.
            ChangeHorizontalDirection();
            if (script != null)
            {
                lifespanTimer = script.projectileLifeSpan;
            }
            bounceSound.Play();
        }
        else if (other.CompareTag("Wall"))
        {
            // Explode upon hitting a standard wall.
            Explode();
        }
    }

    /// <summary>
    /// Spawns a lingering light effect and destroys the projectile.
    /// </summary>
    void Explode()
    {
        if (lingeringLight != null)
        {
            Instantiate(lingeringLight, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }

    /// <summary>
    /// Reverses the horizontal component of the projectile's direction.
    /// </summary>
    void ChangeHorizontalDirection()
    {
        direction = new Vector3(-direction.x, direction.y, direction.z);
    }
}