using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IController
{
    void UpdateAI();
}

public interface IController<TEnum> : IController
    where TEnum : System.Enum
{
    float GetActionValue(TEnum action);

    bool GetActionDown(TEnum action);

    bool GetActionUp(TEnum action);
}
