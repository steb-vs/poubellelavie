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
    private UnityAction callBack;
    private bool _gotPath = false;
    private List<WorldTile> _path;
    private WorldTile nextPos = null;
    private float distX = 0;
    private float distY = 0;
    private float distWalkedX;
    private float distWalkedY;
    private float speed = 0.5f;
    private float timer = 5;
    private uint numberDrinksPending;
    private bool gotDestination = false;
    private WorldTile lastTile = null;

    private SpriteRenderer _renderer;
    private PlayerDataComponent _playerData;

    public void Use(GameObject sender)
    {
    }

    public bool Take(GameObject sender)
    {
        _data.isCarried = true;

        transform.parent = sender.transform.GetChild(0).transform;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        _path?.Clear();
        _path = null;

        _gotPath = false;
        nextPos = null;
        timer = 0;
        gotDestination = false;

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
        _data.isCarried = false;
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

    protected override void Start()
    {
        base.Start();

        _data.drunkType = Utils.Random<DrunkType>();
        _data.npcState = NPCState.NeedDrinking;

        _animator.SetBool("isDrunk", false);
        _animator.SetBool("isTrash", false);

        _renderer = GetComponent<SpriteRenderer>();
        _playerData = GameHelper.Player.GetComponent<PlayerDataComponent>();
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
        

        HandleDeplacement();
        _animator.SetInteger("drunkState", (int)_data.drunkType);
    }

    private void OnDrawGizmos()
    {
        if (_path != null)
        {
            foreach (var worldTile in _path)
            {
                Gizmos.DrawCube(worldTile.transform.position, Vector3.one / 3);
            }
        }
    }

    private void GotBeer()
    {
        _data.npcState = NPCState.Fine;
        _animator.SetBool("isDrunk", true);
        numberDrinksPending = (uint) Random.Range(2, 5); // Between 2 and 4 times to drink before becoming drunk

        timer = 0.5F; // Take 0.5 secs to grab a beer
        gotDestination = false;
        _gotPath = false;
    }

    private void GotToDestination()
    {
        gotDestination = false;
        _gotPath = false;
        timer = _data.npcState == NPCState.Fine ? 2F : 1.5F; // Make the drinking action a little bit longer
        _animator.SetBool("isWalking", false);
    }

    private void GoToPlayer()
    {
        var playerPos = GameHelper.Player.transform.position;
        _path = PathfinderHelper.Pathfinder.GetPath2(
            new Vector2Int((int) transform.position.x, (int) transform.position.y),
            new Vector2Int((int) playerPos.x, (int) playerPos.y));

        if (_path != null && _path.Count > 0)
        {
            gotDestination = true;
            _gotPath = true;
            _animator.SetBool("isWalking", true);
        }
    }

    private void GetRandomDestination(int min, int max)
    {
        int x = (int)transform.position.x;
        int y = (int)transform.position.y;

        var list = PathfinderHelper.Pathfinder._listedNodes.FindAll(n => n.walkable && ((Mathf.Abs(n.gridX - x) + Mathf.Abs(n.gridY - y)) < max));
        WorldTile destination =
            list[Random.Range(0, list.Count)];

        _path = PathfinderHelper.Pathfinder.GetPath2(
            new Vector2Int((int) transform.position.x, (int) transform.position.y),
            new Vector2Int(destination.gridX, destination.gridY));

        if (_path != null && _path.Count > 0)
        {
            gotDestination = true;
            _gotPath = true;
            _animator.SetBool("isWalking", true);
        }
    }

    private void getDrunk()
    {
        _animator.SetBool("isTrash", true);

        switch (drunkType)
        {
            case DrunkState.DANCER:
                speed /= 2; // Goes faster
                callBack = GotToDestination;
                gotDestination = false;
                GameHelper.GameManager.AddDancer();
                break;
            case DrunkState.LOVER:
                speed /= 2; // Goes faster
                callBack = GotToDestination;
                gotDestination = false;
                break;
            case DrunkState.PUKER:
                callBack = GotToDestination;
                gotDestination = false;
                speed *= 2; // Goes slower
                break;
            default:
                print("Unexpected drunk state of an NPC !");
                print("Actual drunk state value:" + drunkType);
                break;
        }
    }

    /// <summary>
    /// Called when NPC's globalState is DRUNK.
    /// </summary>
    private void HandleDrunk()
    {
        if (!_gotPath) // NPC is doing something : decrease timer
            timer -= Time.deltaTime * GameHelper.GameManager.data.timeScale;

        switch (drunkType)
        {
            case DrunkState.DANCER:
                if (timer <= 0.0F && gotDestination == false && _gotPath == false)
                {
                    GetRandomDestination(2, 15);
                    callBack = GotToDestination;
                }

                break;
            case DrunkState.LOVER:
                if (_path == null || _gotPath == false)
                    GoToPlayer();
                else if (_path.Count != 0
                ) // Check if the LOVER is going to the right tile (not too far away from the player)
                {
                    float targetX, targetY;
                    targetX = _path[_path.Count - 1].transform.position.x;
                    targetY = _path[_path.Count - 1].transform.position.y;
                    if (DistanceBetweenPlayer(targetX, targetY) > 2)
                        GoToPlayer(); // Current target tile is too far away from the player
                }

                break;
            case DrunkState.PUKER:
                if (timer <= 0.0f && gotDestination == false && _gotPath == false)
                {
                    GetRandomDestination(2, 13);
                    if (lastTile.walkable)
                        SpawnRandomGarbage();
                }

                break;
            default:
                print("Unexpected drunk state of an NPC !");
                print("Actual drunk state value:" + drunkType);
                break;
        }
    }

    private void SpawnRandomGarbage()
    {
        int chooseGarbageType = Random.Range(0, 2);
        GameObject garbage_;
        if (chooseGarbageType == 0) // Bottle
        {
            garbage_ = Instantiate(bottle, transform.position, Quaternion.identity) as GameObject;
        }
        else // Puke
            garbage_ = Instantiate(puke, transform.position, Quaternion.identity) as GameObject;
        lastTile.walkable = false;
        garbage_.GetComponent<Garbage>().worldTile = lastTile;
    }


    private float DistanceBetweenPlayer(float caseX, float caseY)
    {
        var playerPos = GameHelper.Player.transform.position;
        float res;

        res = Math.Abs(caseX - playerPos.x);
        res += Math.Abs(caseY - playerPos.y);

        return res;
    }
}