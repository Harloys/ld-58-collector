using System;
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
        transform.localScale = baseScale + new Vector3(scale,scale, scale);
    }
}
