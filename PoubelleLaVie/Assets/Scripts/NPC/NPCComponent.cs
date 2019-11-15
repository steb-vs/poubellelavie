using System;
using System.Collections;
using System.Collections.Generic;
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
            GameHelper.GameManager.RetireDancer();

        _animator.StopPlayback();
        _animator.Play("Grabbed");

        return true;
    }

    public bool Drop(GameObject sender)
    {
        _data.isCarried = false;
        transform.parent = null;

        if (GameHelper.GameManager.playerComponent.closeWindow && _data.npcState == NPCState.Drunk)
        {
            _data.falling = true;
            OnFall?.Invoke(gameObject, this);
            transform.position += GameHelper.GameManager.playerComponent.closeWindow.transform.up * 2;
        }

        _collider.enabled = true;

        // Add the dancer to nbrDancer (if dancer)
        if (_data.drunkType == DrunkType.Dancer)
            GameHelper.GameManager.AddDancer();

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
    }

    protected override void Update()
    {
        base.Update();

        if(_data.falling)
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

        if(_data.npcState == NPCState.NeedDrinking)
        {
            if (_gotPath == false) // Choose a random fridge, and go there to pick a beer
            {
                int fridgeNumber = Random.Range(0, PathfinderHelper.Pathfinder.fridges.Count);
                var fridge = PathfinderHelper.Pathfinder.fridges[fridgeNumber];
                _path = PathfinderHelper.Pathfinder.GetPath2(
                    new Vector2Int((int)transform.position.x, (int)transform.position.y),
                    new Vector2Int(fridge.gridX, fridge.gridY - 1));

                if (_path != null && _path.Count > 0)
                {
                    _animator.SetBool("isWalking", true);
                    _gotPath = true;
                }

                callBack = GotBeer;
            }
        }

        else if(_data.npcState == NPCState.Fine)
        {
            // Check if NPC is drunk
            if (numberDrinksPending == 0)
            {
                globalState = GlobalState.DRUNK;
                getDrunk();
            }

            // The NPC just ended the drink action
            if (timer <= 0.0f && gotDestination == false && _gotPath == false)
            {
                GetRandomDestination(2, 6);
                callBack = GotToDestination;

                // NPC's timer has not been set to 0: he drank
                if (timer < 0)
                    numberDrinksPending -= 1;
            }

            // NPC is drinking
            if (!_gotPath)
                timer -= Time.deltaTime * GameHelper.GameManager.timeScale;
        }

        else if(_data.npcState == NPCState.Drunk)
        {
            HandleDrunk();
        }

        HandleDeplacement();
        _animator.SetInteger("drunkState", (int)drunkType);
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
        globalState = GlobalState.FINE;
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
        timer = globalState == GlobalState.FINE ? 2F : 1.5F; // Make the drinking action a little bit longer
        _animator.SetBool("isWalking", false);
    }

    private void GoToPlayer()
    {
        var playerPos = GameHelper.GameManager.player.transform.position;
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

    private void HandleDeplacement()
    {
        // No path
        if (!_gotPath || _path == null)
            return;

        // Ended deplacement
        if (_path.Count <= 0 && callBack != null)
        {
            callBack();
            return;
        }

        
        // NPC is making a turn
        if (!nextPos)
        {
            nextPos = _path[0];

            if (nextPos.walkable == false)
            {
                _path.Clear();
                _path = null;
                nextPos = null;
                _gotPath = false;
                gotDestination = false;
                return;
            }

            distX = nextPos.gridX - transform.position.x;
            distY = nextPos.gridY - transform.position.y;


            var rot = transform.rotation;
            if (distX < -0.1f)
                rot.z = 90;
            else if (distX > 0.1f)
                rot.z = 270;
            else if (distY > 0.1f)
                rot.z = 0;
            else if (distY < -0.1f)
                rot.z = 180;

            transform.rotation = Quaternion.Euler(rot.x, rot.y, rot.z);

            distWalkedX = 0;
            distWalkedY = 0;
            nextPos.walkable = false;

            return;
        }

        // Keep walking at the same direction
        float toWalkX = ((distX * Time.deltaTime) / speed) * GameHelper.GameManager.timeScale;
        float toWalkY = ((distY * Time.deltaTime) / speed) * GameHelper.GameManager.timeScale;

        distWalkedX += toWalkX;
        distWalkedY += toWalkY;

        transform.position += new Vector3(toWalkX, toWalkY, 0);


        if (Mathf.Abs(distX) - Mathf.Abs(distWalkedX) <= 0 &&
            Mathf.Abs(distY) - Mathf.Abs(distWalkedY) <= 0)
        {
            transform.position = new Vector3(nextPos.gridX, nextPos.gridY, 0);
            _path.RemoveAt(0);
            nextPos.walkable = true;
            lastTile = nextPos;
            nextPos = null;
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
            timer -= Time.deltaTime * GameHelper.GameManager.timeScale;

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
            garbage_ = GameObject.Instantiate(bottle, transform.position, Quaternion.identity) as GameObject;
        }
        else // Puke
            garbage_ = GameObject.Instantiate(puke, transform.position, Quaternion.identity) as GameObject;
        lastTile.walkable = false;
        garbage_.GetComponent<Garbage>().worldTile = lastTile;
    }


    private float DistanceBetweenPlayer(float caseX, float caseY)
    {
        var playerPos = GameHelper.GameManager.player.transform.position;
        float res;

        res = Math.Abs(caseX - playerPos.x);
        res += Math.Abs(caseY - playerPos.y);

        return res;
    }
}