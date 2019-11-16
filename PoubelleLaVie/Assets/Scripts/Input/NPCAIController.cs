using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NPCAIController : Controller<HumanAction>, IController<NPCAction>
{
    public delegate void NPCAIHandler(WorldTile targetTile, NPCState npcState, DrunkType drunkType);

    public event NPCAIHandler OnNPCReachTarget;

    private NPCDataComponent _data;
    private List<WorldTile> _path;
    private float _vertical;
    private float _horizontal;
    private WorldTile _nextTile;
    private WorldTile _previousTile;
    private Vector2 _nextDiff;
    private Action _callback;
    private int _numberDrinksPending;
    private float _idleTimer;
    private bool _idleMode;
    private WorldTile _targetTile;
    private bool _leaveFridge;
    private float _stuckTimer;
    private Rigidbody2D _body;
    private bool _dancerRegistered;

    private const float MAX_STUCK_TIME = 2.0f;

    private void Start()
    {
        _data = GetComponent<NPCDataComponent>();
        _body = GetComponent<Rigidbody2D>();
    }

    public bool GetActionDown(NPCAction action)
    {
        return false;
    }

    public override bool GetActionDown(HumanAction action)
    {
        return false;
    }

    public bool GetActionUp(NPCAction action)
    {
        return false;
    }

    public override bool GetActionUp(HumanAction action)
    {
        return false;
    }

    public float GetActionValue(NPCAction action)
    {
        return 0.0f;
    }

    public override float GetActionValue(HumanAction action)
    {
        if (action == HumanAction.Horizontal)
            return _horizontal;

        if (action == HumanAction.Vertical)
            return _vertical;

        return 0.0f;
    }

    public override void UpdateAI()
    {
        if (_data.grabbed || _data.falling || GameHelper.GameManager.data.paused)
            return;

        if(_body.velocity.magnitude < 0.1f && !_idleMode)
            _stuckTimer += Time.fixedDeltaTime;
        else
            _stuckTimer = 0.0f;

        if (_data.npcState == NPCState.NeedDrinking)
        {
            if (_path == null || (_stuckTimer > MAX_STUCK_TIME && !_idleMode)) // Choose a random fridge, and go there to pick a beer
            {
                int fridgeNumber = UnityEngine.Random.Range(0, PathfinderHelper.Pathfinder.fridges.Count);
                var fridge = PathfinderHelper.Pathfinder.fridges[fridgeNumber];
                _path = PathfinderHelper.Pathfinder.GetPath2(
                    new Vector2Int((int)transform.position.x, (int)transform.position.y),
                    new Vector2Int(fridge.gridX, fridge.gridY - 1));

                _callback = GotBeer;
            }
        }

        else if (_data.npcState == NPCState.Fine)
        {
            // The NPC just ended the drink action
            if (_leaveFridge || (_idleMode && _idleTimer <= 0.0f) || (_stuckTimer > MAX_STUCK_TIME && !_idleMode))
            {
                _idleMode = false;
                _leaveFridge = false;

                GetRandomDestination(2, 6);
                _callback = GotToDestinationFine;
            }
        }

        else if (_data.npcState == NPCState.Drunk)
        {
            if(_data.drunkType == DrunkType.Dancer && !_dancerRegistered)
            {
                _dancerRegistered = true;
                GameHelper.GameManager.data.dancerCount++;
            }

            if ((_idleMode && _idleTimer <= 0.0f) || (_stuckTimer > MAX_STUCK_TIME && !_idleMode))
            {
                _idleMode = false;

                if (_data.drunkType == DrunkType.Dancer)
                    GetRandomDestination(5, 15);
                else if(_data.drunkType == DrunkType.Puker)
                    GetRandomDestination(2, 10);
                else
                    GoToPlayer();

                _callback = GotToDestinationDrunk;
            }
        }

        _horizontal = 0.0f;
        _vertical = 0.0f;

        if (_idleMode)
        {
            _idleTimer -= Time.fixedDeltaTime * GameHelper.GameManager.data.timeScale;
            return;
        }

        // No path
        if (_path == null)
            return;

        // Reached target tile
        if (!_path.Any())
        {
            _path = null;

            _callback?.Invoke();
            _callback = null;

            OnNPCReachTarget?.Invoke(_data.tile, _data.npcState, _data.drunkType);

            return;
        }

        _targetTile = _path.Last();
        _previousTile = _nextTile;
        _nextTile = _path[0];

        // Do we try to reach the same tile as the previous tick? 
        // If not, be sure the new tile is walkable...
        if (_previousTile != _nextTile && _nextTile.walkable == false)
        {
            // Not reachable, clear path!
            _path = null;
            _nextTile = null;
            return;
        }

        _nextDiff.x = _nextTile.gridX - transform.position.x;
        _nextDiff.y = _nextTile.gridY - transform.position.y;

        // Did we reach the next tile?
        if (_nextDiff.magnitude < 0.2f)
        {
            // Yes, make the current walkable again...
            if (_data.tile != null)
                _data.tile.walkable = true;

            // ...and update it with the reached one
            _data.tile = _nextTile;
            // No more walkable
            _data.tile.walkable = false;

            // Tell that we need the next tile
            _nextTile = null;
            // Pop
            _path.RemoveAt(0);

            return;
        }

        _horizontal = _nextDiff.normalized.x;
        _vertical = _nextDiff.normalized.y;

        // "Book" the next tile while we don't reach it
        _nextTile.walkable = false;
    }

    private void GotToDestinationFine()
    {
        _idleTimer = _data.npcState == NPCState.Fine ? 2F : 1.5F; // Make the drinking action a little bit longer
        _numberDrinksPending--;
        _idleTimer = UnityEngine.Random.Range(1.0f, 5.0f);

        if (_numberDrinksPending == 0)
            _data.npcState = NPCState.Drunk;

        _idleMode = true;
    }

    private void GotToDestinationDrunk()
    {
        _idleTimer = UnityEngine.Random.Range(0.2f, _data.drunkType != DrunkType.Puker ? 3.0f : 0.8f);
        _idleMode = true;
    }

    private void GotBeer()
    {
        _data.npcState = NPCState.Fine;
        _numberDrinksPending = UnityEngine.Random.Range(2, 5); // Between 2 and 4 times to drink before becoming drunk
        _leaveFridge = true;
    }

    private void GoToPlayer()
    {
        var playerPos = GameHelper.Player.transform.position;
        _path = PathfinderHelper.Pathfinder.GetPath2(
            new Vector2Int((int)transform.position.x, (int)transform.position.y),
            new Vector2Int((int)playerPos.x, (int)playerPos.y));
    }

    private void GetRandomDestination(int min, int max)
    {
        int x = (int)transform.position.x;
        int y = (int)transform.position.y;

        var list = PathfinderHelper.Pathfinder._listedNodes.FindAll(n => n.walkable && ((Mathf.Abs(n.gridX - x) + Mathf.Abs(n.gridY - y)) < max));
        WorldTile destination =
            list[UnityEngine.Random.Range(0, list.Count)];

        _path = PathfinderHelper.Pathfinder.GetPath2(
            new Vector2Int((int)transform.position.x, (int)transform.position.y),
            new Vector2Int(destination.gridX, destination.gridY));
    }

    public void OnDrawGizmos()
    {
        if (_path != null)
        {
            foreach (var worldTile in _path)
            {
                Gizmos.DrawCube(worldTile.transform.position, Vector3.one / 3);
            }
        }
    }
}