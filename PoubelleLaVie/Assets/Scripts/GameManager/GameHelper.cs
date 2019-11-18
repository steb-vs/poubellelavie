using System.Collections.Generic;
using UnityEngine;

public static class GameHelper
{
    public static GameManager GameManager { get; set; }

    public static List<GameObject> Players { get; set; }

    public static GameSettings Settings { get; private set; }

    static GameHelper()
    {
        Settings = new GameSettings
        {
            playerCount = 2
        };
    }
}
