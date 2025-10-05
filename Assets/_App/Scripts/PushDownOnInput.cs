using System;
using UnityEngine;

public class PushDownOnInput : MonoBehaviour
{
    public float pushAmount = 0.1f;
    public KeyCode Code = KeyCode.W;

    private Vector3 initPos;
    private void Awake()
    {
        initPos = transform.localPosition;
    }

    void Update()
    {
        if (Input.GetKey(Code))
        {
            transform.localPosition = initPos - new Vector3(0, pushAmount, 0);
        }
        else
        {
            transform.localPosition = initPos;
        }
    }
}