using System.Collections.Generic;
using UnityEngine;

public class MachinesController : MonoBehaviour
{
    public EffectsController Effects;
    public GameObject ClawMachineGO;

    public List<GameObject> ClawMachines = new List<GameObject>();

    private int previousClawsCount;

    private void Start()
    {
        previousClawsCount = Effects.ClawsCount;
        for (int i = 0; i < previousClawsCount; i++)
        {
            SpawnClaw(i);
        }
    }

    private void Update()
    {
        if (Effects.ClawsCount != previousClawsCount)
        {
            int difference = Effects.ClawsCount - previousClawsCount;

            if (difference > 0)
            {
                for (int i = 0; i < difference; i++)
                    SpawnClaw(previousClawsCount + i);
            }
            else
            {
                for (int i = 0; i < -difference; i++)
                    DespawnClaw();
            }

            previousClawsCount = Effects.ClawsCount;
        }
    }

    private void SpawnClaw(int index)
    {
        var go = Instantiate(ClawMachineGO, transform);
        var clawComponent = go.GetComponent<ClawMachine>();

        if (clawComponent != null)
        {
            int n = Effects.ClawsCount;
            float spacing = 0.25f;

            float xOffset = 0.5f + (index - (n - 1) / 2f) * spacing;
            float yOffset = 0.5f; 

            clawComponent.CenterPoint = new Vector3(xOffset, yOffset, 0f);
            clawComponent.AI = index > 0;
        }

        ClawMachines.Add(go);
    }


    private void DespawnClaw()
    {
        if (ClawMachines.Count == 0) return;

        var lastClaw = ClawMachines[ClawMachines.Count - 1];
        ClawMachines.RemoveAt(ClawMachines.Count - 1);
        Destroy(lastClaw);
    }
}