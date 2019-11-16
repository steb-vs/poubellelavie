using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAIController : Controller<HumanAction>, IController<PlayerAction>
{
    public override bool GetActionDown(HumanAction action)
    {
        throw new System.NotImplementedException();
    }

    public bool GetActionDown(PlayerAction action)
    {
        throw new System.NotImplementedException();
    }

    public override bool GetActionUp(HumanAction action)
    {
        throw new System.NotImplementedException();
    }

    public bool GetActionUp(PlayerAction action)
    {
        throw new System.NotImplementedException();
    }

    public override float GetActionValue(HumanAction action)
    {
        throw new System.NotImplementedException();
    }

    public float GetActionValue(PlayerAction action)
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateAI()
    {
        throw new System.NotImplementedException();
    }
}
