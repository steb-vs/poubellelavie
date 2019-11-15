using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCAIController : Controller<HumanAction>, IController<NPCAction>
{
    private NPCDataComponent _data;
    private bool _gotPath;
    private List<WorldTile> _path;

    private void Start()
    {
        _data = GetComponent<NPCDataComponent>();
    }

    public bool GetActionDown(NPCAction action)
    {
        throw new System.NotImplementedException();
    }

    public override bool GetActionDown(HumanAction action)
    {
        throw new System.NotImplementedException();
    }

    public bool GetActionUp(NPCAction action)
    {
        throw new System.NotImplementedException();
    }

    public override bool GetActionUp(HumanAction action)
    {
        throw new System.NotImplementedException();
    }

    public float GetActionValue(NPCAction action)
    {
        throw new System.NotImplementedException();
    }

    public override float GetActionValue(HumanAction action)
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateAI()
    {
        if (_data.npcState == NPCState.NeedDrinking)
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

        else if (_data.npcState == NPCState.Fine)
        {
            // Check if NPC is drunk
            if (numberDrinksPending == 0)
            {
                _data.npcState = NPCState.Drunk;
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
                timer -= Time.deltaTime * GameHelper.GameManager.data.timeScale;
        }

        else if (_data.npcState == NPCState.Drunk)
        {
            HandleDrunk();
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
        float toWalkX = ((distX * Time.deltaTime) / speed) * GameHelper.GameManager.data.timeScale;
        float toWalkY = ((distY * Time.deltaTime) / speed) * GameHelper.GameManager.data.timeScale;

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
}