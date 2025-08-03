// ==========================================================================
// Meowt of Tune - newNavScript
// Owned and contributed to by: brookcoli (Itch.io) - Aurorafume (GitHub), donkzilla03 (Itch.io) - AliAK03 (GitHub), JaydenFielderTorrens (Itch.io) - Squib5 (GitHub), komorebimoriko (Itch.io), TheRealCrizz (GitHub), LordVGahn (GitHub)
// itch.io listing: https://brookcoli.itch.io/meowt-of-tune - GitHub Repository: https://github.com/Aurorafume/PPR301
// ==========================================================================
//
// WHAT DOES THIS DO:
// This script implements an AI navigation system that blends two distinct
// behaviours: patrolling a set path and actively avoiding a "danger" source
// (the player). It uses a simple "context steering" model to weigh the desire
// to follow the path against the desire to flee from the player, resulting
// in more dynamic and intelligent-looking movement.
//
// Core functionalities include:
// - Standard patrol-point cycling using a NavMeshAgent.
// - A proximity-based avoidance behaviour to move away from the player.
// - Blending the "go to patrol point" vector and the "flee from player"
//   vector into a single, final movement direction.
// - Manually controlling the NavMeshAgent's movement using the blended result,
//   which overrides its default path following.
//
// Dependencies:
// - Must be attached to a GameObject with a NavMeshAgent component.
// - A NavMesh must be baked in the scene for navigation to work.
// - The 'patrolPoints' array and the 'player' transform must be assigned in
//   the Inspector.
//
// ==========================================================================

using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// An AI that patrols a series of points while actively avoiding the player.
/// </summary>
public class newNavScript : MonoBehaviour
{
    [Header("Navigation & Patrol")]
    [Tooltip("An array of transforms representing the points to patrol between.")]
    public Transform[] patrolPoints;
    [Tooltip("The weight of the desire to follow the patrol path.")]
    public float navigationWeight = 1f;

    [Header("Avoidance Behaviour")]
    [Tooltip("The player or other object to be avoided.")]
    public Transform player;
    [Tooltip("The radius within which the AI will start to avoid the danger source.")]
    public float dangerAvoidanceRadius = 5f;
    [Tooltip("The weight of the desire to avoid the danger source.")]
    public float dangerAvoidanceWeight = 3f;

    // --- Private State Variables ---
    private NavMeshAgent agent;
    private int currentPatrolPointIndex = 0;

    /// <summary>
    /// Caches the NavMeshAgent and sets the initial patrol destination.
    /// </summary>
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(patrolPoints[currentPatrolPointIndex].position);
    }

    /// <summary>
    /// The main behaviour loop, called every frame to calculate and apply movement.
    /// </summary>
    void Update()
    {
        // --- Behaviour Blending ---

        // 1. Get the "navigation" desire: a vector pointing towards the current patrol point.
        Vector3 navigationDir = agent.desiredVelocity.normalized * navigationWeight;

        // 2. Calculate the "avoidance" desire: a vector pointing away from the player.
        Vector3 awayFromPlayer = transform.position - player.position;
        float distanceToPlayer = awayFromPlayer.magnitude;
        Vector3 avoidanceDir = Vector3.zero;

        // Only apply avoidance if the player is within the danger radius.
        if (distanceToPlayer < dangerAvoidanceRadius)
        {
            // The strength of the avoidance increases as the player gets closer.
            float avoidStrength = (1f - distanceToPlayer / dangerAvoidanceRadius);
            avoidanceDir = awayFromPlayer.normalized * dangerAvoidanceWeight * avoidStrength;
        }

        // 3. Combine the desires into a single direction.
        Vector3 finalDirection = (navigationDir + avoidanceDir).normalized;
        Vector3 finalVelocity = finalDirection * agent.speed;

        // 4. Manually move the agent using the blended velocity.
        // This overrides the agent's default movement but allows it to continue pathfinding.
        agent.Move(finalVelocity * Time.deltaTime);

        // --- Patrol Point Management ---

        // Check if the agent has reached its current destination.
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            // Cycle to the next patrol point in the array.
            currentPatrolPointIndex = (currentPatrolPointIndex + 1) % patrolPoints.Length;
            agent.SetDestination(patrolPoints[currentPatrolPointIndex].position);
        }
    }
}