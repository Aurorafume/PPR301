// ==========================================================================
// Meowt of Tune - Shoot Projectile
// Owned and contributed to by: brookcoli (Itch.io) - Aurorafume (GitHub), donkzilla03 (Itch.io) - AliAK03 (GitHub), JaydenFielderTorrens (Itch.io) - Squib35 (GitHub), komorebimoriko (Itch.io), TheRealCrizz (GitHub), LordVGahn (GitHub)
// itch.io listing: https://brookcoli.itch.io/meowt-of-tune - GitHub Repository: https://github.com/Aurorafume/PPR301
// ==========================================================================
//
// WHAT DOES THIS DO:
// This script functions as a projectile launcher (e.g. a trumpet or cannon)
// that can be activated by the player. When the player is close enough and
// provides input, this script instantiates and configures a projectile. It
// also manages the number of active projectiles it has fired to prevent spamming.
//
// Core functionalities include:
// - Firing a projectile when the player is nearby and clicks.
// - Limiting the number of concurrently active projectiles from this launcher.
// - Configuring different projectile types (e.g. straight or diagonal).
// - Maintaining a static list of all launchers in the scene.
//
// Dependencies:
// - UnityEngine for core and physics functionality.
// - A Projectile prefab to be instantiated.
// - A reference to the player's Transform to check for proximity.
//
// ==========================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines the different trajectory types for projectiles.
/// </summary>
public enum ProjectileType
{
    Straight,
    Diagonal
}

/// <summary>
/// Manages the firing of projectiles when the player interacts with it.
/// </summary>
public class ShootProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [Tooltip("The GameObject prefab for the projectile to be fired.")]
    public GameObject projectilePrefab;
    [Tooltip("The maximum number of projectiles allowed from this launcher at one time.")]
    public int projectileCount;
    [Tooltip("The Transform where the projectile will be spawned.")]
    public Transform startingSpawnLocator;
    [Tooltip("The direction the projectile will travel (used for specific projectile types).")]
    public Vector3 projectileDirection = Vector3.forward;
    [Tooltip("The lifespan in seconds for projectiles fired from this launcher.")]
    public float projectileLifeSpan;
    [Tooltip("The type of projectile this launcher fires.")]
    public ProjectileType projectileType;

    [Header("Player Interaction")]
    [Tooltip("A reference to the player's Transform to check for distance.")]
    public Transform player;
    [Tooltip("The distance between this launcher and the player.")]
    public float dist;

    public GameObject lingeringLight;
    public SoundEffects soundEffects;

    //public AudioSource shootTrumpet;

    // A list of active projectiles fired by this specific launcher.
    private List<GameObject> projectileList = new List<GameObject>();
    // A static list of all ShootProjectile instances in the scene.
    public static List<ShootProjectile> trumpetList = new List<ShootProjectile>();

    void Start()
    {
        soundEffects = GameObject.Find("Sound effects").GetComponent<SoundEffects>();
    }

    /// <summary>
    /// Called when the script instance is being loaded. Adds itself to the static list.
    /// </summary>
    void Awake()
    {
        trumpetList.Add(this);
    }

    /// <summary>
    /// Called when the script instance is being destroyed. Removes itself from the static list.
    /// </summary>
    void OnDestroy()
    {
        trumpetList.Remove(this);
    }
    /// <summary>
    /// Called every frame. Checks for player proximity and input to fire a projectile.
    /// </summary>
    void Update()
    {
        // If the player reference is missing, do nothing.
        if (player == null) return;
        
        // Check the distance between the launcher and the player.
        dist = Vector3.Distance(transform.position, player.position);
        
        // Only allow firing if the player is within a close range.
        if (dist <= 3)
        {
            // Fire on left mouse button click.
            if (Input.GetMouseButtonDown(0))
            {
                // Clean the list of any projectiles that have been destroyed.
                projectileList.RemoveAll(item => item == null);

                // Only fire if the number of active projectiles is below the limit.
                if (projectileList.Count < projectileCount)
                {
                    //play sounds
                    soundEffects.trumpetBang.Play();
                    soundEffects.Sax();
                    //create note effect
                    Instantiate(lingeringLight, new Vector3(transform.position.x,transform.position.y,transform.position.z + 1), Quaternion.identity);

                    // Create a new projectile instance.
                    GameObject projectileInstance = Instantiate(projectilePrefab, startingSpawnLocator.position, transform.rotation);
                    Projectile projectile = projectileInstance.GetComponent<Projectile>();
                    projectileList.Add(projectileInstance);

                    // Configure the projectile based on the selected type.
                    switch (projectileType)
                    {
                        case ProjectileType.Straight:
                            // No special configuration needed for straight projectiles.
                            break;
                        case ProjectileType.Diagonal:
                            // Set the direction for diagonal projectiles.
                            if (projectile != null)
                            {
                                projectile.direction = projectileDirection;
                            }
                            break;
                    }
                }
            }
        }
    }
}