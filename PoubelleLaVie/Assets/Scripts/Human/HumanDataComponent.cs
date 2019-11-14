using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HumanDataComponent : MonoBehaviour
{
    public float defaultSpeed;

    /// <summary>
    /// Gets or sets the speed.
    /// </summary>
    public float speed;

    /// <summary>
    /// Gets or sets the direction.
    /// </summary>
    public Vector2 direction;

    /// <summary>
    /// Gets or sets the position.
    /// </summary>
    public Vector2 position;

    /// <summary>
    /// Gets or sets the move state.
    /// </summary>
    public PlayerMoveState moveState;

    public HumanDataComponent()
    {
        direction = new Vector2(1, 0);
        speed = 150;
        defaultSpeed = 150;
        position = new Vector2(0, 0);
        moveState = PlayerMoveState.Idle;
    }
}
