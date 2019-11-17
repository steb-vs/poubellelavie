using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDataComponent : HumanDataComponent
{
    public Color loverColor;

    public Color dancerColor;

    public Color pukerColor;

    public NPCState npcState;

    public DrunkType drunkType;

    public bool grabbed;

    public float fallDuration;

    public float fallTime;

    public bool falling;

    public WorldTile tile;

    public NPCDataComponent()
    {
        npcState = NPCState.Fine;
        drunkType = DrunkType.Dancer;
        grabbed = false;
        fallDuration = 1.0f;
        fallTime = 0.0f;
        falling = false;
        tile = null;
        pukerColor = new Color(0, 0.5f, 0); 
        dancerColor = new Color(1, 0.25f, 0); 
        loverColor = new Color(1, 0.5f, 1);
    }
}
