using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    public GameObject Ball;
    public int SpawnCount = 20;
    public int SpawnCountFinal = 100;

    public float SpawnRadius = 0.5f;
    
    void Start()
    {
        Spawn(SpawnCount);
    }

    public void SpawnFinal()
    {
        Spawn(SpawnCountFinal);
    }

    private void Update()
    {
        if(Application.isEditor && Input.GetKeyDown(KeyCode.P))
            Spawn(SpawnCount);
    }

    public void Spawn(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var go = Instantiate(Ball, transform.position, quaternion.identity);
            go.transform.localPosition += new Vector3(Random.Range(-SpawnRadius, SpawnRadius),
                Random.Range(-SpawnRadius, SpawnRadius), Random.Range(-SpawnRadius, SpawnRadius));

            if (!go.TryGetComponent(out ColorRandomizer colorRandomizer))
                continue;
            
            var h = Random.value;                       
            var s = Random.Range(0.5f, 0.8f);           
            var v = Random.Range(0.9f, 1f);             
            var pastelVibrant = Color.HSVToRGB(h, s, v);

            colorRandomizer.SetColor(pastelVibrant);

            if (!(Random.value <= 0.2f)) 
                continue;
            
            var eh = Random.value;                       
            var es = Random.Range(0.5f, 1f);            
            var ev = Random.Range(0.5f, 1f);            
            var emissionColor = Color.HSVToRGB(eh, es, ev) * 2f;

            colorRandomizer.SetEmission(emissionColor);
                    
            var fresnel = Random.Range(0.1f, 5f);
            colorRandomizer.SetFresnel(fresnel);

        }        
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, SpawnRadius);
    }
}
