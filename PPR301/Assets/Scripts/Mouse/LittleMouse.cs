// ==========================================================================
// Meowt of Tune - Little Mouse AI
// Owned and contributed to by: brookcoli (Itch.io) - Aurorafume (GitHub), donkzilla03 (Itch.io) - AliAK03 (GitHub), JaydenFielderTorrens (Itch.io) - Squib35 (GitHub), komorebimoriko (Itch.io), TheRealCrizz (GitHub), LordVGahn (GitHub)
// itch.io listing: https://brookcoli.itch.io/meowt-of-tune - GitHub Repository: https://github.com/Aurorafume/PPR301
// ==========================================================================
//
// WHAT DOES THIS DO:
// This script controls the behavior of a small, ambient mouse creature. It
// implements a simple AI state machine that causes the mouse to either patrol
// between a set of predefined points or run away if the player gets too close.
//
// Core functionalities include:
// - A state machine with two states: Patroling and RunningAway.
// - Pathfinding using a NavMeshAgent.
// - Randomly selects points from a list to patrol to.
// - Player proximity detection to trigger a flee response.
// - Calculates a valid NavMesh destination away from the player to escape.
// - A timer-based cooldown before the mouse calms down and returns to patrolling.
//
// Dependencies:
// - Requires a NavMeshAgent component on the same GameObject.
// - A 'player' GameObject must be assigned in the Inspector.
// - An array of Transform 'patrolPoints' must be assigned in the Inspector.
// - A baked NavMesh must exist in the scene for movement.
// - Optionally interacts with trigger colliders on GameObjects tagged "Portal".
//
// ==========================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Controls the AI for a simple mouse creature that patrols an area and flees from the player.
/// </summary>
public class LittleMouse : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Reference to the player GameObject to determine proximity.")]
    public GameObject player;

    [Header("Patrol Behavior")]
    [Tooltip("An array of transforms representing points the mouse can patrol between.")]
    public Transform[] patrolPoints;
    [Tooltip("The time (in seconds) the mouse will wait before returning to patrol after fleeing.")]
    public float mousePatrolWait;

    // --- Private State Variables ---
    private Vector3 playerLocation;             // The player's current position, updated every frame.
    private NavMeshAgent agent;                 // The cached NavMeshAgent component for movement.
    private int currentPatrolPointIndex;        // The index in the patrolPoints array of the current destination.
    private MouseStates mouseState;             // The current state of the mouse's AI.

    // Note: The following variables are declared but not used in the current implementation.
    // They may be for a planned feature where the mouse waits at each patrol point.
    public float maxWaitingTime;
    public float currentWaitingTime;
    
    /// <summary>
    /// Defines the possible states for the mouse AI.
    /// </summary>
    public enum MouseStates
    {
        Patroling,
        RunningAway
    }
    
    /// <summary>
    /// Initialises the NavMeshAgent and sets initial state values.
    /// </summary>
    void Start()
    {
        // Cache the NavMeshAgent component for performance.
        agent = GetComponent<NavMeshAgent>();
        
        // Initialise state variables.
        currentPatrolPointIndex = 0;
        maxWaitingTime = 0;
        currentWaitingTime = 0;
    }

    /// <summary>
    /// The main update loop, called once per frame to drive the AI's state.
    /// </summary>
    void Update()
    {
        // Keep track of the player's location.
        playerLocation = player.transform.position;
        
        // Determine if the mouse's state should change based on player distance.
        DistanceBehaviour();
        
        // Execute the logic for the current state.
        switch(mouseState)
        {
            case MouseStates.Patroling:
                RatPatrol();
                break;
            case MouseStates.RunningAway:
                RatRunaway();
                break;
            default:
                Debug.Log("Unknown State");
                break;
        }
    }

    /// <summary>
    /// Checks the distance to the player and switches the AI state if necessary.
    /// </summary>
    void DistanceBehaviour()
    {
        // If the player is too close, switch to the RunningAway state.
        if(Vector3.Distance(gameObject.transform.position, playerLocation) < 5f)
        {
            mouseState = MouseStates.RunningAway;
            mousePatrolWait = 5f; // Set the cooldown timer for returning to patrol.
            GoToNextPoint(); // Immediately pick a new point to help with fleeing logic.
        }
        else
        {
            // If the player is far away, start the cooldown to return to patrolling.
            WaitAndPatrol();
        }
    }

    /// <summary>
    /// Manages the cooldown timer that transitions the mouse back to the Patroling state.
    /// </summary>
    void WaitAndPatrol()
    {
        mousePatrolWait -= Time.deltaTime;
        if(mousePatrolWait <= 0)
        {
            mouseState = MouseStates.Patroling;
        }
    }

    /// <summary>
    /// Handles the logic for fleeing from the player.
    /// </summary>
    void RatRunaway()
    {
        // Calculate a direction vector pointing directly away from the player.
        Vector3 dirAway = (transform.position - playerLocation).normalized;
        
        // Find a potential target position 5 units away in the flee direction.
        Vector3 rawTarget = transform.position + dirAway * 5f;
        
        NavMeshHit hit;
        // Check if the raw target position is on or near the NavMesh.
        if (NavMesh.SamplePosition(rawTarget, out hit, 5f, NavMesh.AllAreas))
        {
            // If a valid point is found, set it as the destination.
            agent.SetDestination(hit.position);
        }
        else
        {
            // If no valid point is found, log a warning and stop moving to avoid errors.
            Debug.LogWarning("No valid NavMesh point found for flee near: " + rawTarget);
            agent.ResetPath();
        }
    }

    /// <summary>
    /// Handles the logic for patrolling between points.
    /// </summary>
    void RatPatrol()
    {
        // Set the destination to the current patrol point.
        agent.SetDestination(patrolPoints[currentPatrolPointIndex].position);
        
        // If the mouse has arrived at the destination, pick a new one.
        if(agent.remainingDistance < 0.5f)
        {
            GoToNextPoint();
        }
    }

    /// <summary>
    /// Selects a new, random patrol point from the list.
    /// </summary>
    void GoToNextPoint()
    {
        if(patrolPoints.Length != 0)
        {
            // Choose a random index from the patrol points array.
            currentPatrolPointIndex = Random.Range(0, patrolPoints.Length);
        }
    }

    /// <summary>
    /// Handles trigger events, specifically for interacting with portals.
    /// </summary>
    void OnTriggerEnter(Collider collider)
    {
        // If the mouse enters a trigger tagged "Portal"...
        if(collider.gameObject.CompareTag("Portal"))
        {
            // ...immediately reset the patrol wait timer, causing it to calm down faster.
            Debug.Log("Mouse entered portal, resetting patrol wait time.");
            mousePatrolWait = 0;
        }
    }
}