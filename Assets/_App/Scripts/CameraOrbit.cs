using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    public Transform target;
    public float distance = 10.0f;
    public float mouseSensitivity = 5.0f;

    public float MaxClamp = 80;
    public float MinClamp = -80;
    
    private float currentX = 0.0f;
    private float currentY = 0.0f;

    void Start()
    {
        var angles = transform.eulerAngles;
        currentX = angles.y;
        currentY = angles.x;
    }

    void Update()
    {
        if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
        {
            currentX += Input.GetAxis("Mouse X") * mouseSensitivity;
            currentY -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        }
        
        currentY = Mathf.Clamp(currentY, MinClamp, MaxClamp);
    }

    void LateUpdate()
    {
        if (target == null) 
            return;

        var rotation = Quaternion.Euler(currentY, currentX, 0);
        var newPos = target.position - (rotation * Vector3.forward * distance);

        transform.position = newPos;
        transform.rotation = rotation;
    }
}
