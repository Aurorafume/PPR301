using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Navigation & Targeting")]
    [Tooltip("NavMeshAgent used for pathfinding.")]
    public NavMeshAgent agent;

    [Tooltip("Current position of the player.")]
    public Vector3 playerLocation;

    [Tooltip("Distance at which the enemy will become aggressive.")]
    public int aggroDistance;

    [Tooltip("Distance between this enemy and the player.")]
    public float distanceFromPlayer;

    [Tooltip("Whether the enemy is currently chasing the player.")]
    public bool chasing;

    [Tooltip("Whether the enemy is currently angry (actively hunting).")]
    public bool angry;

    private GameObject playerToChase;

    [Header("Respawn & Fade Settings")]
    [Tooltip("Time remaining until the enemy despawns after losing the player.")]
    private float whenToRespawn;

    [Tooltip("Time (in seconds) to wait before despawning after chase ends.")]
    public float respawnTimer;

    [Tooltip("Whether the enemy is currently fading out.")]
    public bool fading;

    [Tooltip("Visual fade strength for the enemy's sprite (0 = invisible, 1 = opaque).")]
    public float fadeStrength = 100f;

    private float turnNum;
    private bool right;

    [Header("Components & References")]
    [Tooltip("Animator component controlling the enemy's animations.")]
    public Animator anim;

    [Tooltip("SpriteRenderer used to control visual effects like fading.")]
    private SpriteRenderer spriteRenderer;

    [Tooltip("Reference to the NoiseBar used to show noise level.")]
    private NoiseBar noiseBar;

    [Tooltip("Reference to the States manager for global flags.")]
    public States states;

    [Tooltip("Reference to the player GameObject.")]
    public GameObject player;
    
    void Start()
    {
        // Get the States script
        states = FindObjectOfType<States>();
        player = GameObject.Find("Player");

        GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");

        // Set the player to chase as anything with the tag player
        playerToChase = GameObject.Find("Player");

        spriteRenderer = GetComponent<SpriteRenderer>();

        noiseBar = FindObjectOfType<NoiseBar>();

        agent.areaMask = NavMesh.GetAreaFromName("Walkable") | NavMesh.GetAreaFromName("PhaseWalkable");
        anim = GetComponent<Animator>();
    }

    void Update()
    {   
        updatePlayerLocation();
        // billboard();
        checkAggroDistance();
        enemyChase();
        touchPlayer();
        FadeTo();
    }
    public void enemyChase()
    {
        // Check if the player is hiding and set the aggro distance accordingly
        if(chasing)
        {   
            agent.SetDestination(playerLocation);
            walkAnimation();
            whenToRespawn = respawnTimer;
            angry = true;

            if (noiseBar != null)
            {
                noiseBar.ForceChaseVisuals(true);
            }
        }
        else
        {
            whenToRespawn -= Time.deltaTime;
            agent.SetDestination(transform.position);
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);

            if (whenToRespawn <= 0)
            {
                fading = true;
                Debug.Log("Enemy Despawned");
                aggroDistance = 10;

                if (noiseBar != null)
                {
                    noiseBar.ForceChaseVisuals(false);
                }
            }
            angry = false;
        }
    }

public void walkAnimation()
{   
    // Set the walking animation based on the agent's velocity
    if (agent.velocity.magnitude > 0.1f)
    {
        anim.SetBool("isWalking", true);
    }
    else
    {
        anim.SetBool("isWalking", false);
    }
}
    public void checkAggroDistance()
    {
        // Checks distance between player and enemy
        distanceFromPlayer = Vector3.Distance(transform.position, playerToChase.transform.position);
        if(distanceFromPlayer <= aggroDistance)
        {
            chasing = true;
        }
        else
        {
            chasing = false;
        }
        // Longer follow distance when following
        if(angry)
        {
            aggroDistance = 20;
        }
    }

    public void updatePlayerLocation()
    {
        // Set playerLocation to player's current location
        playerLocation = playerToChase.transform.position;
    }

    public void touchPlayer()
    {   
        // Check if player is touching enemy
        GameObject hidingSpot = GameObject.FindGameObjectWithTag("Hiding Spot");

        if (distanceFromPlayer <= 2f)
        {
            Debug.Log("Touching Player");

            if (states.playerIsHiding)
            {
                Debug.Log("Player is hiding. Enemy will despawn.");
                fading = true;
                noiseBar.StopChase();

                // Stop chase visuals when despawning due to hiding
                if (noiseBar != null)
                {
                    noiseBar.ForceChaseVisuals(false);
                }

                FadeTo();
            }
            else
            {
                states.gameOver = true;
            }
        }
    }

    public void billboard()
    {
        // Make the enemy face the camera
        Vector3 lookPos = Camera.main.transform.position - transform.position; // Get direction
        lookPos.y = 0; // Keep Y-axis fixed
        transform.rotation = Quaternion.LookRotation(-lookPos);
    }

    public void FadeTo()
    {
        // float fadeStrength = 100f;
        if(fading == true)
        {
            fadeStrength -= Time.deltaTime / 1.5f;
        }
        else
        {
            fadeStrength += Time.deltaTime / 1.5f;
        }

        fadeStrength = Mathf.Clamp(fadeStrength, 0, 1);
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, fadeStrength);

        if (fadeStrength <= 0.01f && fading) // Small margin for float accuracy
        {
            fading = false;
            NoiseHandler.NotifyEnemyDespawned();
        }
    }
}