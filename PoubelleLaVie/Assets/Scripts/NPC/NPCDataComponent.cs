using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDataComponent : HumanDataComponent
{
    public NPCState npcState;

    public DrunkType drunkType;

    public bool isCarried;

    public float fallDuration;

    public float fallTime;

    public bool falling;
}
