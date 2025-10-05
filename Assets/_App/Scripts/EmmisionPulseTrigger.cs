using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;

public class EmissionPulseTrigger : MonoBehaviour
{
    public UnityEvent OnTrigger;
    
    public Material targetMaterial;
    public float pulseDuration = 3f;
    public float pulseSpeed = 5f;
    public float minEmission = 0f;
    public float maxEmission = 10f;

    public List<Rigidbody> orbitingBodies = new List<Rigidbody>();
    public Transform orbitCenter;
    public float orbitRadius = 3f;
    public float orbitSpeed = 50f;
    
    private void OnTriggerEnter(Collider other)
    {
        var rb = other.GetComponent<Rigidbody>();
        if (rb)
            StartCoroutine(DelayAdd(rb));
        
        StartCoroutine(PulseEmission());
        OnTrigger?.Invoke();
    }

    private IEnumerator DelayAdd(Rigidbody rb)
    {
        yield return new WaitForSeconds(4f);
        orbitingBodies.Add(rb);
       
    }

    private IEnumerator PulseEmission()
    {
        float elapsed = 0f;

        targetMaterial.EnableKeyword("_EMISSION");

        while (elapsed < pulseDuration)
        {
            elapsed += Time.deltaTime;

            float sineValue = Mathf.Sin(Time.time * pulseSpeed);
            float emissionValue = Mathf.Lerp(minEmission, maxEmission, (sineValue + 1f) / 2f);

            targetMaterial.SetColor("_EmissionColor", Color.white * emissionValue);

            yield return null;
        }

        targetMaterial.SetColor("_EmissionColor", Color.black);
    }

    private void Update()
    {
        float angleOffset = 360f / Mathf.Max(1, orbitingBodies.Count);

        for (int i = 0; i < orbitingBodies.Count; i++)
        {
            Rigidbody rb = orbitingBodies[i];
            if (rb == null) continue;

            float angle = (Time.time * orbitSpeed) + (i * angleOffset);
            float rad = -angle * Mathf.Deg2Rad;

            Vector3 offset = new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad)) * orbitRadius;
            Vector3 targetPos = orbitCenter.position + offset;

            rb.MovePosition(Vector3.Lerp(rb.position, targetPos, Time.deltaTime * 2f));
        }
    }
}
