using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static int Count<TEnum>()
        where TEnum : System.Enum
    {
        return System.Enum.GetNames(typeof(TEnum)).Length;
    }

    public static TEnum Random<TEnum>()
        where TEnum : System.Enum
    {
        return (TEnum)(object)UnityEngine.Random.Range(0, Count<TEnum>());
    }
}
