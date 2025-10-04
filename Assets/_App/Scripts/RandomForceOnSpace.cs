using System.Collections.Generic;
using UnityEngine;

public class RandomForceOnSpace : MonoBehaviour
{
    [Header("Force Settings")]
    [SerializeField] float minForce = 5f;
    [SerializeField] float maxForce = 20f;
    [SerializeField] float maxChargeTime = 2f;

    public float currentForce;

    [Header("Button Animation Settings")]
    [SerializeField] Transform buttonTransform; // The button to animate
    [SerializeField] float maxPushDistance = 0.2f; // Max local push down

    private List<Rigidbody> insideRigidbodies = new List<Rigidbody>();
    private float spaceHoldTime = 0f;
    private Vector3 buttonStartLocalPos;

    void Start()
    {
        if (buttonTransform == null)
            buttonTransform = transform;

        buttonStartLocalPos = buttonTransform.localPosition;
    }

    void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;
        if (rb != null && !insideRigidbodies.Contains(rb))
            insideRigidbodies.Add(rb);
    }

    void OnTriggerExit(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;
        if (rb != null && insideRigidbodies.Contains(rb))
            insideRigidbodies.Remove(rb);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            // Track hold time
            spaceHoldTime += Time.deltaTime;
            if (spaceHoldTime > maxChargeTime)
                spaceHoldTime = maxChargeTime;

            // Calculate current force
            currentForce = Mathf.Lerp(minForce, maxForce, spaceHoldTime / maxChargeTime);

            // Map force to push distance (local down)
            float pushPercent = (currentForce - minForce) / (maxForce - minForce); // 0..1
            buttonTransform.localPosition = buttonStartLocalPos - buttonTransform.localRotation * Vector3.up * (pushPercent * maxPushDistance);

        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            // Apply force to objects inside
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

            // Reset
            spaceHoldTime = 0f;
            buttonTransform.localPosition = buttonStartLocalPos;
        }

        // Smooth return if key not held
        if (!Input.GetKey(KeyCode.Space))
        {
            buttonTransform.localPosition = Vector3.Lerp(buttonTransform.localPosition, buttonStartLocalPos, Time.deltaTime * 10f);
        }
    }
}
