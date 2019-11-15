using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUserController : Controller<HumanAction>, IController<PlayerAction>
{
    public bool GetActionDown(PlayerAction action)
    {
        throw new System.NotImplementedException();
    }

    public bool GetActionUp(PlayerAction action)
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
