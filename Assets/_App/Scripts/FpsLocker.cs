using UnityEngine;

public class FpsLocker : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Application.targetFrameRate = -1;
        QualitySettings.vSyncCount = 1;
    }
}
