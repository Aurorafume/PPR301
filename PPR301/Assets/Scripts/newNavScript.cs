using UnityEngine;
using UnityEngine.AI;

public class newNavScript : MonoBehaviour
{
    public Transform player;
    public Transform[] pathPoints;
    public float fleeDistance = 5f;    // Start fleeing if player closer than this
    public float safeDistance = 3f;    // Minimum distance from player to waypoint
    public float reachThreshold = 0.5f;

    private NavMeshAgent agent;
    private int currentPointIndex = 0;
    private bool isFleeing = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (pathPoints.Length > 0)
        {
            agent.SetDestination(pathPoints[currentPointIndex].position);
        }
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer < fleeDistance)
        {
            isFleeing = true;

            // If close to current waypoint or agent has no path, find next safe point
            if ((!agent.pathPending && agent.remainingDistance < reachThreshold) || !agent.hasPath)
            {
                MoveToNextSafePoint();
            }
        }
        else
        {
            isFleeing = false;
            agent.ResetPath();
        }
    }

    void MoveToNextSafePoint()
    {
        int tries = 0;
        int nextIndex = currentPointIndex;

        do
        {
            nextIndex = (nextIndex + 1) % pathPoints.Length;
            tries++;

            float distToPlayerFromNextPoint = Vector3.Distance(pathPoints[nextIndex].position, player.position);

            // If the next point is far enough from player, pick it
            if (distToPlayerFromNextPoint >= safeDistance)
            {
                currentPointIndex = nextIndex;
                agent.SetDestination(pathPoints[currentPointIndex].position);
                return;
            }

            // Prevent infinite loop if no points safe enough
            if (tries > pathPoints.Length)
            {
                // No safe point found, fallback: move directly away from player
                Vector3 directionAway = (transform.position - player.position).normalized;
                Vector3 fleeTarget = transform.position + directionAway * safeDistance;

                NavMeshHit hit;
                if (NavMesh.SamplePosition(fleeTarget, out hit, 2f, NavMesh.AllAreas))
                {
                    agent.SetDestination(hit.position);
                }
                return;
            }
        }
        while (true);
    }
}
