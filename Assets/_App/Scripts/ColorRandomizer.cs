using UnityEngine;

public class ColorRandomizer : MonoBehaviour
{
    private static readonly int Color1 = Shader.PropertyToID("_Color");
    private static readonly int EmColor = Shader.PropertyToID("_EmColor"); // Make sure your shader uses this
    private static readonly int FresnelPower = Shader.PropertyToID("_FresnelPower"); // Shader property for Fresnel
    private Renderer renderer;
    void Awake()
    {
        renderer = GetComponent<Renderer>();
    }

    public void SetColor(Color value)
    {
        renderer.material.SetColor(Color1, value);
    }
    
    
    public void SetEmission(Color value)
    {
        renderer.material.EnableKeyword("_EMISSION");
        renderer.material.SetColor(EmColor, value);
    }
    
    
    public void SetFresnel(float value)
    {
        renderer.material.SetFloat(FresnelPower, value);
    }
}