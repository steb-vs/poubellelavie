using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class InputHelper
{
    /// <summary>
    /// X axis.
    /// </summary>
    public const string HORIZONTAL = "Horizontal";

    /// <summary>
    /// Y axis.
    /// </summary>
    public const string VERTICAL = "Vertical";

    /// <summary>
    /// Take or drop.
    /// </summary>
    public const string TAKE_N_DROP = "Take / Drop";

    /// <summary>
    /// Use.
    /// </summary>
    public const string USE = "Use";

    /// <summary>
    /// Pause.
    /// </summary>
    public const string PAUSE = "Pause";

    public static bool GetUserActionDown<TEnum>(Dictionary<TEnum, string> bindings, TEnum action)
        where TEnum : System.Enum
    {
        string inputName;

        if (!bindings.TryGetValue(action, out inputName))
            return false;

        return Input.GetButtonDown(inputName);
    }

    public static bool GetUserActionUp<TEnum>(Dictionary<TEnum, string> bindings, TEnum action)
        where TEnum : System.Enum
    {
        string inputName;

        if (!bindings.TryGetValue(action, out inputName))
            return false;

        return Input.GetButtonUp(inputName);
    }

    public static float GetUserActionValue<TEnum>(Dictionary<TEnum, string> bindings, TEnum action)
        where TEnum : System.Enum
    {
        string inputName;

        if (!bindings.TryGetValue(action, out inputName))
            return 0.0f;

        return Input.GetAxis(inputName);
    }
}
