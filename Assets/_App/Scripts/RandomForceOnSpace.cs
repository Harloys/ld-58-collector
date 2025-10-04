using System.Collections.Generic;
using UnityEngine;

public class RandomForceOnSpace : MonoBehaviour
{
    [SerializeField] float minForce = 5f; // Minimum push strength
    [SerializeField] float maxForce = 20f; // Maximum push strength
    [SerializeField] float maxChargeTime = 2f; // Time to reach max force

    private List<Rigidbody> insideRigidbodies = new List<Rigidbody>();
    private float spaceHoldTime = 0f;

    void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;
        if (rb != null && !insideRigidbodies.Contains(rb))
        {
            insideRigidbodies.Add(rb);
        }
    }

    void OnTriggerExit(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;
        if (rb != null && insideRigidbodies.Contains(rb))
        {
            insideRigidbodies.Remove(rb);
        }
    }

    void Update()
    {
        // Track how long space is held
        if (Input.GetKey(KeyCode.Space))
        {
            spaceHoldTime += Time.deltaTime;
            if (spaceHoldTime > maxChargeTime)
                spaceHoldTime = maxChargeTime; // Clamp to max charge
        }

        // Apply force when key is released
        if (Input.GetKeyUp(KeyCode.Space))
        {
            float forceAmount = Mathf.Lerp(minForce, maxForce, spaceHoldTime / maxChargeTime);

            foreach (Rigidbody rb in insideRigidbodies)
            {
                if (rb != null)
                {
                    Vector3 upward = Vector3.up;
                    Vector3 randomSide = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
                    Vector3 randomDirection = (upward * 2f + randomSide).normalized;

                    rb.AddForce(randomDirection * forceAmount, ForceMode.Impulse);
                }
            }

            spaceHoldTime = 0f; // Reset hold time
        }
    }
}