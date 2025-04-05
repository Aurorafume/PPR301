using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent; 
    private GameObject playerToChase;
    public Vector3 playerLocation;
    public bool angry;
    public int aggroDistance;
    public float distanceFromPlayer;
    private float whenToRespawn;
    public float respawnTimer;
    public bool chasing;
    private float turnNum;
    private bool right;
    public float fadeStrength = 100f;
    public bool fading;
    private SpriteRenderer spriteRenderer;
    private NoiseBar noiseBar;
    public States states;
    public GameObject player;
    
    void Start()
    {
        // get the States script
        states = FindObjectOfType<States>();
        player = GameObject.Find("Player");

        // Set the player to chase as anything with the tag player
        playerToChase = GameObject.Find("Player");

        spriteRenderer = GetComponent<SpriteRenderer>();

        noiseBar = FindObjectOfType<NoiseBar>();
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

            if (noiseBar != null)
            {
                noiseBar.ForceChaseVisuals(true);
            }
        }
        else
        {
            whenToRespawn -= Time.deltaTime;
            agent.SetDestination(transform.position);
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);

            if (whenToRespawn <= 0)
            {
                fading = true;
                Debug.Log("Enemy Despawned");
                aggroDistance = 10;

                if (noiseBar != null)
                {
                    noiseBar.ForceChaseVisuals(false);
                }
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
            aggroDistance = 20;
        }
    }

    public void updatePlayerLocation()
    {
        //set playerLocation to player's current location
        playerLocation = playerToChase.transform.position;
    }

    public void touchPlayer()
    {   
        GameObject hidingSpot = GameObject.FindGameObjectWithTag("Hiding Spot");

        if (distanceFromPlayer <= 2f)
        {
            Debug.Log("Touching Player");

            if (states.playerIsHiding)
            {
                Debug.Log("Player is hiding. Enemy will despawn.");
                fading = true;
                noiseBar.StopChase();

                // Stop chase visuals when despawning due to hiding
                if (noiseBar != null)
                {
                    noiseBar.ForceChaseVisuals(false);
                }

                FadeTo();
            }
            else
            {
                states.gameOver = true;
            }
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
            //transform.position = enemySpawnPoint;
            fading = false;
            
            // Reset enemy existence flag
            NoiseHandler.NotifyEnemyDespawned();
        }

    }
}