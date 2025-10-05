using System;
using Unity.Mathematics;
using UnityEngine;

public class BallScaler : MonoBehaviour
{
    public float ScaleInc = 0.1f;

    private Vector3 baseScale;
    
    private void Awake()
    {
        baseScale = transform.localScale;
    }

    private void Update()
    {
        var scale = (EffectsController.Instance.BallsScaleMult * ScaleInc);
        scale = math.clamp(scale, 0, 0.4f);
        transform.localScale = baseScale + new Vector3(scale,scale, scale);
    }
}
