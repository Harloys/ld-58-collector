using System;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    public GameObject Ball;
    public int SpawnCount = 20;
    public float SpawnRadius = 0.5f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Spawn();
    }

    private void Update()
    {
        if(Application.isEditor && Input.GetKeyDown(KeyCode.P))
            Spawn();
    }

    [ContextMenu("Spawn")]
    public void Spawn()
    {
        for (int i = 0; i < SpawnCount; i++)
        {
            var pos = Instantiate(Ball, transform.position, quaternion.identity);
            pos.transform.localPosition += new Vector3(Random.Range(-SpawnRadius, SpawnRadius),
                Random.Range(-SpawnRadius, SpawnRadius), Random.Range(-SpawnRadius, SpawnRadius));
        }        
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, SpawnRadius);
    }
}
