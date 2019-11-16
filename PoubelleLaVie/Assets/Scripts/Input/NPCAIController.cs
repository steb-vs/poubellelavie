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
    private Vector2 _targetDist;
    private Action _callback;
    private int _numberDrinksPending;
    private float _idleTimer;
    private bool _idleMode;
    private WorldTile _targetTile;

    private void Start()
    {
        _data = GetComponent<NPCDataComponent>();
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
        if (_data.grabbed || _data.falling)
            return;

        if (_data.npcState == NPCState.NeedDrinking)
        {
            if (_path == null) // Choose a random fridge, and go there to pick a beer
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
            if (_idleTimer <= 0.0f)
            {
                _idleMode = false;

                GetRandomDestination(2, 6);
                _callback = GotToDestinationFine;
            }

            // NPC is drinking
            if (_idleMode)
                _idleTimer -= Time.fixedDeltaTime * GameHelper.GameManager.data.timeScale;
        }

        else if (_data.npcState == NPCState.Drunk)
        {
            if (_idleTimer <= 0.0f)
            {
                _idleMode = false;

                if (_data.drunkType == DrunkType.Dancer)
                    GetRandomDestination(2, 15);
                else if(_data.drunkType == DrunkType.Puker)
                    GetRandomDestination(2, 13);
                else
                    GoToPlayer();

                _callback = GotToDestinationDrunk;
            }

            if (_idleMode)
                _idleTimer -= Time.fixedDeltaTime * GameHelper.GameManager.data.timeScale;
        }

        _horizontal = 0.0f;
        _vertical = 0.0f;

        // No path
        if (_path == null)
            return;

        // Ended deplacement
        if (!_path.Any())
        {
            _callback?.Invoke();
            _callback = null;
            OnNPCReachTarget?.Invoke(_targetTile, _data.npcState, _data.drunkType);
            _path = null;
            return;
        }

        _targetTile = _path.Last();

        _nextTile = _path[0];

        if (_nextTile.walkable == false)
        {
            _path = null;
            _nextTile = null;
            return;
        }

        _targetDist.x = _nextTile.gridX - transform.position.x;
        _targetDist.y = _nextTile.gridY - transform.position.y;

        if (_targetDist.magnitude < 0.1f)
        {
            if(_path.Count > 1)
                _nextTile.walkable = true;

            _nextTile = null;
            _path.RemoveAt(0);

            return;
        }

        if (_targetDist.x < -0.1f)
            _horizontal = -1.0f;
        else if (_targetDist.x > 0.1f)
            _horizontal = 1.0f;

        if (_targetDist.y > 0.1f)
            _vertical = 1.0f;
        else if (_targetDist.y < -0.1f)
            _vertical = -1.0f;

        //_nextTile.walkable = false;
    }

    private void GotToDestinationFine()
    {
        _idleTimer = _data.npcState == NPCState.Fine ? 2F : 1.5F; // Make the drinking action a little bit longer
        _numberDrinksPending--;

        if (_numberDrinksPending == 0)
            _data.npcState = NPCState.Drunk;

        _idleMode = true;
    }

    private void GotToDestinationDrunk()
    {
        _idleTimer = UnityEngine.Random.Range(0.2f, 3.0f);
        _idleMode = true;
    }

    private void GotBeer()
    {
        _data.npcState = NPCState.Fine;
        _numberDrinksPending = UnityEngine.Random.Range(2, 5); // Between 2 and 4 times to drink before becoming drunk
        _idleTimer = 0.5f; // Take 0.5 secs to grab a beer
        _idleMode = true;
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
}