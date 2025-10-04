using System.Collections.Generic;
using UnityEngine;

public class RandomForceOnSpace : MonoBehaviour
{
    [SerializeField] float forceAmount = 10f; // Strength of push
    private List<Rigidbody> insideRigidbodies = new List<Rigidbody>();

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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            foreach (Rigidbody rb in insideRigidbodies)
            {
                if (rb != null)
                {
                    Vector3 randomDirection = Random.onUnitSphere;
                    rb.AddForce(randomDirection * forceAmount, ForceMode.Impulse);
                }
            }
        }
    }
}