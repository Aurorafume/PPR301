using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueBox : MonoBehaviour
{
    public TMP_Text textSign;
    public List<string> sentenceList = new List<string>();
    public int sentenceIndex;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(sentenceIndex < sentenceList.Count)
            {
                sentenceIndex = sentenceIndex + 1;
                textSign.text = sentenceList[sentenceIndex - 1];
            }
            
        }
    }
}
