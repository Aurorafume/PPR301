// ==========================================================================
// Meowt of Tune - Dialogue Box
// Owned and contributed to by: brookcoli (Itch.io) - Aurorafume (GitHub), donkzilla03 (Itch.io) - AliAK03 (GitHub), JaydenFielderTorrens (Itch.io) - Squib35 (GitHub), komorebimoriko (Itch.io), TheRealCrizz (GitHub), LordVGahn (GitHub)
// itch.io listing: https://brookcoli.itch.io/meowt-of-tune - GitHub Repository: https://github.com/Aurorafume/PPR301
// ==========================================================================
//
// WHAT DOES THIS DO:
// This script is a component for managing NPC dialogue interactions. It handles
// proximity checks to the player, displaying dialogue text sentence-by-sentence,
// controlling talking animations, and managing a "billboard" effect for the
// speech bubble to ensure it always faces the camera. It uses a static list
// to coordinate between multiple NPCs, preventing conflicting interactions.
//
// Core functionalities include:
// - A multi-stage dialogue system using lists of sentences.
// - Proximity-based activation and player input handling (mouse click).
// - Control of NPC and speech bubble animators.
// - A smooth billboard effect to keep the speech bubble facing the camera.
// - Management of a static list of all dialogue NPCs.
// - Ability to trigger game events (like activating a key) at the end of a dialogue tree.
//
// Dependencies:
// - Requires Animator components on the NPC and the 'speechBubble' GameObject.
// - Requires a TextMeshPro component ('textSign') for displaying dialogue text.
// - The sentence lists must be populated with dialogue lines in the Inspector.
// - Requires a 'player' transform and a 'camera1' to be assigned.
//
// ==========================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Manages the dialogue system for an NPC, including text progression and animations.
/// </summary>
public class DialogueBox : MonoBehaviour
{
    [Header("UI & Text")]
    [Tooltip("The TextMeshPro component where dialogue text is displayed.")]
    public TMP_Text textSign;
    [Tooltip("The parent GameObject of the speech bubble UI.")]
    public GameObject speechBubble;
    [Tooltip("The icon that appears above the NPC to indicate interaction is possible.")]
    public GameObject icon;

    [Header("Dialogue Content")]
    [Tooltip("The first list of sentences for the dialogue tree.")]
    public List<string> sentenceList = new List<string>();
    [Tooltip("The second list of sentences, used after the first is completed.")]
    public List<string> sentenceList2 = new List<string>();
    
    [Header("Game Event Objects")]
    [Tooltip("A key or other object to activate after the dialogue is complete.")]
    public GameObject key;
    [Tooltip("A light or other indicator to deactivate after the dialogue is complete.")]
    public GameObject indicatorLight;

    [Header("Required References")]
    [Tooltip("The main player transform.")]
    public Transform player;
    [Tooltip("The main camera used for the billboard effect.")]
    public Camera camera1;

    // --- Private & Static State Variables ---
    private Animator anim; // This NPC's animator.
    private Animator speechBubbleAnim; // The speech bubble's animator.
    private int listIndex; // The index of the current list of sentences (0 or 1).
    private int sentenceIndex; // The index of the current sentence within the list.
    private List<List<string>> sentenceListList = new List<List<string>>(); // A list containing the other sentence lists.
    private static List<DialogueBox> raccoonList = new List<DialogueBox>(); // A static list of all dialogue NPCs in the scene.

    public SoundEffects raccoonLaugh;
    public int audioPoint;
    
    /// <summary>
    /// Caches components and initialises the dialogue structure.
    /// </summary>
    void Start()
    {
        anim = GetComponent<Animator>();
        speechBubbleAnim = speechBubble.GetComponent<Animator>();
        
        // Combine the individual sentence lists into a master list for easier progression.
        sentenceListList.Add(sentenceList);
        sentenceListList.Add(sentenceList2);

        //assign raccoon laugh
        raccoonLaugh = GameObject.Find("Sound effects").GetComponent<SoundEffects>();
    }

    /// <summary>
    /// Adds this instance to the static list of all dialogue NPCs when it becomes active.
    /// </summary>
    void OnEnable()
    {
        raccoonList.Add(this);
    }

    /// <summary>
    /// Removes this instance from the static list when it is disabled or destroyed.
    /// </summary>
    void OnDisable()
    {
        raccoonList.Remove(this);
    }

    /// <summary>
    /// Main update loop, called once per frame.
    /// </summary>
    void Update()
    {
        BillboardSpeechBubble();
        HandleDialogue();
    }

    /// <summary>
    /// Rotates the speech bubble to smoothly face the camera at all times.
    /// </summary>
    void BillboardSpeechBubble()
    {
        // Calculate the direction from the bubble to the camera.
        Vector3 direction = camera1.transform.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        
        // Flip the rotation 180 degrees so the front face is visible.
        targetRotation *= Quaternion.Euler(0, 180f, 0);
        
        // Smoothly interpolate to the target rotation.
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 12);
    }
    
    /// <summary>
    /// Manages the core logic for dialogue interaction based on player proximity and input.
    /// </summary>
    void HandleDialogue()
    {
        bool isPlayerNearAnyRaccoon = false;
        
        // Check if the player is close to any of the raccoons in the scene.
        foreach (DialogueBox rac in raccoonList)
        {
            if (Vector3.Distance(player.position, rac.transform.position) < 3f)
            {
                isPlayerNearAnyRaccoon = true;
                break; // Found one nearby, no need to check the rest.
            }
        }

        // If the player is not near any raccoon, reset this raccoon's dialogue state.
        if (!isPlayerNearAnyRaccoon)
        {
            icon.SetActive(false);
            anim.SetBool("talking", false);
            speechBubbleAnim.SetBool("isTalking", false);
            sentenceIndex = 0;
            audioPoint = 1;
        }
        // If the player is near at least one raccoon, handle interaction logic.
        else
        {
            // Show the interaction icon for this raccoon.
            icon.SetActive(true);
            icon.transform.LookAt(Camera.main.transform.position, Vector3.up);
            icon.transform.Rotate(0f, 180f, 0f);

            // If the player is close to THIS specific raccoon and clicks...
            if (Vector3.Distance(player.position, transform.position) < 3f && Input.GetMouseButtonDown(0))
            {
                if(audioPoint == 1)
                {
                    raccoonLaugh.raccoonLaugh.Play();
                    audioPoint = 0;
                }
                // ...start the talking animations.
                anim.SetBool("talking", true);
                speechBubbleAnim.SetBool("isTalking", true);
                
                // If there are more sentences in the current list, show the next one.
                if (sentenceIndex < sentenceListList[listIndex].Count)
                {
                    textSign.text = sentenceListList[listIndex][sentenceIndex];
                    sentenceIndex++;
                }
                // If at the end of a list but there is another list to show, switch to it.
                else if (listIndex < sentenceListList.Count - 1)
                {
                    listIndex++;
                    sentenceIndex = 0;
                    anim.SetTrigger("Trigger");
                    speechBubbleAnim.SetBool("isTalking", false);
                    
                    // Trigger game events after completing a dialogue stage.
                    if (key != null) key.SetActive(true);
                    if (indicatorLight != null) indicatorLight.SetActive(false);

                    audioPoint = 1;
                }
                // If at the end of all dialogue, reset.
                else
                {
                    sentenceIndex = 0;
                    anim.SetBool("talking", false);
                    speechBubbleAnim.SetBool("isTalking", false);
                    audioPoint = 1;
                }
            }
        }
    }
}