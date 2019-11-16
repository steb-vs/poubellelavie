using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDataComponent : HumanDataComponent
{
    public NPCState npcState;

    public DrunkType drunkType;

    public bool grabbed;

    public float fallDuration;

    public float fallTime;

    public bool falling;

    public NPCDataComponent()
    {
        npcState = NPCState.Fine;
        drunkType = DrunkType.Dancer;
        grabbed = false;
        fallDuration = 2.0f;
        fallTime = 0.0f;
        falling = false;
    }
}
