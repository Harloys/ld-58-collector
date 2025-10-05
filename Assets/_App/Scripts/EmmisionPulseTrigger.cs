using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class EmissionPulseTrigger : MonoBehaviour
{
    public UnityEvent OnTrigger;
    
    public Material targetMaterial;
    public float pulseDuration = 3f;
    public float pulseSpeed = 5f;
    public float minEmission = 0f;
    public float maxEmission = 10f;

    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(PulseEmission());
        OnTrigger?.Invoke();
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
}