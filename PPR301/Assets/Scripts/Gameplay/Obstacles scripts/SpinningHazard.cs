using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningHazard : MonoBehaviour
{
    public int hazards;
    public float distance;
    private float distance2;
    public GameObject obj;
    public Transform parentObject; // Assign an empty GameObject in the Inspector
    // Start is called before the first frame update
    void Start()
    {
        distance2 = distance;
        for(int i = 0; i < hazards; i++)
        {
            Debug.Log("hello");
            //x
            GameObject obj1 = Instantiate(obj, new Vector3(transform.position.x + distance2,transform.position.y,transform.position.z), Quaternion.identity);
            GameObject obj2 = Instantiate(obj, new Vector3(transform.position.x - distance2,transform.position.y,transform.position.z), Quaternion.identity);
            //z
            GameObject obj3 = Instantiate(obj, new Vector3(transform.position.x, transform.position.y, transform.position.z + distance2), Quaternion.identity);
            GameObject obj4 = Instantiate(obj, new Vector3(transform.position.x, transform.position.y, transform.position.z - distance2), Quaternion.identity);
            distance2 = distance2 + distance;
            obj1.transform.SetParent(parentObject); // Set parent
            obj2.transform.SetParent(parentObject); // Set parent
            obj3.transform.SetParent(parentObject); // Set parent
            obj4.transform.SetParent(parentObject); // Set parent
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
