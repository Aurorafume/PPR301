using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent; 
    public GameObject playerToChase; //players gameobject to chase
    public Vector3 playerLocation; //player's location
    //enemy
    public bool angry;
    public int aggroDistance;
    public float distanceFromPlayer;
    private float whenToRespawn;
    public float respawnTimer;
    public bool chasing;
    public Vector3 enemySpawnPoint; //where enemy respawns when it loses the player
    //animation
    private float turnNum;
    private bool right;

    public float fadeStrength = 100f;
    public bool fading;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        enemySpawnPoint = transform.position;

        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void Update()
    {   
        updatePlayerLocation();
        billboard();
        checkAggroDistance();
        enemyChase();
        touchPlayer();
        FadeTo();
    }
    public void enemyChase()
    {
        if(chasing)
        {
            agent.SetDestination(playerLocation);
            walkAnimation();
            whenToRespawn = respawnTimer;
            angry = true;
        }
        else
        {
            whenToRespawn -= Time.deltaTime;
            agent.SetDestination(transform.position);//stops immediately instead of drifting
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
            if(whenToRespawn <= 0)
            {
                fading = true;
                //transform.position = enemySpawnPoint;
                aggroDistance = 10;
            }
            angry = false;
        }
    }
    public void walkAnimation()
    {
        if(turnNum < 15 && right)
        {
            turnNum++;
            if(turnNum >= 15)
            {
                right = false;
            }
        }
        else if(turnNum > -15 && !right)
        {
            turnNum--;
            if(turnNum <= -15)
            {
                right = true;
            }
        }
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, turnNum);
    }
    public void checkAggroDistance()
    {
        //checks distance between player and enemy
        distanceFromPlayer = Vector3.Distance(transform.position, playerToChase.transform.position);
        if(distanceFromPlayer <= aggroDistance)
        {
            chasing = true;
        }
        else
        {
            chasing = false;
        }
        //longer follow distance when following
        if(angry)
        {
            aggroDistance = 15;
        }
    }
    public void updatePlayerLocation()
    {
        //set playerLocation to player's current location
        playerLocation = playerToChase.transform.position;
    }
    public void touchPlayer()//2D enemy
    {
        if(distanceFromPlayer <= 1.3f)
        {
            //do something when touching player
            Debug.Log("Touching Player");
        }
    }
    public void billboard()//turns enemy towards camera
    {
        Vector3 lookPos = Camera.main.transform.position - transform.position; // Get direction
        lookPos.y = 0; // Keep Y-axis fixed
        transform.rotation = Quaternion.LookRotation(-lookPos);
    }
    public void FadeTo()
    {
        //float fadeStrength = 100f;
        if(fading == true)
        {
            fadeStrength -= Time.deltaTime / 1.5f;
        }
        else
        {
            fadeStrength += Time.deltaTime / 1.5f;
        }
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, fadeStrength);
        fadeStrength = Mathf.Clamp(fadeStrength, 0, 1);
        if(fadeStrength == 0)
        {
            transform.position = enemySpawnPoint;
            fading = false;
        }

    }
}