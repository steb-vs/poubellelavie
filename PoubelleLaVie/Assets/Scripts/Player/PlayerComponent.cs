using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerComponent : HumanComponent<PlayerDataComponent>
{
    public delegate void TrashThrownHandler(GameObject obj, PlayerComponent playerComponent, int trashCount);
    public delegate void UsableHandler(GameObject obj, PlayerComponent playerComponent, IUsable usable);

    public event TrashThrownHandler OnTrashThrown;
    public event UsableHandler OnObjectTaken;
    public event UsableHandler OnObjectDropped;

    private HashSet<IUsable> _closeObjects;
    private IUsable _carriedObject;
    private IController<PlayerAction> _controller;

    protected override void Start()
    {
        base.Start();

        _data = GetComponent<PlayerDataComponent>();
        _controller = GetComponent<IController<PlayerAction>>();
        _closeObjects = new HashSet<IUsable>();
    }

    protected override string ResolveAnimationName()
    {
        return _data.actionState.ToString() + _data.moveState.ToString();
    }

    protected override void Update()
    {
        base.Update();
        ProcessActions();
    }

    /// <summary>
    /// Processes the user input.
    /// </summary>
    private void ProcessActions()
    {
        // Handle take / drop action
        if(_controller.GetActionDown(PlayerAction.TakeDrop))
        {
            // If we are holding an object
            if (_carriedObject != null)
            {
                OnObjectDropped?.Invoke(gameObject, this, _carriedObject);

                // Drop it!
                if(_carriedObject.Drop(gameObject))
                    _carriedObject = null;
            }
            
            else if (_data.trashCount > 0 && _data.closeWindows.Any())
            {
                OnTrashThrown?.Invoke(gameObject, this, _data.trashCount);

                _data.trashCount = 0;

                GameObject t = Instantiate(
                    GameHelper.GameManager.thrownTrash,
                    transform.position + _data.closeWindows.First().transform.up * 2,
                    Quaternion.identity);

                Destroy(t, 1);
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
        if (_controller.GetActionDown(PlayerAction.Use) && _carriedObject != null)
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
        if (_data.speedModifierObject != null && _data.speedModifierObject.SpeedModifier < speedModObj.SpeedModifier)
            return;

        _data.speedModifierObject = speedModObj;
    }

    private void RestoreSpeedMod(GameObject obj)
    {
        ISpeedModifier speedModObj = obj.GetComponent<ISpeedModifier>();

        if (speedModObj == null)
            return;
        
        // Skip if the speed mod object is not the same as the current
        if (_data.speedModifierObject != speedModObj)
            return;

        _data.speedModifierObject = null;
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

    protected override void Move(ref bool updateAnimation)
    {
        // Update the action state
        if (_carriedObject != null)
        {
            if (_carriedObject.IsHeavy)
            {
                _data.speed = _data.defaultSpeed * 0.6f * (_data.speedModifierObject != null ? _data.speedModifierObject.SpeedModifier : 1) * GameHelper.GameManager.data.timeScale;
                _data.actionState = PlayerActionState.Grabbing;
            }
            else
            {
                _data.speed = _data.defaultSpeed * (_data.speedModifierObject != null ? _data.speedModifierObject.SpeedModifier : 1) * GameHelper.GameManager.data.timeScale;
                _data.actionState = PlayerActionState.Holding;
            }
        }
        else
        {
            _data.actionState = PlayerActionState.Default;
            _data.speed = _data.defaultSpeed * (_data.speedModifierObject != null ? _data.speedModifierObject.SpeedModifier : 1) * GameHelper.GameManager.data.timeScale;
        }

        base.Move(ref updateAnimation);

        // Update the animator parameters
        if (_animator.GetInteger(PlayerHelper.ANIMATOR_ACTION_STATE_PARAM_NAME) != (int)_data.actionState)
        {
            _animator.SetInteger(PlayerHelper.ANIMATOR_ACTION_STATE_PARAM_NAME, (int)_data.actionState);
            updateAnimation = true;
        }
    }
}
