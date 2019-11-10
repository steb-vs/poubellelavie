using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class NPCBehaviour : MonoBehaviour
{
    private BoxCollider2D _boxCollider;
    private Animator _animatorNPC;

    private GlobalState _globalState;
    private ActionState _actionState;
    public DrunkState drunkType;

    public float prctUntilDrunk;
    public float incrDrunkOverTime = 1.0F;

    public float incrCopsBarOverTime = 5.8f;

    public GameObject bottle;

    // Start is called before the first frame update
    void Start()
    {
        _boxCollider = GetComponent<BoxCollider2D>();
        _animatorNPC = GetComponent<Animator>();

        Random rnd = new Random();
//        drunkType = (DrunkState) Random.Range(0, (int) DrunkState.TOTAL_DRUNK_STATES);
        drunkType = DrunkState.PUKER;
        _globalState = GlobalState.NEED_DRINKING;
        _actionState = ActionState.IDLE;

        prctUntilDrunk = 0; // Over 100 (100%), the NPC becomes drunk
        _animatorNPC.SetBool("isDrunk", false);
        _animatorNPC.SetBool("isTrash", false);
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

    void GotBeer()
    {
        _globalState = GlobalState.FINE;
        _animatorNPC.SetBool("isDrunk", true);
        timer = -1;
    }

    void GotToDestination()
    {
        gotDestination = false;
        timer = 1.5f;
        _animatorNPC.SetBool("isWalking", false);
    }

    void GetRandomDestination()
    {
        int rndX = Random.Range(0, PathfinderHelper.Pathfinder.getGridBoundX);
        int rndY = Random.Range(0, PathfinderHelper.Pathfinder.getGridBoundY);

        var list = PathfinderHelper.Pathfinder._listedNodes.FindAll(n => n.walkable);
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

    // Update is called once per frame
    void Update()
    {
        if (_globalState.HasFlag(GlobalState.BEING_CARRIED))
        {
            print("DO THE THINGS WHILE I AM CARRIED");
            return; // We stop here
        }


        // NPC is not being carried by the player: he can do his things
        switch (_globalState)
        {
            case GlobalState.NEED_DRINKING:
                if (_gotPath == false)
                {
                    var fridge = PathfinderHelper.Pathfinder.fridges[0];
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
                _boxCollider.enabled = false;

                // Increments the drunk bar
                prctUntilDrunk += incrDrunkOverTime * Time.deltaTime * GameHelper.GM.timeScale;
                if (prctUntilDrunk >= 100)
                {
                    _globalState = GlobalState.DRUNK;
                    getDrunk();
                }

//                Debug.Log(timer);
                if (timer <= 0.0f && gotDestination == false && _gotPath == false)
                {
                    GetRandomDestination();
                    callBack = GotToDestination;
                }

                if (!_gotPath)
                {
                    timer -= Time.deltaTime;
                }

//                print("CURRENT DRINK BAR: " + prctUntilDrunk);
                break;
            case GlobalState.DRUNK:
                HandleDrunk();
                
  

                break;
            default:
                print("Unexpected global state of an NPC !");
                print("Actual global state value: " + _globalState);
                break;
        }

        // Set variables for the state machine


        if (_gotPath)
        {
            if (_path == null)
                return;

            if (_path.Count <= 0)
            {
                _gotPath = false;
                if (callBack != null)
                    callBack();
            }
            else
            {
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
                    {
                        rot.z = 90;
                    }
                    else if (distX > 0.1f)
                    {
                        rot.z = 270;
                    }
                    else if (distY > 0.1f)
                    {
                        rot.z = 0;
                    }
                    else if (distY < -0.1f)
                    {
                        rot.z = 180;
                    }
                    
                    transform.rotation = Quaternion.Euler(rot.x, rot.y, rot.z);   //rot;
                    
                    distWalkedX = 0;
                    distWalkedY = 0;
                    nextPos.walkable = false;
                    _actionState = ActionState.IDLE;
                }
                else
                {
                    float toWalkX = (distX * Time.deltaTime) / speed;
                    float toWalkY = (distY * Time.deltaTime) / speed;

                    distWalkedX += toWalkX;
                    distWalkedY += toWalkY;

                    transform.position += new Vector3(toWalkX, toWalkY, 0);
//                    transform.Translate(toWalkX, toWalkY, 0);
                    _actionState = ActionState.WALKING;


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
            }
        }

//        _animatorNPC.SetBool("isWalking", _actionState == ActionState.WALKING);
//        _animatorNPC.SetBool("isDrunk", _globalState == GlobalState.FINE);
//        _animatorNPC.SetBool("isTrash", _globalState == GlobalState.DRUNK);
        _animatorNPC.SetInteger("drunkState", (int) drunkType);
    }

    private void getDrunk()
    {
        _animatorNPC.SetBool("isTrash", true);

        switch (drunkType)
        {
            case DrunkState.DANCER:
                _animatorNPC.SetBool("isWalking", false);

//                callBack = GotToDestination;
                _actionState = ActionState.IDLE;
                callBack = null;
//                speed /= 2;
                break;
            case DrunkState.LOVER:
                break;
            case DrunkState.PUKER:
                callBack = GotToDestination;
                _actionState = ActionState.IDLE;
                gotDestination = false;
                speed *= 2;
                break;
            default:
                print("Unexpected drunk state of an NPC !");
                print("Actual drunk state value:" + drunkType);
                break;
        }
    }

    // Called when NPC's _globalState is DRUNK
    private void HandleDrunk()
    {
        Debug.Log($"{timer} - {gotDestination} - {_gotPath}");
        if (timer <= 0.0f && gotDestination == false && _gotPath == false)
        {
            GetRandomDestination();
            if (drunkType == DrunkState.PUKER)
            {
                if (lastTile.walkable)
                {
                    var bottle_ =
                        GameObject.Instantiate(bottle, transform.position, Quaternion.identity) as GameObject;
                    lastTile.walkable = false;
                    bottle_.GetComponent<Garbage>().worldTile = lastTile;
                }
            }
        }

        if (!_gotPath)
        {
            timer -= Time.deltaTime;
        }
        
        switch (drunkType)
        {
            case DrunkState.DANCER:
                GameHelper.GM.AddPrctUntilCops(incrCopsBarOverTime * Time.deltaTime * GameHelper.GM.timeScale);
                _animatorNPC.SetBool("isWalking", false);

                break;
            case DrunkState.LOVER:
                break;
            case DrunkState.PUKER:
                break;
            default:
                print("Unexpected drunk state of an NPC !");
                print("Actual drunk state value:" + drunkType);
                break;
        }
    }

    /*
     * Add to the NPC the state of 'BEING_CARRIED'.
     * Doesn't change the inner state.
     */
    public void ToCarried()
    {
        _globalState |= GlobalState.BEING_CARRIED;

        _path.Clear();
        _path = null;

        _gotPath = false;
        timer = 0;
        nextPos = null;
    }

    /*
     * Retire to the NPC the state of 'BEING_CARRIED'.
     */
    public void ToTheGround()
    {
        _globalState ^= GlobalState.BEING_CARRIED;
    }

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
    private bool gotDestination = false;
    private WorldTile lastTile = null;
}