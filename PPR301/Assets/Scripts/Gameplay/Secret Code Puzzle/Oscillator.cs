using Unity.VisualScripting.FullSerializer.Internal;
using UnityEngine;

public class Oscillator : MonoBehaviour
{
    [SerializeField] Vector3 offsetPosition1;
    [SerializeField] Vector3 offsetPosition2;
    [SerializeField] float startingOffset;
    [SerializeField] float period = 2f;

    Vector3 position1;
    Vector3 position2;
    float movementFactor;

    float cycles;
    
    void Start()
    {
        position1 = transform.localPosition + offsetPosition1;
        position2 = transform.localPosition + offsetPosition2;
    }

    void Update()
    {
        if (period <= Mathf.Epsilon) return;

        cycles = Time.time / period;

        const float tau = Mathf.PI * 2f;
        float angle = cycles * tau - (Mathf.PI / 2f) + startingOffset;

        float rawValue = Mathf.Sin(angle);

        movementFactor = (rawValue + 1f) / 2f;

        Vector3 offset = (position2 - position1) * movementFactor;
        transform.localPosition = position1 + offset;
    }
}
