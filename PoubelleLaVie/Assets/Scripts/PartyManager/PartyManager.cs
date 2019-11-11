using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    public GameObject npc;
    
    void Start()
    {
        
    }

    void Update()
    {
        if (timer <= 0.0f)
        {
            timer = timeToPop;
            var entry = PathfinderHelper.Pathfinder.entries[0];
            Vector3 spawnPos = new Vector3(entry.gridX, entry.gridY + 1, 0);
            GameObject.Instantiate(npc, spawnPos, Quaternion.identity);
        }

        timer -= Time.deltaTime * GameHelper.GM.timeScale;
    }

    private float timer = 0.0f;
    private float timeToPop = 3.0f;
}
