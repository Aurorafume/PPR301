using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin_Object : MonoBehaviour
{
    public float spinSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(0, Time.time * spinSpeed * 5, 0);
    }
}
