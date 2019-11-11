﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerComponent : MonoBehaviour
{
    public GameObject playerSprite;
    [HideInInspector] public Window closeWindow = null;
    [HideInInspector] public int grabbedObjects = 0;
    public int maxGrabbedObjects = 10;

    public delegate void TrashThrownHandler(GameObject obj, PlayerComponent playerComponent, int garbageCount);
    public delegate void UsableHandler(GameObject obj, PlayerComponent playerComponent, IUsable usable);

    public event TrashThrownHandler OnTrashTrown;
    public event UsableHandler OnObjectTaken;
    public event UsableHandler OnObjectDropped;

    public PlayerData Data { get; private set; }
    public ISpeedModifier SpeedModObj { get; private set; }

    private Rigidbody2D _body;
    private Animator _animator;
    private HashSet<IUsable> _closeObjects;
    private IUsable _carriedObject;

    /// <summary>
    /// Sets the player data.
    /// </summary>
    /// <param name="data"></param>
    public void SetData(PlayerData data)
    {
        Data = data;
        transform.position = data.Position;
    }

    private void Start()
    {
        _body = GetComponent<Rigidbody2D>();
        _animator = playerSprite.GetComponent<Animator>();
        Data = PlayerData.Default;
        _closeObjects = new HashSet<IUsable>();

        if (GameHelper.GM != null)
        {
            GameHelper.GM.player = this.gameObject;
            GameHelper.GM.playerComponent = this;
        }
    }

    private void FixedUpdate()
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
        Data.Direction = new Vector2(
            Input.GetAxis(InputHelper.HORIZONTAL),
            Input.GetAxis(InputHelper.VERTICAL)
        );

        // Handle take / drop action
        if(Input.GetButtonDown(InputHelper.TAKE_N_DROP))
        {
            // If we are holding an object
            if (_carriedObject != null)
            {
                OnObjectDropped?.Invoke(gameObject, this, _carriedObject);

                // Drop it!
                if(_carriedObject.Drop(gameObject))
                    _carriedObject = null;
            }
            
            else if (grabbedObjects > 0 && closeWindow != null)
            {
                OnTrashTrown?.Invoke(gameObject, this, grabbedObjects);

                grabbedObjects = 0;
                var t = GameObject.Instantiate(GameHelper.GM.thrownTrash, transform.position +
                                                                          closeWindow.transform.up * 2,
                    Quaternion.identity);
                GameObject.Destroy(t, 1);
            }

            // Else, get a new object if any available
            else if(_closeObjects.Any())
            {
                // Find the closest object to the player
                _carriedObject = _closeObjects
                    .Select(x => new { Obj = x, Distance = (x.Position - transform.position).magnitude })
                    .OrderBy(x => x.Distance)
                    .First().Obj;

                OnObjectTaken?.Invoke(gameObject, this, _carriedObject);

                if (!_carriedObject.Take(gameObject))
                    _carriedObject = null;
            }
        }

        // Handle use action
        if (Input.GetButtonDown(InputHelper.USE) && _carriedObject != null)
            _carriedObject.Use(gameObject);
    }

    private void RegisterUsableObject(GameObject obj)
    {
        // Check if the object is usable
        IUsable usable = obj.GetComponent<IUsable>();

        if (usable == null)
            return;

        // Yes, register it!
        _closeObjects.Add(usable);
    }

    private void UnregisterUsableObject(GameObject obj)
    {
        // Check if the object is usable
        IUsable usable = obj.GetComponent<IUsable>();

        if (usable == null)
            return;

        // Yes, remove it!
        _closeObjects.Remove(usable);
    }

    private void UpdateSpeedMod(GameObject obj)
    {
        ISpeedModifier speedModObj = obj.GetComponent<ISpeedModifier>();

        if (speedModObj == null)
            return;

        // Only update speed mod if the new speed mod is lesser than the current
        if (SpeedModObj != null && SpeedModObj.SpeedModifier < speedModObj.SpeedModifier)
            return;

        SpeedModObj = speedModObj;
    }

    private void RestoreSpeedMod(GameObject obj)
    {
        ISpeedModifier speedModObj = obj.GetComponent<ISpeedModifier>();

        if (speedModObj == null)
            return;
        
        // Skip if the speed mod object is not the same as the current
        if (SpeedModObj != speedModObj)
            return;

        SpeedModObj = null;
    }

    private void OnCollisionEnter2D(Collision2D collision) => RegisterUsableObject(collision.gameObject);

    private void OnCollisionExit2D(Collision2D collision) => UnregisterUsableObject(collision.gameObject);

    private void OnTriggerEnter2D(Collider2D collision)
    {
        RegisterUsableObject(collision.gameObject);
        UpdateSpeedMod(collision.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        UnregisterUsableObject(collision.gameObject);
        RestoreSpeedMod(collision.gameObject);
    }

    /// <summary>
    /// Moves the player with the current data.
    /// </summary>
    private void Move()
    {
        bool updateAnimation = false;

        // Update the action state
        if (_carriedObject != null)
        {
            if (_carriedObject.IsHeavy)
            {
                Data.Speed = Data.DefaultSpeed * 0.6f * (SpeedModObj != null ? SpeedModObj.SpeedModifier : 1) * GameHelper.GM.timeScale;
                Data.ActionState = PlayerActionState.Grabbing;
            }
            else
            {
                Data.Speed = Data.DefaultSpeed * (SpeedModObj != null ? SpeedModObj.SpeedModifier : 1) * GameHelper.GM.timeScale;
                Data.ActionState = PlayerActionState.Holding;
            }
        }
        else
        {
            Data.ActionState = PlayerActionState.Default;
            Data.Speed = Data.DefaultSpeed * (SpeedModObj != null ? SpeedModObj.SpeedModifier : 1) * GameHelper.GM.timeScale;
        }

        // Add force to the rigid body
        _body.AddForce(Data.Direction * Data.Speed);

        if(Data.Direction.magnitude > 0.01f)
            playerSprite.transform.localRotation = Quaternion.FromToRotation(Vector2.up, Data.Direction.normalized);

        // Update the move state
        if (_body.velocity.magnitude > 0.1f)
            Data.MoveState = PlayerMoveState.Run;
        else
            Data.MoveState = PlayerMoveState.Idle;

        // Update the animator parameters
        if (_animator.GetInteger(PlayerHelper.ANIMATOR_ACTION_PARAM_NAME) != (int)Data.ActionState)
        {
            _animator.SetInteger(PlayerHelper.ANIMATOR_ACTION_PARAM_NAME, (int)Data.ActionState);
            updateAnimation = true;
        }

        if (_animator.GetInteger(PlayerHelper.ANIMATOR_MOVE_PARAM_NAME) != (int)Data.MoveState)
        {
            _animator.SetInteger(PlayerHelper.ANIMATOR_MOVE_PARAM_NAME, (int)Data.MoveState);
            updateAnimation = true;
        }

        if(updateAnimation)
        {
            string newAnimationName = Data.ActionState.ToString() + Data.MoveState.ToString();
            _animator.StopPlayback();
            _animator.Play(newAnimationName);
        }

        _animator.speed = 0.25f + (_body.velocity.magnitude / 6.0f);

        Data.Position = transform.position;
    }
}
