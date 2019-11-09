using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

public class PlayerComponent : MonoBehaviour
{
    public GameObject playerSprite;

    private PlayerData _data;

    private Rigidbody2D _body;

    private Animator _animator;

    private HashSet<IUsable> _closeObjects;

    private Animation _animation;

    /// <summary>
    /// Sets the player data.
    /// </summary>
    /// <param name="data"></param>
    public void SetData(PlayerData data)
    {
        _data = data;
        transform.position = data.Position;
    }

    private void Start()
    {
        _body = GetComponent<Rigidbody2D>();
        _animator = playerSprite.GetComponent<Animator>();
        _data = PlayerData.Default;
        _closeObjects = new HashSet<IUsable>();
    }

    private void Update()
    {
        ReadInput();
        Move();
    }

    /// <summary>
    /// Processes the user input.
    /// </summary>
    private void ReadInput()
    {
        // Get the character direction
        _data.Direction = new Vector2(
            Input.GetAxis(InputHelper.HORIZONTAL),
            Input.GetAxis(InputHelper.VERTICAL)
        );

        // Handle take / drop action
        if(Input.GetButtonDown(InputHelper.TAKE_N_DROP))
        {
            // If we are holding an object
            if (_data.CarriedObject != null)
            {
                // Drop it!
                _data.CarriedObject.Drop(gameObject);
                _data.CarriedObject = null;
            }

            // Else, get a new object if any available
            else if(_closeObjects.Any())
            {
                // Find the closest object to the player
                _data.CarriedObject = _closeObjects
                    .Select(x => new { Obj = x, Distance = (x.Position - transform.position).magnitude })
                    .OrderBy(x => x.Distance)
                    .First().Obj;

                _data.CarriedObject.Take(gameObject);
            }
        }

        // Handle use action
        if (Input.GetButtonDown(InputHelper.USE) && _data.CarriedObject != null)
            _data.CarriedObject.Use(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the object is usable
        IUsable usable = collision.gameObject.GetComponent<IUsable>();

        if (usable == null)
            return;

        // Yes, register it!
        _closeObjects.Add(usable);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Check if the object is usable
        IUsable usable = collision.gameObject.GetComponent<IUsable>();

        if (usable == null)
            return;

        // Yes, remove it!
        _closeObjects.Remove(usable);
    }

    /// <summary>
    /// Moves the player with the current data.
    /// </summary>
    private void Move()
    {
        bool updateAnimation = false;

        // Add force to the rigid body
        _body.AddForce(_data.Direction * _data.Speed);

        if(_data.Direction.magnitude > 0.01f)
            playerSprite.transform.localRotation = Quaternion.FromToRotation(Vector2.up, _data.Direction.normalized);

        // Update the move state
        if (_body.velocity.magnitude > 0.1f)
            _data.MoveState = PlayerMoveState.Run;
        else
            _data.MoveState = PlayerMoveState.Idle;

        // Update the action state
        if (_data.CarriedObject != null)
        {
            if (_data.CarriedObject.IsHeavy)
            {
                _data.Speed = 60;
                _data.ActionState = PlayerActionState.Grabbing;
            }
            else
                _data.ActionState = PlayerActionState.Holding;
        }
        else
        {
            _data.ActionState = PlayerActionState.Default;
            _data.Speed = 100;
        }

        // Update the animator parameters
        if (_animator.GetInteger(PlayerHelper.ANIMATOR_ACTION_PARAM_NAME) != (int)_data.ActionState)
        {
            _animator.SetInteger(PlayerHelper.ANIMATOR_ACTION_PARAM_NAME, (int)_data.ActionState);
            updateAnimation = true;
        }

        if (_animator.GetInteger(PlayerHelper.ANIMATOR_MOVE_PARAM_NAME) != (int)_data.MoveState)
        {
            _animator.SetInteger(PlayerHelper.ANIMATOR_MOVE_PARAM_NAME, (int)_data.MoveState);
            updateAnimation = true;
        }

        if(updateAnimation)
        {
            string newAnimationName = _data.ActionState.ToString() + _data.MoveState.ToString();
            _animator.StopPlayback();
            _animator.Play(newAnimationName);
        }

        _animator.speed = 0.25f + (_body.velocity.magnitude / 6.0f);

        _data.Position = transform.position;
    }
}
