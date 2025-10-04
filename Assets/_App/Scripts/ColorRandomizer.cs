using UnityEngine;

public class ColorRandomizer : MonoBehaviour
{
    private static readonly int Color1 = Shader.PropertyToID("_Color");
    private static readonly int EmColor = Shader.PropertyToID("_EmColor"); // Make sure your shader uses this
    private static readonly int FresnelPower = Shader.PropertyToID("_FresnelPower"); // Shader property for Fresnel

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();

        // Random pastel/vibrant base color
        float h = Random.value;                       
        float s = Random.Range(0.5f, 0.8f);           
        float v = Random.Range(0.9f, 1f);             
        Color pastelVibrant = Color.HSVToRGB(h, s, v);

        // Apply base color
        renderer.material.SetColor(Color1, pastelVibrant);

        // 20% chance to apply emission + random FresnelPower
        if (Random.value <= 0.2f)
        {
            // Random emission color
            float eh = Random.value;                       
            float es = Random.Range(0.5f, 1f);            
            float ev = Random.Range(0.5f, 1f);            
            Color emissionColor = Color.HSVToRGB(eh, es, ev) * 2f; // HDR intensity always 2

            renderer.material.EnableKeyword("_EMISSION"); // Enable emission
            renderer.material.SetColor(EmColor, emissionColor);

            // Random FresnelPower between 1 and 10
            float fresnel = Random.Range(0.1f, 5f);
            renderer.material.SetFloat(FresnelPower, fresnel);
        }
    }
}