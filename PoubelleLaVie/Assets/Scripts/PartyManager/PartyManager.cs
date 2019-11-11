using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    public GameObject npc;
    public int[] entriesMass;
    public float startingTimeToPop = 10.0F;
    public float endingTimeToPop = 2.0F;
    public float timeToEnd = 180.0F;
    
    void Start()
    {
        // Init histogram
        int totalMass = 0;
        probEntries = new float[entriesMass.Length];

        foreach (int mass in entriesMass)
            totalMass += mass;

        probEntries[0] = (float)entriesMass[0] / totalMass; // Init prob[0]
        for (int i = 1; i < probEntries.Length; i++)
            probEntries[i] = (float)entriesMass[i] / totalMass + probEntries[i - 1];

        // Init lastNPCStates
        lastNPCStates = new DrunkState[4]; // Save of the last 4 NPC
        for (int i = 0; i < lastNPCStates.Length; i++)
            lastNPCStates[i] = DrunkState.DANCER;

        // Calcul the coeff to adjust the function
        coeff = (endingTimeToPop - startingTimeToPop) / timeToEnd;
    }

    void Update()
    {
        timer -= Time.deltaTime * GameHelper.GM.timeScale;
        t += Time.deltaTime * GameHelper.GM.timeScale;


        if (timer <= 0.0f) // Spawn a new NPC
        {
            // Choose a new entry, following the histogram
            float chooseEntry = Random.Range(0.0F, 1.0F);
            int entryNumber = 0;

            while (probEntries[entryNumber] < chooseEntry)
                entryNumber++;

            var entry = PathfinderHelper.Pathfinder.entries[entryNumber];
            Vector3 spawnPos = new Vector3(entry.gridX, entry.gridY + 1, 0);
            GameObject newNPC = GameObject.Instantiate(npc, spawnPos, Quaternion.identity);

            // Check if a dancer has spawn in the last 4 NPC
            bool dancerFound = false;
            foreach (DrunkState state in lastNPCStates)
                if (state == DrunkState.DANCER)
                {
                    dancerFound = true;
                    break;
                }

            if (!dancerFound) // If not, modify the drunkType to a DANCER
                newNPC.GetComponent<NPCBehaviour>().drunkType = DrunkState.DANCER;

            for (int i = 1; i < lastNPCStates.Length; i++)
                lastNPCStates[i - 1] = lastNPCStates[i];
            lastNPCStates[lastNPCStates.Length - 1] = newNPC.GetComponent<NPCBehaviour>().drunkType;

            // Set timer
            timeToPop = t * coeff + startingTimeToPop;
            timeToPop = timeToPop > startingTimeToPop ? startingTimeToPop : timeToPop; // Does not come across startingTimeToStop
            timeToPop = timeToPop < endingTimeToPop ? endingTimeToPop : timeToPop;

            timer = timeToPop;
        }
    }

    private float timer = 0.0f;
    private float t = 0.0F;
    private float coeff;
    private float timeToPop = 5.0F;
    private float[] probEntries; // Histogram of probabilities
    private DrunkState[] lastNPCStates;
}
