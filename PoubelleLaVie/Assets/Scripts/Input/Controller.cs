using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Controller<TEnum> : MonoBehaviour, IController<TEnum>
    where TEnum : System.Enum
{
    public int id;

    public Controller()
    {
        id = -1;
    }

    public abstract bool GetActionDown(TEnum action);

    public abstract bool GetActionUp(TEnum action);

    public abstract float GetActionValue(TEnum action);

    public abstract void UpdateAI();
}
