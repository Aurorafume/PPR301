// ==========================================================================
// Meowt of Tune - Enemy AI
// Owned and contributed to by: brookcoli (Itch.io) - Aurorafume (GitHub), donkzilla03 (Itch.io) - AliAK03 (GitHub), JaydenFielderTorrens (Itch.io) - Squib35 (GitHub), komorebimoriko (Itch.io), TheRealCrizz (GitHub), LordVGahn (GitHub)
// itch.io listing: https://brookcoli.itch.io/meowt-of-tune - GitHub Repository: https://github.com/Aurorafume/PPR301
// ==========================================================================
//
// WHAT DOES THIS DO:
// This script controls the AI behavior for an enemy character in a Unity game.
// It includes logic for the enemy to detect, chase, and respond to the player's
// presence, as well as fade away when the player escapes or hides.
// 
// Core functionalities include:
// - Pathfinding to follow the player using NavMeshAgent.
// - Distance-based aggro detection and chase initiation.
// - Animation control for walking behavior.
// - Fade-out effect for despawning after chase ends.
// - Game state updates when the player is caught or hides.
// - Integration with a noise meter system to visually indicate chase state.
//
// Dependencies:
// - UnityEngine.AI for navigation.
// - Animator and SpriteRenderer for visual behavior.
// - States and NoiseBar custom scripts for gameplay logic and feedback.
//
// ==========================================================================

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
        // Initialise references
        states = FindObjectOfType<States>();
        player = GameObject.Find("Player");
        playerToChase = GameObject.Find("Player");
        spriteRenderer = GetComponent<SpriteRenderer>();
        noiseBar = FindObjectOfType<NoiseBar>();
        anim = GetComponent<Animator>();

        // Set the enemy to walk on defined area masks
        agent.areaMask = NavMesh.GetAreaFromName("Walkable") | NavMesh.GetAreaFromName("PhaseWalkable");
    }

    void Update()
    {
        // Main behavior loop
        updatePlayerLocation();
        checkAggroDistance();
        enemyChase();
        touchPlayer();
        FadeTo();
    }

    /// <summary>
    /// Controls enemy chasing logic based on player location and aggro state.
    /// </summary>
    public void enemyChase()
    {
        if (chasing)
        {
            agent.SetDestination(playerLocation);  // Move toward player
            walkAnimation();                       // Play walk animation
            whenToRespawn = respawnTimer;          // Reset despawn timer
            angry = true;

            if (noiseBar != null)
            {
                noiseBar.ForceChaseVisuals(true);   // Enable visual alert
            }
        }
        else
        {
            // Countdown to despawn
            whenToRespawn -= Time.deltaTime;

            // Stop moving and reset rotation to upright
            agent.SetDestination(transform.position);
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);

            if (whenToRespawn <= 0)
            {
                fading = true;
                aggroDistance = 10;

                if (noiseBar != null)
                {
                    noiseBar.ForceChaseVisuals(false);
                }
            }

            angry = false;
        }
    }

    /// <summary>
    /// Plays or stops walking animation depending on velocity.
    /// </summary>
    public void walkAnimation()
    {
        if (agent.velocity.magnitude > 0.1f)
        {
            anim.SetBool("isWalking", true);
        }
        else
        {
            anim.SetBool("isWalking", false);
        }
    }

    /// <summary>
    /// Checks the distance from player and toggles chasing state.
    /// </summary>
    public void checkAggroDistance()
    {
        distanceFromPlayer = Vector3.Distance(transform.position, playerToChase.transform.position);

        chasing = (distanceFromPlayer <= aggroDistance);

        // If angry, increase aggro radius to maintain chase longer
        if (angry)
        {
            aggroDistance = 20;
        }
    }

    /// <summary>
    /// Continuously updates the player's position for targeting.
    /// </summary>
    public void updatePlayerLocation()
    {
        playerLocation = playerToChase.transform.position;
    }

    /// <summary>
    /// Checks if the enemy is close enough to the player to trigger an interaction.
    /// If the player is hiding, enemy despawns. Otherwise, game over.
    /// </summary>
    public void touchPlayer()
    {
        GameObject hidingSpot = GameObject.FindGameObjectWithTag("Hiding Spot");

        if (distanceFromPlayer <= 2f)
        {
            if (states.playerIsHiding)
            {
                fading = true;
                noiseBar.StopChase();

                if (noiseBar != null)
                {
                    noiseBar.ForceChaseVisuals(false);
                }

                FadeTo();  // Begin fading out if player is hiding
            }
            else
            {
                states.gameOver = true;  // Player caught
            }
        }
    }

    /// <summary>
    /// Gradually fades enemy sprite out when despawning.
    /// Also triggers global enemy despawn notification.
    /// </summary>
    public void FadeTo()
    {
        // Increase or decrease fade strength depending on fade state
        if (fading)
        {
            fadeStrength -= Time.deltaTime / 1.5f;
        }
        else
        {
            fadeStrength += Time.deltaTime / 1.5f;
        }

        fadeStrength = Mathf.Clamp(fadeStrength, 0, 1);

        spriteRenderer.color = new Color(
            spriteRenderer.color.r,
            spriteRenderer.color.g,
            spriteRenderer.color.b,
            fadeStrength
        );

        // If fully faded, finalise despawn
        if (fadeStrength <= 0.01f && fading)
        {
            fading = false;
            NoiseHandler.NotifyEnemyDespawned();
        }
    }
}
