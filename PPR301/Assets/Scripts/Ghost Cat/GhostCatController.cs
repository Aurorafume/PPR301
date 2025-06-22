using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; 

[RequireComponent(typeof(NavMeshAgent))]
public class GhostCatController : MonoBehaviour
{
    [Header("Game State & References")]
    [Tooltip("Reference to the main game state manager.")]
    public States gameStates;

    [Tooltip("Reference to the Piano script on the piano GameObject.")]
    public Piano piano;

    [Header("Movement")]
    [Tooltip("The transform representing the target position ON THE FLOOR in front of the piano chair.")]
    public Transform pianoChairTarget;
    
    [Tooltip("How high to offset the ghost model to make it appear to sit on the chair. Adjust this in the Inspector.")]
    public float sittingHeightOffset = 0.5f;

    [Header("Game Settings")]
    [Tooltip("How many notes the final sequence will have.")]
    private const int TUNE_LENGTH = 15;

    [Tooltip("The delay in seconds before the ghost starts the first note sequence.")]
    public float initialWaitTime = 5.0f;

    [Tooltip("The delay in seconds between each note the ghost plays.")]
    public float delayBetweenNotes = 0.7f;
    
    [Tooltip("The distance the player must be from the chair to start the game.")]
    public float playerStartDistance = 2.0f;


    // --- Private Variables ---
    private NavMeshAgent agent;
    private List<int> noteSequence = new List<int>();
    private GhostState currentState;
    private int currentSequenceLevel = 1; // How many notes are in the current sequence to remember
    private int playerInputStep = 0; // Which step of the sequence the player is on

    // Enum to manage the different states of the ghost's behavior
    private enum GhostState
    {
        IDLE,
        WALKING_TO_PIANO,
        WAITING_FOR_PLAYER,
        PLAYING_INTRO_TUNE,
        STARTING_GAME,
        GHOSTS_TURN,
        PLAYERS_TURN,
        INCORRECT_NOTE,
        GAME_WON
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (piano == null || gameStates == null || pianoChairTarget == null)
        {
            Debug.LogError("GhostCatController is missing one or more references (Piano, States, or PianoChairTarget). Please assign them in the Inspector.");
            currentState = GhostState.IDLE; // Prevent errors
            return;
        }

        // Link this controller to the piano script so it can receive player input
        piano.ghostCatController = this;

        // Start in the idle state
        currentState = GhostState.IDLE;
    }

    void Update()
    {
        // State machine to control ghost behavior
        switch (currentState)
        {
            case GhostState.IDLE:
                // When the player enters the room, the States script will set this flag.
                if (gameStates.InMemoryRoomGame)
                {
                    Debug.Log("Player entered the room. Ghost is walking to the piano.");
                    agent.baseOffset = 0; // Ensure ghost is on the ground while walking.
                    agent.SetDestination(pianoChairTarget.position);
                    currentState = GhostState.WALKING_TO_PIANO;
                }
                break;

            case GhostState.WALKING_TO_PIANO:
                // Check if the ghost has reached the piano chair.
                if (!agent.pathPending && agent.remainingDistance < 0.1f)
                {
                    Debug.Log("Ghost has reached the piano. Waiting for player.");
                    
                    // --- POSITION & ROTATION FIX ---
                    // 1. Stop the agent to prevent it from trying to move further.
                    agent.isStopped = true; 
                    
                    // 2. Set the vertical offset to "sit" on the chair.
                    agent.baseOffset = sittingHeightOffset;

                    // 3. Force the ghost to look at the piano, but only rotate on the Y-axis.
                    Vector3 lookAtPosition = piano.transform.position;
                    lookAtPosition.y = transform.position.y; // Keep the ghost level, prevent tilting.
                    transform.LookAt(lookAtPosition);
                    // --------------------------------

                    currentState = GhostState.WAITING_FOR_PLAYER;
                }
                break;

            case GhostState.WAITING_FOR_PLAYER:
                // Check if the player is close enough to the piano chair to begin.
                if (Vector3.Distance(gameStates.player.transform.position, pianoChairTarget.position) < playerStartDistance)
                {
                    Debug.Log("Player has joined the ghost. Starting the tune.");
                    currentState = GhostState.PLAYING_INTRO_TUNE;
                    StartCoroutine(PlayIntroTuneRoutine());
                }
                break;
        }
    }

    public void PlayerPressedKey(int keyIndex)
    {
        // Only process player input if it's their turn.
        if (currentState != GhostState.PLAYERS_TURN) return;

        // Check if the player's note matches the sequence
        if (keyIndex == noteSequence[playerInputStep])
        {
            // Correct note
            playerInputStep++;

            // Check if the player has completed the current sequence
            if (playerInputStep >= currentSequenceLevel)
            {
                Debug.Log($"Correct sequence for level {currentSequenceLevel}!");
                // Check for win condition
                if (currentSequenceLevel >= TUNE_LENGTH)
                {
                    currentState = GhostState.GAME_WON;
                    Debug.Log("GAME WON! The player has successfully replicated the entire tune.");
                    // You can add logic here for what happens when the player wins.
                }
                else
                {
                    // Advance to the next level and let the ghost play.
                    currentSequenceLevel++;
                    playerInputStep = 0;
                    currentState = GhostState.GHOSTS_TURN;
                    StartCoroutine(PlayGhostSequenceRoutine());
                }
            }
        }
        else
        {
            // Incorrect note
            Debug.Log($"Incorrect note! Expected {noteSequence[playerInputStep]}, Player pressed {keyIndex}. Resetting level.");
            currentState = GhostState.INCORRECT_NOTE;
            playerInputStep = 0; // Reset player progress
            
            // Replay the ghost's sequence after a short delay
            StartCoroutine(PlayGhostSequenceRoutine(2.0f)); 
        }
    }

    private IEnumerator PlayIntroTuneRoutine()
    {
        Debug.Log("Ghost is playing the intro melody.");
        GenerateNewTune(); // Create the random sequence of notes

        // Play the full tune for ambience
        for (int i = 0; i < TUNE_LENGTH; i++)
        {
            piano.PlayKey(noteSequence[i]);
            yield return new WaitForSeconds(delayBetweenNotes);
        }

        Debug.Log($"Intro finished. Game will start in {initialWaitTime} seconds.");
        yield return new WaitForSeconds(initialWaitTime);

        currentState = GhostState.STARTING_GAME;
        StartCoroutine(PlayGhostSequenceRoutine());
    }

    private IEnumerator PlayGhostSequenceRoutine(float startDelay = 0f)
    {
        if(startDelay > 0) yield return new WaitForSeconds(startDelay);
        
        currentState = GhostState.GHOSTS_TURN;
        Debug.Log($"Ghost's turn. Playing {currentSequenceLevel} notes.");

        yield return new WaitForSeconds(1.0f); // Small pause before playing

        for (int i = 0; i < currentSequenceLevel; i++)
        {
            piano.PlayKey(noteSequence[i]);
            yield return new WaitForSeconds(delayBetweenNotes);
        }

        Debug.Log("Player's turn.");
        currentState = GhostState.PLAYERS_TURN;
    }

    private void GenerateNewTune()
    {
        noteSequence.Clear();
        for (int i = 0; i < TUNE_LENGTH; i++)
        {
            // Adds a random note index (0-9) to the sequence
            noteSequence.Add(Random.Range(0, 10));
        }
    }
}