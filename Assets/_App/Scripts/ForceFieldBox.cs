using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ForceFieldBox : MonoBehaviour
{
    public float slowDownRate = 2f;
    public LayerMask affectedLayers;

    private void Reset()
    {
        var col = GetComponent<BoxCollider>();
        col.isTrigger = true;
        col.size = new Vector3(5f, 5f, 5f);
    }

    private void OnTriggerStay(Collider other)
    {
        if (((1 << other.gameObject.layer) & affectedLayers) == 0)
            return;
        
        var rb = other.attachedRigidbody;
        if (!rb) 
            return;
            
        rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, slowDownRate * Time.fixedDeltaTime);
        rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, Vector3.zero, slowDownRate * Time.fixedDeltaTime);
    }
}