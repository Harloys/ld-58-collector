using UnityEngine;

public class ColorRandomizer : MonoBehaviour
{
    private static readonly int Color1 = Shader.PropertyToID("_Color");

    void Start()
    {
        var h = Random.value;                    // any hue
        var s = Random.Range(0.5f, 0.8f);        // medium-high saturation
        var v = Random.Range(0.9f, 1f);          // high brightness for pastel look

        var pastelVibrant = Color.HSVToRGB(h, s, v);
        GetComponent<Renderer>().material.SetColor(Color1, pastelVibrant);

    }
}
