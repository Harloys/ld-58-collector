using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class BallTriggerTracker : MonoBehaviour
{
    public string ballTag = "Ball";
    public UnityEvent onNoBallsLeft;
    private List<Rigidbody> ballsInside = new();

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(ballTag)) 
            return;
        var rb = other.attachedRigidbody;
        if (rb != null)
            ballsInside.Add(rb);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(ballTag))
            return;
        var rb = other.attachedRigidbody;
        if (rb != null && ballsInside.Contains(rb))
        {
            ballsInside.Remove(rb);
            if (ballsInside.Count == 0)
            {
                onNoBallsLeft.Invoke();
            }
        }
    }
}