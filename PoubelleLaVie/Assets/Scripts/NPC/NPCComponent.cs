using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class NPCComponent : HumanComponent<NPCDataComponent>, IUsable
{
    public GameObject bottle;
    public GameObject puke;

    public Vector3 Position => transform.position;
    public bool IsHeavy => true;

    public event Action<GameObject, NPCComponent> OnFall;

    private BoxCollider2D _boxCollider;
    private PlayerDataComponent _playerData;
    private NPCAIController _controller;

    public void Use(GameObject sender)
    {
    }

    public bool Take(GameObject sender)
    {
        _data.grabbed = true;

        transform.parent = sender.transform.GetChild(0).transform;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        _collider.enabled = false;

        // Desactivate the dancer (if dancer)
        if (_data.drunkType == DrunkType.Dancer)
            GameHelper.GameManager.data.dancerCount--;

        _animator.StopPlayback();
        _animator.Play("Grabbed");

        return true;
    }

    public bool Drop(GameObject sender)
    {
        _data.grabbed = false;
        transform.parent = null;

        if (_playerData.closeWindows.Any() && _data.npcState == NPCState.Drunk)
        {
            _data.falling = true;
            OnFall?.Invoke(gameObject, this);
            transform.position += _playerData.closeWindows.First().transform.up * 2;
        }

        _collider.enabled = true;

        // Add the dancer to nbrDancer (if dancer)
        if (_data.drunkType == DrunkType.Dancer)
            GameHelper.GameManager.data.dancerCount++;

        _animator.StopPlayback();
        _animator.Play("New State");

        return true;
    }

    protected override string ResolveAnimationName()
    {
        if (_data.grabbed)
            return "Grabbed";
        if (_data.npcState == NPCState.Drunk)
            return _data.drunkType.ToString() + _data.moveState.ToString();
        else
            return _data.npcState.ToString() + _data.moveState.ToString();
    }

    protected override void Start()
    {
        base.Start();

        _data.drunkType = Utils.Random<DrunkType>();
        _data.npcState = NPCState.NeedDrinking;

        _playerData = GameHelper.Player.GetComponent<PlayerDataComponent>();
        _controller = GetComponent<NPCAIController>();

        if(_controller != null)
            _controller.OnNPCReachTarget += NPCReachTargetAction;
    }

    private void NPCReachTargetAction(WorldTile targetTile, NPCState npcState, DrunkType drunkType)
    {
        if (drunkType != DrunkType.Puker)
            return;

        int chooseGarbageType = Random.Range(0, 2);
        GameObject garbage;

        if (chooseGarbageType == 0) // Bottle
        {
            garbage = Instantiate(bottle, transform.position, Quaternion.identity) as GameObject;
        }
        else // Puke
            garbage = Instantiate(puke, transform.position, Quaternion.identity) as GameObject;

        targetTile.walkable = false;

        garbage.GetComponent<Garbage>().worldTile = targetTile;
    }

    protected override void Update()
    {
        if (_data.falling)
        {
            float scale;

            _data.fallTime += Time.deltaTime;

            scale = 1 - (_data.fallTime / _data.fallDuration);
            if (scale < 0)
                scale = 0;

            transform.localScale = new Vector3(scale, scale, scale);

            if (_data.fallTime > _data.fallDuration)
                Destroy(this);

            return;
        }

        base.Update();
    }

    protected override void Move(ref bool updateAnimation)
    {
        if (_data.drunkType == DrunkType.Dancer)
            _data.speed = _data.defaultSpeed;
        else if (_data.drunkType == DrunkType.Lover)
            _data.speed = _data.defaultSpeed;
        else if (_data.drunkType == DrunkType.Puker)
            _data.speed = _data.defaultSpeed * 0.5f;

        base.Move(ref updateAnimation);

        // Update the animator parameters
        if (_animator.GetInteger(NPCHelper.ANIMATOR_DRUNK_TYPE_PARAM_NAME) != (int)_data.drunkType)
        {
            _animator.SetInteger(NPCHelper.ANIMATOR_DRUNK_TYPE_PARAM_NAME, (int)_data.drunkType);
            updateAnimation = true;
        }

        if (_animator.GetInteger(NPCHelper.ANIMATOR_NPC_STATE_PARAM_NAME) != (int)_data.npcState)
        {
            _animator.SetInteger(NPCHelper.ANIMATOR_NPC_STATE_PARAM_NAME, (int)_data.npcState);
            updateAnimation = true;
        }

        if (_animator.GetBool(NPCHelper.ANIMATOR_GRABBED_PARAM_NAME) != _data.grabbed)
        {
            _animator.SetBool(NPCHelper.ANIMATOR_GRABBED_PARAM_NAME, _data.grabbed);
            updateAnimation = true;
        }
    }
}