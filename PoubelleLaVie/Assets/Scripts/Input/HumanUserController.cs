using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HumanUserController : IController<HumanAction>
{
    public bool GetActionDown(HumanAction action)
    {
        return false;
    }

    public bool GetActionUp(HumanAction action)
    {
        return false;
    }

    public float GetActionValue(HumanAction action)
    {
        if (action == HumanAction.Horizontal)
            return Input.GetAxis(InputHelper.HORIZONTAL);

        if (action == HumanAction.Vertical)
            return Input.GetAxis(InputHelper.VERTICAL);

        return 0.0f;
    }
}
