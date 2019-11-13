using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IController<TEnum>
    where TEnum : System.Enum
{
    float GetActionValue(TEnum action);
    bool GetActionDown(TEnum action);
    bool GetActionUp(TEnum action);
}
