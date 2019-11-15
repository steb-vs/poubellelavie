using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HumanAIController : IController<HumanAction>
{
    public bool GetActionDown(HumanAction action)
    {
        throw new System.NotImplementedException();
    }

    public bool GetActionUp(HumanAction action)
    {
        throw new System.NotImplementedException();
    }

    public float GetActionValue(HumanAction action)
    {
        throw new System.NotImplementedException();
    }
}
