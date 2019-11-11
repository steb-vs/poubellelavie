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
            Debug.Log($"OSIUEJFIOUZHEFIOHJ {PathfinderHelper.Pathfinder.entries.Count} -- {Random.Range(0, PathfinderHelper.Pathfinder.entries.Count)}");
            var entry = PathfinderHelper.Pathfinder.entries[Random.Range(0, PathfinderHelper.Pathfinder.entries.Count)];
            Vector3 spawnPos = new Vector3(entry.gridX, entry.gridY + 1, 0);
            GameObject.Instantiate(npc, spawnPos, Quaternion.identity);
        }

        timer -= Time.deltaTime * GameHelper.GM.timeScale;
    }

    private float timer = 0.0f;
    private float timeToPop = 10.0f;
}
