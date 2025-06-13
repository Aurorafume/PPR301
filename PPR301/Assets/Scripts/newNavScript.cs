using UnityEngine;
using UnityEngine.AI;

public class newNavScript : MonoBehaviour
{
    public Transform player;
    public float fleeDistance = 5f;

    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        Vector3 directionAway = transform.position - player.position;
        Vector3 fleeTarget = transform.position + directionAway.normalized * fleeDistance;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(fleeTarget, out hit, 5f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }
}