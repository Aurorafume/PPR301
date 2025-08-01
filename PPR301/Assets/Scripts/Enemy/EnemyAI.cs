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

using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Navigation & Targeting")]
    [Tooltip("NavMeshAgent used for pathfinding.")]
    public NavMeshAgent agent;
    [Tooltip("Distance at which the enemy will become aggressive.")]
    public int idleAggroDistance = 10;
    [Tooltip("Distance between this enemy and the player.")]
    public int chaseAggroDistance = 20;
    [Tooltip("Distance between this enemy and the player.")]
    public float distanceFromPlayer;
    [Tooltip("Whether the enemy is currently chasing the player.")]
    public bool chasing;
    private GameObject playerToChase;

    [Header("Respawn & Fade Settings")]
    [Tooltip("Time remaining until the enemy despawns after losing the player.")]
    private float despawnTimer;
    [Tooltip("Time (in seconds) to wait before despawning after losing the player.")]
    public float timeUntilDespawn = 5f;
    [Tooltip("Whether the enemy is currently fading out.")]
    public bool fading;
    [Tooltip("Time (in seconds) for the enemy to fully fade out.")]
    public float fadeOutTime = 1.5f;
    [Tooltip("Visual fade strength for the enemy's sprite (0 = invisible, 1 = opaque).")]
    public float fadeStrength = 100f;

    [Header("Components & References")]
    [Tooltip("Animator component controlling the enemy's animations.")]
    public Animator anim;
    [Tooltip("Reference to the NoiseBar used to show noise level.")]
    private NoiseBar noiseBar;
    [Tooltip("Reference to the States manager for global flags.")]
    public States states;
    [Tooltip("Reference to the player GameObject.")]
    public GameObject player;

    private bool despawning;
    private int aggroDistance;

    void Start()
    {
        // Initialise references
        states = FindObjectOfType<States>();
        player = GameObject.Find("Player");
        playerToChase = GameObject.Find("Player");
        noiseBar = FindObjectOfType<NoiseBar>();
        anim = GetComponent<Animator>();

        // --- START DEBUG CHECKS ---
        Debug.Log("--- EnemyAI Start() Initialising ---");
        states = FindObjectOfType<States>();
        if (states == null) { Debug.LogError("EnemyAI: Could not find 'States' object in scene!"); return; }
        else { Debug.Log("EnemyAI: Found 'States' object."); }

        player = GameObject.Find("Player");
        if (player == null) { Debug.LogError("EnemyAI: Could not find GameObject named 'Player'!"); return; }
        else { Debug.Log("EnemyAI: Found 'Player' object by name."); }

        playerToChase = GameObject.Find("Player");
        // No need to check playerToChase again, but good practice to be robust
        if (playerToChase == null) { Debug.LogError("EnemyAI: Could not find 'playerToChase' object!"); return; }

        noiseBar = FindObjectOfType<NoiseBar>();
        if (noiseBar == null) { Debug.LogError("EnemyAI: Could not find 'NoiseBar' object in scene!"); return; }
        else { Debug.Log("EnemyAI: Found 'NoiseBar' object."); }
        // --- END DEBUG CHECKS ---

        // Set the enemy to walk on defined area masks
        agent.areaMask = NavMesh.GetAreaFromName("Walkable") | NavMesh.GetAreaFromName("PhaseWalkable");

        aggroDistance = chaseAggroDistance;
        despawnTimer = timeUntilDespawn;
    }

    void Update()
    {
        HandleWalkAnimation();

        // Main behavior loop
        if (!despawning)
        {
            DetermineIfChasing();
            HandleChasing();
            HandleIdling();
            HandleCatchPlayer();
        }
        else
        {
            Despawn();
        }
    }

    /// <summary>
    /// Controls enemy chasing logic based on player location and aggro state.
    /// </summary>
    public void HandleChasing()
    {
        if (chasing)
        {
            Vector3 playerLocation = playerToChase.transform.position;
            agent.SetDestination(playerLocation);  // Move toward player
            aggroDistance = chaseAggroDistance;

            if (noiseBar != null)
            {
                noiseBar.ForceChaseVisuals(true);   // Enable visual alert
            }
        }
    }

    void HandleIdling()
    {
        // If was chasing, but player is hiding, stop chasing
        if (chasing && states.playerIsHiding)
        {
            StopChasing();
        }

        // If not chasing, handle despawn timer
        if (!chasing)
        {
            HandleDespawnTimer();
        }
    }

    void StopChasing()
    {
        chasing = false;
        agent.SetDestination(transform.position);  // Stop moving
        despawnTimer = timeUntilDespawn;
        aggroDistance = idleAggroDistance;
    }

    void HandleDespawnTimer()
    {
        despawnTimer -= Time.deltaTime;

        if (despawnTimer <= 0)
        {
            despawning = true;
        }
    }

    /// <summary>
    /// Plays or stops walking animation depending on velocity.
    /// </summary>
    public void HandleWalkAnimation()
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
    public void DetermineIfChasing()
    {
        if (states.playerIsHiding) return;

        distanceFromPlayer = Vector3.Distance(transform.position, playerToChase.transform.position);

        chasing = distanceFromPlayer <= aggroDistance;
    }

    /// <summary>
    /// Checks if the enemy is close enough to the player to trigger an interaction.
    /// If the player is hiding, enemy despawns. Otherwise, game over.
    /// </summary>
    public void HandleCatchPlayer()
    {
        if (distanceFromPlayer <= 2f && !states.playerIsHiding)
        {
            if (noiseBar != null)
            {
                noiseBar.StopChase();
                noiseBar.ForceChaseVisuals(false);
            }
            
            states.gameOver = true;  // Player caught
            NoiseHandler.NotifyEnemyDespawned();
        }
    }

    /// <summary>
    /// Gradually fades enemy sprite out when despawning.
    /// Also triggers global enemy despawn notification.
    /// </summary>
    void Despawn()
    {
        // Fade enemy or some other visual effect
        if (noiseBar != null)
        {
            noiseBar.ForceChaseVisuals(false);
        }

        NoiseHandler.NotifyEnemyDespawned();
    }
}
