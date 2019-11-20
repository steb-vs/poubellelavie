using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataComponent : HumanDataComponent
{
    /// <summary>
    /// Gets or sets the action state.
    /// </summary>
    public PlayerActionState actionState;

    public IUsable usableObject;

    public ISpeedModifier speedModifierObject;

    public int trashCount;

    public HashSet<WindowComponent> closeWindows;

    public int id;

    public PlayerDataComponent() : base()
    {
        actionState = PlayerActionState.Default;
        usableObject = null;
        speedModifierObject = null;
        trashCount = 0;
        closeWindows = new HashSet<WindowComponent>();
        id = -1;
    }
}
