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
    public const string TAKE_N_DROP = "Take_Drop";

    /// <summary>
    /// Use.
    /// </summary>
    public const string USE = "Use";

    /// <summary>
    /// Pause.
    /// </summary>
    public const string PAUSE = "Pause";

    public const string SELECT = "Select";

    public const string BACK = "Back";

    public static string GetActionName(int id, string inputName, bool fromController)
    {
        return (id != -1 ? $"{id}_" : "") + inputName + (fromController ? "_Controller" : "_Keyboard");
    }

    public static bool GetUserActionDown<TEnum>(Dictionary<TEnum, string> bindings, TEnum action, int id)
        where TEnum : System.Enum
    {
        string inputName;

        if (!bindings.TryGetValue(action, out inputName))
            return false;

        if (Input.GetButtonDown(GetActionName(id, inputName, false)))
            return true;

        return Input.GetButtonDown(GetActionName(id, inputName, true));
    }

    public static bool GetUserActionUp<TEnum>(Dictionary<TEnum, string> bindings, TEnum action, int id)
        where TEnum : System.Enum
    {
        string inputName;

        if (!bindings.TryGetValue(action, out inputName))
            return false;

        if (Input.GetButtonUp(GetActionName(id, inputName, false)))
            return true;

        return Input.GetButtonUp(GetActionName(id, inputName, true));
    }

    public static float GetUserActionValue<TEnum>(Dictionary<TEnum, string> bindings, TEnum action, int id)
        where TEnum : System.Enum
    {
        string inputName;
        float result;

        if (!bindings.TryGetValue(action, out inputName))
            return 0.0f;

        result = Input.GetAxis(GetActionName(id, inputName, false));

        if (result != 0.0f)
            return result;

        return Input.GetAxis(GetActionName(id, inputName, true));
    }
}
