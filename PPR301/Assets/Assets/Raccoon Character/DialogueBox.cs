using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueBox : MonoBehaviour
{
    private Animator anim;
    //talk
    public TMP_Text textSign;
    public GameObject speechBubble;
    public int listIndex;
    public List<List<string>> sentenceListList = new List<List<string>>();
    public List<string> sentenceList = new List<string>();
    public List<string> sentenceList2 = new List<string>();
    public int sentenceIndex;
    //distance
    public float dist;
    public Transform player;
    //icon
    public GameObject icon;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        sentenceListList.Add(sentenceList);
        sentenceListList.Add(sentenceList2);
        //anim.SetBool("talking", false);
    }

    // Update is called once per frame
    void Update()
    {
        Billboard();
        Talk();
    }
    void Billboard()
    {
        transform.LookAt(Camera.main.transform.position, Vector3.up);
        transform.Rotate(0f, 180f, 0f);
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
}
