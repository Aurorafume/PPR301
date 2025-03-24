using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningHazard : MonoBehaviour
{
    public int hazards;
    public float distance;
    public GameObject hazard;
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < hazards; i++)
        {
            Debug.Log("hello");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
