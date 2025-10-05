using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ForceField : MonoBehaviour
{
    public float forceStrength = 10f;
    public ForceMode forceMode = ForceMode.Acceleration;

    private void OnTriggerStay(Collider other)
    {
        if(other.attachedRigidbody)
            other.attachedRigidbody.AddForce(transform.forward * forceStrength, forceMode);
    }
}