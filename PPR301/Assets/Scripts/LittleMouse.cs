using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LittleMouse : MonoBehaviour
{
    public Vector3 playerLocation;
    public GameObject player;
    private NavMeshAgent agent;
    //patrol
    public Transform[] patrolPoints;
    public int currentPatrolPointIndex;
    public float maxWaitingTime;
    public float currentWaitingTime;
    public MouseStates mouseState;
    //wait for mouse patrol after running
    public float mousePatrolWait;

    public enum MouseStates
    {
        Patroling,
        RunningAway
    }
   
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentPatrolPointIndex = 0;
        maxWaitingTime = 0;
        currentWaitingTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        playerLocation = player.transform.position;
        DistanceBehaviour();
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
    void DistanceBehaviour()
    {
        if(Vector3.Distance(gameObject.transform.position, playerLocation) < 5f)
        {
            mouseState = MouseStates.RunningAway;
            mousePatrolWait = 5f;
            GoToNextPoint();
        }
        else
        {
            WaitAndPatrol();
        }
    }
    void WaitAndPatrol()
    {
        mousePatrolWait -= Time.deltaTime;
        if(mousePatrolWait <= 0)
        {
            mouseState = MouseStates.Patroling;
        }
    }
    void RatRunaway()
    {
        // run away
        //agent.SetDestination(playerLocation);
        //Vector3 dirAway = (transform.position - playerLocation).normalized;
        //Vector3 targetPos = transform.position + dirAway * 15f;
        //agent.SetDestination(targetPos);
        //run away 2
        Vector3 dirAway = (transform.position - playerLocation).normalized;
        Vector3 rawTarget = transform.position + dirAway * 5f;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(rawTarget, out hit, 5f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
        else
        {
            Debug.LogWarning("No valid NavMesh point found near: " + rawTarget);
            agent.ResetPath();
        }
    }
    void RatPatrol()
    {
        agent.SetDestination(patrolPoints[currentPatrolPointIndex].position);
        if(agent.remainingDistance < 0.5f)
        {
            GoToNextPoint();
        }
    }
    void GoToNextPoint()
    {
        if(patrolPoints.Length != 0)
        {
            //currentPatrolPointIndex = (currentPatrolPointIndex + 1) % patrolPoints.Length;
            currentPatrolPointIndex = Random.Range(0, patrolPoints.Length);
        }
    }
    void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.CompareTag("Portal"))
        {
            Debug.Log("setting wait time to 0");
            mousePatrolWait = 0;
        }
    }
}
