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

    }
}
