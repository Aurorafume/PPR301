using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LittleMouse : MonoBehaviour
{
    public Vector3 playerLocation;
    public GameObject player;
    public NavMeshAgent agent;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        updatePlayerLocation();
        enemyAvoid();
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
