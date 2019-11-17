using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUserController : Controller<HumanAction>, IController<PlayerAction>
{
    private static readonly Dictionary<HumanAction, string> HUMAN_BINDINGS = new Dictionary<HumanAction, string>
    {
        { HumanAction.Horizontal, InputHelper.HORIZONTAL },
        { HumanAction.Vertical, InputHelper.VERTICAL }
    };

    private static readonly Dictionary<PlayerAction, string> PLAYER_BINDINGS = new Dictionary<PlayerAction, string>
    {
        { PlayerAction.TakeDrop, InputHelper.TAKE_N_DROP },
        { PlayerAction.Use, InputHelper.USE }
    };

    public override bool GetActionDown(HumanAction action)
    {
        return InputHelper.GetUserActionDown(HUMAN_BINDINGS, action);
    }

    public bool GetActionDown(PlayerAction action)
    {
        return InputHelper.GetUserActionDown(PLAYER_BINDINGS, action);
    }

    public override bool GetActionUp(HumanAction action)
    {
        return InputHelper.GetUserActionUp(HUMAN_BINDINGS, action);
    }

    public bool GetActionUp(PlayerAction action)
    {
        return InputHelper.GetUserActionUp(PLAYER_BINDINGS, action);
    }

    public override float GetActionValue(HumanAction action)
    {
        return InputHelper.GetUserActionValue(HUMAN_BINDINGS, action);
    }

    public float GetActionValue(PlayerAction action)
    {
        return InputHelper.GetUserActionValue(PLAYER_BINDINGS, action);
    }

    public override void UpdateAI()
    {
    }
}
