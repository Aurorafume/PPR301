using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevealKey : MonoBehaviour
{
    public float moveSpeed;
    public AnimationCurve moveCurve;

    public Vector3 localEndPosition;
    Vector3 startPostion;
    Vector3 endPostion;

    Rigidbody rb;
    Collider myCollider;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        myCollider = GetComponentInChildren<Collider>();
    } 

    void Start()
    {
        startPostion = transform.position;
        endPostion = transform.position + localEndPosition;

        SetKeyInteractable(false);
    }

    void SetKeyInteractable(bool isActive)
    {
        if (isActive)
        {
            rb.isKinematic = false;
            myCollider.enabled = true;
        }
        else
        {
            rb.isKinematic = true;
            myCollider.enabled = false;
        }
    }

    public void Reveal()
    {
        StartCoroutine(MoveCoroutine());
    }

    IEnumerator MoveCoroutine()
    {
        float target = 1f;
        float current = 0f;

        while (current != target)
        {
            current = Mathf.MoveTowards(current, target, Time.deltaTime * moveSpeed);
            Vector3 newPosition = Vector3.Lerp(startPostion, endPostion, moveCurve.Evaluate(current));

            transform.position = newPosition;

            yield return new WaitForEndOfFrame();
        }

        SetKeyInteractable(true);
    }
}
