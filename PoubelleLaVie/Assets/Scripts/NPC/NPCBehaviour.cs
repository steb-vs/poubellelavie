using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class NPCBehaviour : MonoBehaviour, IUsable
{
    #region Public fields

    private GlobalState _globalState;
    public DrunkState drunkType;
    public float incrCopsBarOverTime = 5.8f;
    public GameObject bottle;
    public GameObject puke;
    public Vector3 Position => transform.position;
    public bool IsHeavy => true;

    #endregion

    #region Private fields 

    private BoxCollider2D _boxCollider;
    private Animator _animatorNPC;
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
    private Collider2D _collider;

    #endregion

    #region Public methods

    public void Use(GameObject sender)
    {
    }

    public bool Take(GameObject sender)
    {
        transform.parent = sender.transform.GetChild(0).transform;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        ToCarried();

        _animatorNPC.StopPlayback();
        _animatorNPC.Play("Grabbed");

        return true;
    }

    public bool Drop(GameObject sender)
    {
        transform.parent = null;
        ToTheGround();

        _animatorNPC.StopPlayback();
        _animatorNPC.Play("New State");

        return true;
    }

    /// <summary>
    /// Add to the NPC the state of 'BEING_CARRIED'.
    /// Doesn't change the inner state.
    /// </summary>
    public void ToCarried()
    {
        _globalState |= GlobalState.BEING_CARRIED;

        if (_path != null)
            _path.Clear();
        _path = null;

        _gotPath = false;
        nextPos = null;
        timer = 0;
        gotDestination = false;

        //_renderer.enabled = false;
        _collider.enabled = false;
    }

    private IEnumerator fall()
    {
        while (transform.localScale.x > 0)
        {
            var coef = Time.deltaTime;
            transform.localScale -= new Vector3(coef, coef, coef);
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }
    
    /// <summary>
    /// Retire to the NPC the state of 'BEING_CARRIED'.
    /// </summary>
    public void ToTheGround()
    {
        if (GameHelper.GM.playerComponent.closeWindow &&
            _globalState.HasFlag(GlobalState.DRUNK))
        {
            transform.position += GameHelper.GM.playerComponent.closeWindow.transform.up * 2;
            StartCoroutine(fall());
            Destroy(this.gameObject, 1);
            return;
        }

        _globalState ^= GlobalState.BEING_CARRIED;
        _renderer.enabled = true;
        _collider.enabled = true;
    }

    #endregion

    #region Private methods

    private void Start()
    {
        _boxCollider = GetComponent<BoxCollider2D>();
        _animatorNPC = GetComponent<Animator>();

        Random rnd = new Random();
        drunkType = (DrunkState) Random.Range(0, (int) DrunkState.TOTAL_DRUNK_STATES);
//        drunkType = DrunkState.LOVER;

        _globalState = GlobalState.NEED_DRINKING;

        _animatorNPC.SetBool("isDrunk", false);
        _animatorNPC.SetBool("isTrash", false);

        _renderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
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
        _globalState = GlobalState.FINE;
        _animatorNPC.SetBool("isDrunk", true);
        numberDrinksPending = (uint) Random.Range(2, 5); // Between 2 and 4 times to drink before becoming drunk

        timer = 0.5F; // Take 0.5 secs to grab a beer
        gotDestination = false;
        _gotPath = false;
    }

    private void GotToDestination()
    {
        gotDestination = false;
        _gotPath = false;
        timer = _globalState == GlobalState.FINE ? 2F : 1.5F; // Make the drinking action a little bit longer
        _animatorNPC.SetBool("isWalking", false);
    }

    private void GoToPlayer()
    {
        var playerPos = GameHelper.GM.player.transform.position;
        _path = PathfinderHelper.Pathfinder.GetPath2(
            new Vector2Int((int) transform.position.x, (int) transform.position.y),
            new Vector2Int((int) playerPos.x, (int) playerPos.y));

        if (_path != null && _path.Count > 0)
        {
            gotDestination = true;
            _gotPath = true;
            _animatorNPC.SetBool("isWalking", true);
        }
    }

    private void GetRandomDestination()
    {
        int rndX = Random.Range(0, PathfinderHelper.Pathfinder.getGridBoundX);
        int rndY = Random.Range(0, PathfinderHelper.Pathfinder.getGridBoundY);

        int x = (int)transform.position.x;
        int y = (int)transform.position.y;

        var list = PathfinderHelper.Pathfinder._listedNodes.FindAll(n => n.walkable && ((Mathf.Abs(n.gridX - x) + Mathf.Abs(n.gridY - y)) < 5));
        WorldTile destination =
            list[Random.Range(0, list.Count)];

        _path = PathfinderHelper.Pathfinder.GetPath2(
            new Vector2Int((int) transform.position.x, (int) transform.position.y),
            new Vector2Int(destination.gridX, destination.gridY));

        if (_path != null && _path.Count > 0)
        {
            gotDestination = true;
            _gotPath = true;
            _animatorNPC.SetBool("isWalking", true);
        }
    }

    private void Update()
    {
        if (_globalState.HasFlag(GlobalState.BEING_CARRIED))
            return; // We stop here


        // NPC is not being carried by the player: he can do his things
        switch (_globalState)
        {
            case GlobalState.NEED_DRINKING:
                if (_gotPath == false) // Choose a random fridge, and go there to pick a beer
                {
                    int fridgeNumber = Random.Range(0, PathfinderHelper.Pathfinder.fridges.Count);
                    var fridge = PathfinderHelper.Pathfinder.fridges[fridgeNumber];
                    _path = PathfinderHelper.Pathfinder.GetPath2(
                        new Vector2Int((int) transform.position.x, (int) transform.position.y),
                        new Vector2Int(fridge.gridX, fridge.gridY - 1));

                    if (_path != null && _path.Count > 0)
                    {
                        _animatorNPC.SetBool("isWalking", true);
                        _gotPath = true;
                    }

                    callBack = GotBeer;
                }

                break;
            case GlobalState.FINE:
                // Check if NPC is drunk
                if (numberDrinksPending == 0)
                {
                    _globalState = GlobalState.DRUNK;
                    getDrunk();
                }
                
                // The NPC just ended the drink action
                if (timer <= 0.0f && gotDestination == false && _gotPath == false)
                {
                    GetRandomDestination();
                    callBack = GotToDestination;

                    // NPC's timer has not been set to 0: he drank
                    if (timer < 0) 
                        numberDrinksPending -= 1;
                }

                // NPC is drinking
                if (!_gotPath)
                    timer -= Time.deltaTime * GameHelper.GM.timeScale;
                
                break;
            case GlobalState.DRUNK:
                HandleDrunk();
                break;
            default:
                print("Unexpected global state of an NPC !");
                print("Actual global state value: " + _globalState);
                break;
        }

        HandleDeplacement();
        _animatorNPC.SetInteger("drunkState", (int) drunkType);
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
        float toWalkX = ((distX * Time.deltaTime) / speed) * GameHelper.GM.timeScale;
        float toWalkY = ((distY * Time.deltaTime) / speed) * GameHelper.GM.timeScale;

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
        _animatorNPC.SetBool("isTrash", true);

        switch (drunkType)
        {
            case DrunkState.DANCER:
                speed /= 2;
                callBack = GotToDestination;
                gotDestination = false;
                break;
            case DrunkState.LOVER:
                callBack = GotToDestination;
                gotDestination = false;
                break;
            case DrunkState.PUKER:
                callBack = GotToDestination;
                gotDestination = false;
                speed *= 2;
                break;
            default:
                print("Unexpected drunk state of an NPC !");
                print("Actual drunk state value:" + drunkType);
                break;
        }
    }

    /// <summary>
    /// Called when NPC's _globalState is DRUNK.
    /// </summary>
    private void HandleDrunk()
    {
        if (!_gotPath) // NPC is doing something : decrease timer
            timer -= Time.deltaTime * GameHelper.GM.timeScale;

        switch (drunkType)
        {
            case DrunkState.DANCER:
                // Make the player lose more and more
                GameHelper.GM.AddPrctUntilCops(incrCopsBarOverTime * Time.deltaTime * GameHelper.GM.timeScale);

                if (timer <= 0.0F && gotDestination == false && _gotPath == false)
                {
                    GetRandomDestination();
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
                    GetRandomDestination();
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
            garbage_ = GameObject.Instantiate(bottle, transform.position, Quaternion.identity) as GameObject;
        else // Puke
            garbage_ = GameObject.Instantiate(puke, transform.position, Quaternion.identity) as GameObject;

        lastTile.walkable = false;
        garbage_.GetComponent<Garbage>().worldTile = lastTile;
    }


    private float DistanceBetweenPlayer(float caseX, float caseY)
    {
        var playerPos = GameHelper.GM.player.transform.position;
        float res;

        res = Math.Abs(caseX - playerPos.x);
        res += Math.Abs(caseY - playerPos.y);

        return res;
    }

    #endregion
}