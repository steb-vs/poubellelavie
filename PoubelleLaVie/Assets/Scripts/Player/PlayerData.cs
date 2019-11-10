using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
    /// <summary>
    /// Gets or sets the speed.
    /// </summary>
    public float Speed { get; set; }

    /// <summary>
    /// Gets or sets the direction.
    /// </summary>
    public Vector2 Direction { get; set; }

    /// <summary>
    /// Gets or sets the position.
    /// </summary>
    public Vector2 Position { get; set; }

    /// <summary>
    /// Gets or sets the move state.
    /// </summary>
    public PlayerMoveState MoveState { get; set; }

    /// <summary>
    /// Gets or sets the action state.
    /// </summary>
    public PlayerActionState ActionState { get; set; }

    /// <summary>
    /// Gets or sets the carried object.
    /// </summary>
    public IUsable CarriedObject { get; set; }

    /// <summary>
    /// Returns default player data.
    /// </summary>
    public static PlayerData Default => new PlayerData
    {
        Direction = new Vector2(1, 0),
        Speed = 100,
        Position = new Vector2(0, 0),
        MoveState = PlayerMoveState.Idle,
        ActionState = PlayerActionState.Default,
        CarriedObject = null
    };
}
