using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; 
using TMPro;

[RequireComponent(typeof(NavMeshAgent))]
public class GhostCatController : MonoBehaviour
{
    [Header("Game State & References")]
    [Tooltip("Reference to the main game state manager.")]
    public States gameStates;

    [Tooltip("Reference to the Piano script on the piano GameObject.")]
    public Piano piano;

    [Tooltip("An empty GameObject placed at the center of the piano keyboard.")]
    public Transform pianoLookTarget;

    [Header("UI Display")]
    [Tooltip("The TextMeshPro UI element to display the ghost's notes.")]
    public TextMeshProUGUI noteDisplayText;

    [Header("Movement")]
    [Tooltip("The transform representing the target position of the piano chair.")]
    public Transform pianoChairTarget;
    
    [Tooltip("How high to offset the ghost model to make it appear to sit on the chair. Adjust this in the Inspector.")]
    public float sittingHeightOffset = 0.5f;

    [Header("Game Settings")]
    [Tooltip("How many notes the final sequence will have.")]
    private const int TUNE_LENGTH = 10;

    [Tooltip("The delay in seconds before the ghost starts the first note sequence.")]
    public float initialWaitTime = 2.0f;

    [Tooltip("The delay in seconds between each note the ghost plays.")]
    public float delayBetweenNotes = 0.7f;
    
    [Tooltip("The distance the player must be from the chair to start the game.")]
    public float playerStartDistance = 2.0f;


    // Private Variables
    private NavMeshAgent agent;
    private List<int> noteSequence = new List<int>();
    private GhostState currentState;
    private int currentSequenceLevel = 2;
    private int playerInputStep = 0;

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
            currentState = GhostState.IDLE;
            return;
        }

        if (noteDisplayText == null)
        {
            Debug.LogWarning("Note Display Text is not assigned. Numbers will not be shown.");
        }
        else
        {
            noteDisplayText.text = "";
        }

        piano.ghostCatController = this;

        currentState = GhostState.IDLE;
    }

    void Update()
    {
        switch (currentState)
        {
            case GhostState.IDLE:
                if (gameStates.InMemoryRoomGame)
                {
                    Debug.Log("Player entered the room. Ghost is walking to the piano.");
                    
                    agent.enabled = true;
                    
                    agent.baseOffset = 0.5f; 
                    
                    agent.SetDestination(pianoChairTarget.position);
                    currentState = GhostState.WALKING_TO_PIANO;
                }
                break;

            case GhostState.WALKING_TO_PIANO:
                if (!agent.pathPending && agent.remainingDistance < 0.1f)
                {
                    Debug.Log("Ghost has reached the piano. Waiting for player.");
                    
                    agent.isStopped = true; 

                    agent.baseOffset = sittingHeightOffset;

                if (pianoLookTarget != null)
                {
                    Vector3 lookAtPosition = pianoLookTarget.position;
                    lookAtPosition.y = transform.position.y;
                    transform.LookAt(lookAtPosition);
                }
                else
                {
                    Debug.LogWarning("No pianoLookTarget assigned. Falling back to piano center.");
                    Vector3 lookAtPosition = piano.transform.position;
                    lookAtPosition.y = transform.position.y;
                    transform.LookAt(lookAtPosition);
                }

                    currentState = GhostState.WAITING_FOR_PLAYER;
                }
                break;

            case GhostState.WAITING_FOR_PLAYER:
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
        if (currentState != GhostState.PLAYERS_TURN) return;

        if (keyIndex == noteSequence[playerInputStep])
        {
            playerInputStep++;

            if (playerInputStep >= currentSequenceLevel)
            {
                Debug.Log($"Correct sequence for level {currentSequenceLevel}!");

                if (currentSequenceLevel >= TUNE_LENGTH)
                {
                    currentState = GhostState.GAME_WON;
                    Debug.Log("GAME WON! The player has successfully replicated the entire tune.");
                }
                else
                {
                    currentSequenceLevel++;
                    playerInputStep = 0;
                    currentState = GhostState.GHOSTS_TURN;
                    StartCoroutine(PlayGhostSequenceRoutine());
                }
            }
        }
        else
        {
            Debug.Log($"Incorrect note! Expected {noteSequence[playerInputStep]}, Player pressed {keyIndex}. Resetting level.");
            currentState = GhostState.INCORRECT_NOTE;
            playerInputStep = 0;
            
            StartCoroutine(PlayGhostSequenceRoutine(2.0f)); 
        }
    }

    private IEnumerator PlayIntroTuneRoutine()
    {
        Debug.Log("Ghost is playing the intro melody.");
        GenerateNewTune();

        yield return new WaitForSeconds(initialWaitTime);

        for (int i = 0; i < TUNE_LENGTH; i++)
        {
            int currentNote = noteSequence[i];

            if (noteDisplayText != null)
            {
                noteDisplayText.text = (currentNote + 1).ToString();
            }

            piano.PlayKey(currentNote);

            yield return new WaitForSeconds(delayBetweenNotes);
        }

        if (noteDisplayText != null)
        {
            noteDisplayText.text = "";
        }

        Debug.Log($"Intro finished. The game will now start.");
        currentState = GhostState.STARTING_GAME;
        StartCoroutine(PlayGhostSequenceRoutine());
    }

    private IEnumerator PlayGhostSequenceRoutine(float startDelay = 0f)
    {
        if(startDelay > 0) yield return new WaitForSeconds(startDelay);
        
        currentState = GhostState.GHOSTS_TURN;
        Debug.Log($"Ghost's turn. Playing {currentSequenceLevel} notes.");

        yield return new WaitForSeconds(1.0f);

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
            noteSequence.Add(Random.Range(0, 10));
        }
    }
}