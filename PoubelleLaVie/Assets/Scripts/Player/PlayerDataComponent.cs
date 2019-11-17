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

    public int trashLimit;

    public HashSet<WindowComponent> closeWindows;

    public PlayerDataComponent() : base()
    {
        actionState = PlayerActionState.Default;
        usableObject = null;
        speedModifierObject = null;
        trashCount = 0;
        trashLimit = 10;
        closeWindows = new HashSet<WindowComponent>();
    }
}
