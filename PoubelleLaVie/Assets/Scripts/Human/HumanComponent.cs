using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HumanComponent<TData> : MonoBehaviour
    where TData : HumanDataComponent
{
    public GameObject spriteGameObject;

    protected Rigidbody2D _body;
    protected Animator _animator;
    protected BoxCollider2D _collider;
    protected TData _data;

    private IController<HumanAction> _controller;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        _controller = GetComponent<IController<HumanAction>>();
        _body = GetComponent<Rigidbody2D>();
        _data = GetComponent<TData>();
        _collider = GetComponent<BoxCollider2D>();
        _animator = spriteGameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
    }

    protected virtual void Awake()
    {
    }

    protected virtual void FixedUpdate()
    {
        bool updateAnimation = false;
        _controller.UpdateAI();
        Move(ref updateAnimation);

        if (updateAnimation)
            UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        _animator.StopPlayback();
        _animator.Play(ResolveAnimationName());
    }

    protected abstract string ResolveAnimationName();

    /// <summary>
    /// Moves the player with the current data.
    /// </summary>
    protected virtual void Move(ref bool updateAnimation)
    {
        // Get the character direction
        _data.direction = new Vector2(
            _controller.GetActionValue(HumanAction.Horizontal),
            _controller.GetActionValue(HumanAction.Vertical)
        );

        // No more strafe cheat huehuehue
        if (_data.direction.magnitude > 1.0f)
            _data.direction = _data.direction.normalized;

        // Add force to the rigid body
        _body.AddForce(_data.direction * _data.speed);

        if (_data.direction.magnitude > 0.01f)
            spriteGameObject.transform.localRotation = Quaternion.FromToRotation(Vector2.up, _data.direction.normalized);

        // Update the move state
        if (_body.velocity.magnitude > 0.1f)
            _data.moveState = HumanMoveState.Walk;
        else
            _data.moveState = HumanMoveState.Idle;

        if (_animator.GetInteger(HumanHelper.ANIMATOR_MOVE_STATE_PARAM_NAME) != (int)_data.moveState)
        {
            _animator.SetInteger(HumanHelper.ANIMATOR_MOVE_STATE_PARAM_NAME, (int)_data.moveState);
            updateAnimation = true;
        }

        _animator.speed = 0.25f + (_body.velocity.magnitude / 6.0f);

        _data.position = transform.position;
    }
}
