using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIUserController : Controller<UIAction>
{
    private static readonly Dictionary<UIAction, string> BINDINGS = new Dictionary<UIAction, string>
    {
        { UIAction.Back, InputHelper.BACK },
        { UIAction.Select, InputHelper.SELECT },
        { UIAction.Horizontal, InputHelper.HORIZONTAL },
        { UIAction.Vertical, InputHelper.VERTICAL }
    };

    public override bool GetActionDown(UIAction action)
    {
        return InputHelper.GetUserActionDown(BINDINGS, action, 1);
    }

    public override bool GetActionUp(UIAction action)
    {
        return InputHelper.GetUserActionUp(BINDINGS, action, 1);
    }

    public override float GetActionValue(UIAction action)
    {
        return InputHelper.GetUserActionValue(BINDINGS, action, 1);
    }

    public override void UpdateAI()
    {
    }
}
