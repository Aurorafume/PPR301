// ==========================================================================
// Meowt of Tune - Enemy AI
// Owned and contributed to by: brookcoli (Itch.io) - Aurorafume (GitHub), donkzilla03 (Itch.io) - AliAK03 (GitHub), JaydenFielderTorrens (Itch.io) - Squib35 (GitHub), komorebimoriko (Itch.io), TheRealCrizz (GitHub), LordVGahn (GitHub)
// itch.io listing: https://brookcoli.itch.io/meowt-of-tune - GitHub Repository: https://github.com/Aurorafume/PPR301
// ==========================================================================
//
// WHAT DOES THIS DO:
// This script defines the behavior for the main enemy AI. It manages the
// enemy's entire lifecycle, from spawning with a fade-in effect to despawning
// when the player escapes. The AI uses a NavMeshAgent for pathfinding to
// pursue the player.
//
// Core functionalities include:
// - State-driven behavior for spawning, chasing, idling, and despawning.
// - Pathfinding using Unity's NavMeshAgent system.
// - Distance-based detection to initiate and maintain a chase.
// - Dynamic animation control based on movement speed.
// - Fade-in and fade-out effects managed via material alpha properties.
// - Integration with game state systems (States, NoiseBar) to handle player
//   hiding, game over conditions, and UI feedback.
// - Particle effects for spawning and despawning events.
//
// Dependencies:
// - UnityEngine.AI for NavMeshAgent pathfinding.
// - Animator, Renderer, and ParticleSystem for visual effects.
// - Custom scripts: States, NoiseBar, and NoiseHandler for core game logic.
// - A scene with a baked NavMesh and a GameObject tagged "Player".
// - NavMesh Areas named "Walkable" and "PhaseWalkable".
//
// ==========================================================================

using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Navigation & Targeting")]
    [Tooltip("NavMeshAgent used for pathfinding.")]
    public NavMeshAgent agent;
    [Tooltip("Distance at which the enemy will become aggressive when idle.")]
    public int idleAggroDistance = 10;
    [Tooltip("Distance at which the enemy will maintain a chase.")]
    public int chaseAggroDistance = 20;
    private float distanceFromPlayer;       // Current distance between this enemy and the player.
    private bool chasing;                   // Tracks if the enemy is actively chasing the player.
    private GameObject playerToChase;       // A direct reference to the player's GameObject for targeting.

    [Header("Respawn & Fade Settings")]
    [Tooltip("Time (in seconds) to wait before despawning after losing the player.")]
    public float timeUntilDespawn = 5f;
    [Tooltip("Time taken to fade out the enemy.")]
    public float fadeOutTime = 1.5f;
    [Tooltip("Time taken to fade in the enemy.")]
    public float fadeInTime = 1.5f;
    private float fadeOutSpeed;             // Calculated speed for the fade-out effect.
    private float fadeInSpeed;              // Calculated speed for the fade-in effect.
    private float despawnTimer;             // Countdown timer for despawning when idle.

    [Header("Effects & Visuals")]
    [Tooltip("Transform defining the position for particle effects.")]
    public Transform effectLocator;
    [Tooltip("Particle effect played when the enemy spawns in.")]
    public ParticleSystem spawnEffect;
    [Tooltip("Particle effect played when the enemy despawns.")]
    public ParticleSystem despawnEffect;

    [Header("Components & References")]
    [Tooltip("Animator component controlling the enemy's animations.")]
    public Animator anim;
    [Tooltip("Reference to the player GameObject.")]
    public GameObject player;
    [Tooltip("Renderer component used for fade effects.")]
    public Renderer enemyRenderer;
    private NoiseBar noiseBar;              // Reference to the NoiseBar for UI feedback.
    private States states;                  // Reference to the States manager for global flags.
    private bool spawning;                  // State flag for the initial spawn-in sequence.
    private bool despawning;                // State flag for the despawn sequence.
    private int aggroDistance;              // The current aggro distance, which changes based on state.
    private Material[] enemyMaterials;      // Array of materials for the fade effect.
    private float enemyAlpha;               // The current alpha (transparency) value of the enemy materials.

    /// <summary>
    /// Initialises components, references, and state variables at the start.
    /// </summary>
    void Start()
    {
        // Cache component and object references for performance.
        states = FindObjectOfType<States>();
        player = GameObject.Find("Player");
        playerToChase = GameObject.Find("Player"); // Target the player object
        noiseBar = FindObjectOfType<NoiseBar>();
        anim = GetComponent<Animator>();

        // Configure the NavMeshAgent to use specific walkable areas.
        agent.areaMask = NavMesh.GetAreaFromName("Walkable") | NavMesh.GetAreaFromName("PhaseWalkable");

        // Initialise state variables and calculate fade speeds.
        aggroDistance = chaseAggroDistance;
        despawnTimer = timeUntilDespawn;
        enemyMaterials = enemyRenderer.materials;
        fadeOutSpeed = 1f / fadeOutTime;
        fadeInSpeed = 1f / fadeInTime;
        enemyAlpha = 0f; // Start fully transparent

        // Set the initial alpha of all materials to transparent for the fade-in.
        foreach (Material mat in enemyMaterials)
        {
            mat.SetFloat("_Alpha", enemyAlpha);
        }

        // Trigger the spawn-in sequence and its visual effect.
        spawning = true;
        CreateSpawnEffect();
    }

    /// <summary>
    /// Instantiates and configures the spawn particle system.
    /// </summary>
    void CreateSpawnEffect()
    {
        if (spawnEffect != null)
        {
            ParticleSystem effect = Instantiate(spawnEffect, effectLocator.position, Quaternion.identity);
            effect.transform.SetParent(effectLocator);
            ParticleSystem.MainModule main = effect.main;
            main.duration = fadeInTime; // Sync particle duration with fade-in time
            effect.Play();
        }
    }

    /// <summary>
    /// Main update loop, called once per frame. Directs the enemy's state.
    /// </summary>
    void Update()
    {
        // Always update animations based on movement.
        HandleAnimations();

        // If spawning, handle the fade-in and skip other logic.
        if (spawning)
        {
            SpawnIn();
            return;
        }

        // If currently despawning, handle the fade-out process.
        if (despawning)
        {
            Despawn();
        }
        // Otherwise, run the main AI behavior loop.
        else
        {
            DetermineIfChasing();
            HandleChasing();
            HandleIdling();
            HandleCatchPlayer();
        }
    }
    
    /// <summary>
    /// Handles the fade-in effect and transitions the state from spawning to active.
    /// </summary>
    void SpawnIn()
    {
        // Increase alpha over time to fade in.
        enemyAlpha += fadeInSpeed * Time.deltaTime;
        enemyAlpha = Mathf.Clamp01(enemyAlpha); // Keep alpha between 0 and 1

        // Apply the new alpha value to all materials.
        foreach (Material mat in enemyMaterials)
        {
            mat.SetFloat("_Alpha", enemyAlpha);
        }

        // Once fully visible, exit the spawning state.
        if (enemyAlpha >= 1f)
        {
            spawning = false;
        }
    }

    /// <summary>
    /// Controls enemy chasing logic based on player location and aggro state.
    /// </summary>
    public void HandleChasing()
    {
        if (chasing)
        {
            // Set the NavMeshAgent's destination to the player's current location.
            Vector3 playerLocation = playerToChase.transform.position;
            agent.SetDestination(playerLocation);
            aggroDistance = chaseAggroDistance; // Use the larger chase aggro distance

            // Update the noise bar to show the chase is active.
            if (noiseBar != null)
            {
                noiseBar.ForceChaseVisuals(true);
            }
        }
    }

    /// <summary>
    /// Manages behavior when not actively chasing, such as when the player hides or is out of range.
    /// </summary>
    void HandleIdling()
    {
        // If the enemy was chasing but the player is now hiding, stop the chase.
        if (chasing && states.playerIsHiding)
        {
            StopChasing();
        }

        // If not chasing for any reason, start the countdown to despawn.
        if (!chasing)
        {
            HandleDespawnTimer();
        }
    }

    /// <summary>
    /// Resets the enemy's state from chasing to idle.
    /// </summary>
    void StopChasing()
    {
        chasing = false;
        agent.SetDestination(transform.position); // Stop moving
        despawnTimer = timeUntilDespawn;          // Reset despawn timer
        aggroDistance = idleAggroDistance;        // Revert to smaller idle aggro distance
    }

    /// <summary>
    /// Counts down the despawn timer and initiates the despawn sequence when it reaches zero.
    /// </summary>
    void HandleDespawnTimer()
    {
        despawnTimer -= Time.deltaTime;

        // When the timer runs out, begin despawning.
        if (despawnTimer <= 0)
        {
            // Turn off the chase visuals on the noise bar.
            if (noiseBar != null)
            {
                noiseBar.ForceChaseVisuals(false);
            }

            // Set the despawning flag and create the visual effect.
            despawning = true;
            CreateDespawnEffect();
        }
    }

    /// <summary>
    /// Instantiates and plays the despawn particle effect.
    /// </summary>
    void CreateDespawnEffect()
    {
        if (despawnEffect != null)
        {
            ParticleSystem effect = Instantiate(despawnEffect, effectLocator.position, Quaternion.identity);
            ParticleSystem.MainModule main = effect.main;
            main.duration = fadeOutTime; // Sync particle duration with fade-out time
            effect.Play();
        }
    }

    /// <summary>
    /// Toggles the walking animation based on the NavMeshAgent's velocity.
    /// </summary>
    public void HandleAnimations()
    {
        // If the agent is moving, play the walking animation.
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
    /// Checks the distance to the player and toggles the chasing state accordingly.
    /// </summary>
    public void DetermineIfChasing()
    {
        // Do not start a chase if the player is currently hidden.
        if (states.playerIsHiding) return;

        distanceFromPlayer = Vector3.Distance(transform.position, playerToChase.transform.position);

        // Start or continue chasing if the player is within the current aggro distance.
        chasing = distanceFromPlayer <= aggroDistance;
    }

    /// <summary>
    /// Checks if the enemy is close enough to catch the player and triggers the game over state.
    /// </summary>
    public void HandleCatchPlayer()
    {
        // Check if the enemy is very close and the player is not hiding.
        if (distanceFromPlayer <= 2f && !states.playerIsHiding)
        {
            // Reset the noise bar visuals.
            if (noiseBar != null)
            {
                noiseBar.StopChase();
                noiseBar.ForceChaseVisuals(false);
            }
            
            // Player is caught, trigger game over.
            states.gameOver = true;
            NoiseHandler.NotifyEnemyDespawned(); // Notify that this enemy is no longer active.
        }
    }

    /// <summary>
    /// Gradually fades the enemy out and notifies the system upon completion.
    /// </summary>
    void Despawn()
    {
        // Decrease alpha over time to fade out.
        enemyAlpha -= fadeOutSpeed * Time.deltaTime;
        enemyAlpha = Mathf.Clamp01(enemyAlpha);

        // Apply the new alpha value to all materials.
        foreach (Material mat in enemyMaterials)
        {
            mat.SetFloat("_Alpha", enemyAlpha);
        }

        // Once fully faded, notify the system and destroy the GameObject.
        if (enemyAlpha <= 0)
        {
            NoiseHandler.NotifyEnemyDespawned();
            Destroy(gameObject); // Remove the enemy from the scene.
        }
    }
}