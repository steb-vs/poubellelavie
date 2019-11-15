using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUserController : HumanUserController, IController<PlayerAction>
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
}
