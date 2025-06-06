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

    public enum MouseStates
    {
        Patroling,
        RunningAway
    }
   
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentPatrolPointIndex = -1;
        maxWaitingTime = 0;
        currentWaitingTime = 0;
        GoToNextPoint();
    }

    // Update is called once per frame
    void Update()
    {
        //updatePlayerLocation();
        //enemyAvoid();
        MousePatrol();
    }
    void MousePatrol()
    {
        if(agent.remainingDistance < 0.5f)
        {
            if(maxWaitingTime == 0)
            maxWaitingTime = Random.Range(2, 5);

            if(currentWaitingTime >= maxWaitingTime)
            {
                maxWaitingTime = 0;
                currentWaitingTime = 0;
                GoToNextPoint();
            }
            else currentWaitingTime += Time.deltaTime;
        }
    }
    void GoToNextPoint()
    {
        if(patrolPoints.Length != 0)
        {
            currentPatrolPointIndex = (currentPatrolPointIndex + 1) % patrolPoints.Length;
            agent.SetDestination(patrolPoints[currentPatrolPointIndex].position);
        }
    }
    void updatePlayerLocation()
    {
        playerLocation = player.transform.position;
    }
    void enemyAvoid()
    {
        if (Vector3.Distance(transform.position, playerLocation) < 3f)
        {
            // move away
            //agent.SetDestination(playerLocation);
            Vector3 dirAway = (transform.position - playerLocation).normalized;
            Vector3 targetPos = transform.position + dirAway * 5f;
            agent.SetDestination(targetPos);
        }
    }
}
