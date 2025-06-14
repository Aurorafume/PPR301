using UnityEngine;
using UnityEngine.AI;

public class newNavScript : MonoBehaviour
{
    public Transform[] patrolPoints;
    public Transform player;
    public float dangerAvoidanceRadius = 5f;
    public float navigationWeight = 1f;
    public float dangerAvoidanceWeight = 3f;

    private NavMeshAgent agent;
    private int currentPatrolPointIndex = 0;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(patrolPoints[currentPatrolPointIndex].position);
    }

    void Update()
    {
        // Get normal navigation velocity
        Vector3 navigationDir = agent.desiredVelocity.normalized * navigationWeight;

        // Calculate avoidance
        Vector3 awayFromPlayer = transform.position - player.position;
        float distanceToPlayer = awayFromPlayer.magnitude;
        Vector3 avoidanceDir = Vector3.zero;

        if (distanceToPlayer < dangerAvoidanceRadius)
        {
            float avoidStrength = (1f - distanceToPlayer / dangerAvoidanceRadius);
            avoidanceDir = awayFromPlayer.normalized * dangerAvoidanceWeight * avoidStrength;
        }

        // Combine navigation + avoidance
        Vector3 finalVelocity = (navigationDir + avoidanceDir).normalized * agent.speed;

        agent.Move(finalVelocity * Time.deltaTime);

        // Check patrol point
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            currentPatrolPointIndex = (currentPatrolPointIndex + 1) % patrolPoints.Length;
            agent.SetDestination(patrolPoints[currentPatrolPointIndex].position);
        }
    }
}
