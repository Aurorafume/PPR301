using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public Transform[] portalPoints;
    public LittleMouse script;
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
            //collider.gameObject.transform.position = new Vector3(portalPoints[0].position.x, collider.gameObject.transform.position.y, portalPoints[0].position.z);
            script.mousePatrolWait = 0f;

            switch(portalType)
            {
                case PortalTypes.portal1:
                Debug.Log("Portal 1");
                collider.gameObject.transform.position = new Vector3(portalPoints[0].position.x, collider.gameObject.transform.position.y, portalPoints[0].position.z);
                break;
                case PortalTypes.portal2:
                Debug.Log("Portal 2");
                collider.gameObject.transform.position = new Vector3(portalPoints[1].position.x, collider.gameObject.transform.position.y, portalPoints[1].position.z);
                break;
                case PortalTypes.portal3:
                Debug.Log("Portal 3");
                collider.gameObject.transform.position = new Vector3(portalPoints[2].position.x, collider.gameObject.transform.position.y, portalPoints[2].position.z);
                break;
                case PortalTypes.portal4:
                Debug.Log("Portal 4");
                collider.gameObject.transform.position = new Vector3(portalPoints[3].position.x, collider.gameObject.transform.position.y, portalPoints[3].position.z);
                break;
            }
        }
    }
}
