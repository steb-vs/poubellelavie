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
    public int Priority =>
        _data?.npcState != NPCState.Drunk ? 100 :
        _data?.drunkType == DrunkType.Dancer ? 1 :
        _data?.drunkType == DrunkType.Puker ? 2 :
        _data?.drunkType == DrunkType.Dancer ? 3 :
        100;

    public event Action<GameObject, NPCComponent> OnFall;

    private PlayerDataComponent _playerData;
    private NPCAIController _controller;
    private SpriteRenderer _spriteRenderer;

    public void Use(GameObject sender)
    {
    }

    public bool Take(GameObject sender)
    {
        _data.grabbed = true;

        transform.parent = sender.transform.GetChild(0).transform;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        spriteGameObject.transform.localRotation = Quaternion.identity;

        _collider.enabled = false;
        _body.isKinematic = true;
        _body.velocity = Vector2.zero;

        // Desactivate the dancer (if dancer)
        if (_data.drunkType == DrunkType.Dancer)
            GameHelper.GameManager.data.dancerCount--;

        return true;
    }

    public bool Drop(GameObject sender)
    {
        _data.grabbed = false;
        transform.parent = null;
        transform.localRotation = Quaternion.identity;

        if (_playerData.closeWindows.Any() && _data.npcState == NPCState.Drunk)
        {
            _data.falling = true;
            OnFall?.Invoke(gameObject, this);
            transform.position += _playerData.closeWindows.First().transform.up * 2;
            GameHelper.GameManager.data.score += 100;

            return true;
        }

        _collider.enabled = true;
        _body.isKinematic = false;

        // Add the dancer to nbrDancer (if dancer)
        if (_data.drunkType == DrunkType.Dancer)
            GameHelper.GameManager.data.dancerCount++;

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
        _spriteRenderer = spriteGameObject.GetComponent<SpriteRenderer>();

        if(_controller != null)
            _controller.OnNPCReachTarget += NPCReachTargetAction;
    }

    private void NPCReachTargetAction(WorldTile reachedTile, NPCState npcState, DrunkType drunkType)
    {
        if (drunkType != DrunkType.Puker || npcState != NPCState.Drunk || reachedTile == null || reachedTile.garbage != null)
            return;

        int chooseGarbageType = Random.Range(0, 2);
        GameObject garbage;

        if (chooseGarbageType == 0) // Bottle
        {
            garbage = Instantiate(bottle, transform.position, Quaternion.identity) as GameObject;
        }
        else // Puke
            garbage = Instantiate(puke, transform.position, Quaternion.identity) as GameObject;

        reachedTile.walkable = false;
        reachedTile.garbage = garbage;
        garbage.GetComponent<Garbage>().worldTile = reachedTile;
    }

    protected override void Update()
    {
        base.Update();

        if (GameHelper.GameManager.data.gameOver)
            return;

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
    }

    protected override void Move(ref bool updateAnimation)
    {
        if (_animator.GetBool(NPCHelper.ANIMATOR_GRABBED_PARAM_NAME) != _data.grabbed)
        {
            _animator.SetBool(NPCHelper.ANIMATOR_GRABBED_PARAM_NAME, _data.grabbed);
            updateAnimation = true;
        }

        if (_data.grabbed)
        {
            _body.velocity = Vector2.zero;
            return;
        }

        if (_data.npcState == NPCState.Drunk)
        {
            if (_data.drunkType == DrunkType.Dancer)
                _data.speed = _data.defaultSpeed * 1.8f;
            else if (_data.drunkType == DrunkType.Lover)
                _data.speed = _data.defaultSpeed * 1.5f;
            else if (_data.drunkType == DrunkType.Puker)
                _data.speed = _data.defaultSpeed * 0.5f;
        }
        else
            _data.speed = _data.defaultSpeed;

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

        if (_data.npcState == NPCState.Drunk)
        {
            _animator.speed = 1.0f;

            if (_data.drunkType == DrunkType.Dancer)
                _spriteRenderer.color = _data.dancerColor;
            else if (_data.drunkType == DrunkType.Lover)
                _spriteRenderer.color = _data.loverColor;
            else if (_data.drunkType == DrunkType.Puker)
                _spriteRenderer.color = _data.pukerColor;
        }
        else
        {
            _spriteRenderer.color = Color.white;
        }
    }
}