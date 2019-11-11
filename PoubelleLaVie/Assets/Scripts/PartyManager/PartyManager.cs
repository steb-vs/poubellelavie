using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    public GameObject npc;
    public int[] entriesMass;
    
    void Start()
    {
        int totalMass = 0;
        probEntries = new float[entriesMass.Length];

        foreach (int mass in entriesMass)
            totalMass += mass;

        probEntries[0] = (float) entriesMass[0] / totalMass; // Init prob[0]
        for (int i = 1; i < probEntries.Length; i++)
            probEntries[i] = (float) entriesMass[i] / totalMass + probEntries[i-1];
    }

    void Update()
    {
        if (timer <= 0.0f)
        {
            float chooseEntry = Random.Range(0.0F, 1.0F);
            int entryNumber = 0;

            while (probEntries[entryNumber] < chooseEntry)
                entryNumber++;

            var entry = PathfinderHelper.Pathfinder.entries[entryNumber];
            Vector3 spawnPos = new Vector3(entry.gridX, entry.gridY + 1, 0);
            GameObject.Instantiate(npc, spawnPos, Quaternion.identity);

            timer = timeToPop;
        }

        timer -= Time.deltaTime * GameHelper.GM.timeScale;
    }

    private float timer = 0.0f;
    private float timeToPop = 1.0F;
    private float[] probEntries; // Histogram of probabilities
}
