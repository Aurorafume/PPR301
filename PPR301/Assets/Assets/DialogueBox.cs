using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueBox : MonoBehaviour
{
    //talk
    public TMP_Text textSign;
    public GameObject speechBubble;
    public List<string> sentenceList = new List<string>();
    public int sentenceIndex;
    //distance
    public float dist;
    public Transform player;
    //icon
    public GameObject icon;

    // Start is called before the first frame update
    void Start()
    {
        
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
        if(dist < 2)
        {
            //icon
            icon.SetActive(true);
            icon.transform.LookAt(Camera.main.transform.position, Vector3.up);
            icon.transform.Rotate(0f, 180f, 0f);
            //talk
            if(Input.GetMouseButtonDown(0))
            {
                speechBubble.SetActive(true);
                if(sentenceIndex < sentenceList.Count)
                {
                    sentenceIndex = sentenceIndex + 1;
                    textSign.text = sentenceList[sentenceIndex - 1];
                }
                else
                {
                    sentenceIndex = 0;
                    speechBubble.SetActive(false);
                }
            }
        }
        else
        {
            icon.SetActive(false);
        }
    }
}
