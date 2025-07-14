using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueBox : MonoBehaviour
{
    private Animator anim;
    private Animator anim2;
    //talk
    public TMP_Text textSign;
    public GameObject speechBubble;
    public int listIndex;
    public List<List<string>> sentenceListList = new List<List<string>>();
    public List<string> sentenceList = new List<string>();
    public List<string> sentenceList2 = new List<string>();
    public int sentenceIndex;
    public static bool isTalking;
    //distance
    public float dist; //individual distance
    public static float distFromRaccoonList; //distance from anything in the raccoonList
    public Transform player;
    //icon
    public GameObject icon;
    //raccoon list
    public static List<DialogueBox> raccoonList = new List<DialogueBox>(); 
    //
    public float letters;
    //
    public Camera camera1;
    //
    public GameObject key;
    public GameObject light;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        anim2 = speechBubble.GetComponent<Animator>();
        sentenceListList.Add(sentenceList);
        sentenceListList.Add(sentenceList2);
    }
    void OnEnable()
    {
        raccoonList.Add(this);
    }
    void OnDisable()
    {
        raccoonList.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        Billboard();
        //Talk();
        Talk2();
        //talk3();
    }
    void Billboard()
    {
        //transform.LookAt(Camera.main.transform.position, Vector3.up);
        //smooth rotation
        Vector3 direction = camera1.transform.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        // Flip 180 degrees around Y axis
        targetRotation *= Quaternion.Euler(0, 180f, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 12);



    }
    void Talk()
    {
        dist = Vector3.Distance(player.position, transform.position);
        if(dist < 3)
        {
            //icon
            icon.SetActive(true);
            icon.transform.LookAt(Camera.main.transform.position, Vector3.up);
            icon.transform.Rotate(0f, 180f, 0f);
            //talk
            if(Input.GetMouseButtonDown(0))
            {
                anim.SetBool("talking", true);
                speechBubble.SetActive(true);
                if(sentenceIndex < sentenceListList[listIndex].Count)
                {
                    sentenceIndex = sentenceIndex + 1;
                    textSign.text = sentenceListList[listIndex][sentenceIndex - 1];
                }
                else if(listIndex < sentenceListList.Count - 1)
                {
                    listIndex = listIndex + 1;
                    sentenceIndex = 0;
                    speechBubble.SetActive(false);
                    anim.SetBool("talking", false);
                }
                else
                {
                    Debug.Log("Reached end of dialogue");
                    sentenceIndex = 0;
                    speechBubble.SetActive(false);
                    anim.SetBool("talking", false);
                }

                //if(sentenceIndex < sentenceList.Count)
                //{
                //    sentenceIndex = sentenceIndex + 1;
                //    textSign.text = sentenceList[sentenceIndex - 1];
                //}
                //else
                //{
                //    anim.SetBool("talking", false);
                //    sentenceIndex = 0;
                //    speechBubble.SetActive(false);
                //}
            }
        }
        else
        {
            anim.SetBool("talking", false);
            icon.SetActive(false);
            //reset speech
            sentenceIndex = 0;
            speechBubble.SetActive(false);
        }
    }
    void Talk2()
    {
        bool nearAnyRaccoon = false;
        dist = Vector3.Distance(player.position, transform.position);
        foreach(DialogueBox rac in raccoonList)
        {
            distFromRaccoonList = Vector3.Distance(player.position, rac.transform.position);
            if(distFromRaccoonList < 3)
            {
                nearAnyRaccoon = true;
                //talk
                if(Input.GetMouseButtonDown(0))
                {
                    isTalking = true;
                }
                break;
            }
        }
        if (!nearAnyRaccoon)
        {
            icon.SetActive(false);
            //reset talking
            isTalking = false;
            anim.SetBool("talking", false);
            anim2.SetBool("isTalking", false);
            //reset speech
            sentenceIndex = 0;
            //speechBubble.SetActive(false);
        }
        else
        {
            //icon
            icon.SetActive(true);
            icon.transform.LookAt(Camera.main.transform.position, Vector3.up);
            icon.transform.Rotate(0f, 180f, 0f);
            //talk
            if(dist < 3)
            if(Input.GetMouseButtonDown(0))
            {
                anim.SetBool("talking", true);
                anim2.SetBool("isTalking", true);
                //speechBubble.SetActive(true);
                if(sentenceIndex < sentenceListList[listIndex].Count)
                {
                    sentenceIndex = sentenceIndex + 1;
                    textSign.text = sentenceListList[listIndex][sentenceIndex - 1];
                }
                else if(listIndex < sentenceListList.Count - 1)
                {
                    listIndex = listIndex + 1;
                    sentenceIndex = 0;
                    //speechBubble.SetActive(false);
                    anim.SetBool("talking", false);
                    anim2.SetBool("isTalking", false);
                    if(key != null)
                    key.SetActive(true);
                    if(light != null)
                    light.SetActive(false);
                }
                else
                {
                    Debug.Log("Reached end of dialogue");
                    sentenceIndex = 0;
                    //speechBubble.SetActive(false);
                    anim.SetBool("talking", false);
                    anim2.SetBool("isTalking", false);
                }
            }
            
        }
    }
    void talk3()
    {
        string text = "Hello how are you?";
        speechBubble.SetActive(true);
        letters -= Time.deltaTime;
        textSign.text = text.Substring(0, text.Length - (int)letters);
    }
}
