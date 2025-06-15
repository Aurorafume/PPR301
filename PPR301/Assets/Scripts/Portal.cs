using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public Transform[] portalPoints;
    public Transform point;
    public PortalTypes portalType;
    public enum PortalTypes
    {
        portal1,
        portal2,
        portal3,
        portal4
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.CompareTag("Mouse"))
        {
            //Debug.Log("Teleporting Mouse");
            //collider.gameObject.transform.position = new Vector3(point.position.x, 3.55f, point.position.z);

            switch(portalType)
            {
                case PortalTypes.portal1:
                Debug.Log("Portal 1");
                break;
                case PortalTypes.portal2:
                Debug.Log("Portal 2");
                break;
                case PortalTypes.portal3:
                Debug.Log("Portal 3");
                break;
                case PortalTypes.portal4:
                Debug.Log("Portal 4");
                break;
            }
        }
    }
}
