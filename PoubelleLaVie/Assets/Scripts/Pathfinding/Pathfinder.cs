using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Pathfinder : MonoBehaviour
{
    [HideInInspector] public List<WorldTile> fridges;
    [HideInInspector] public List<WorldTile> entries;

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(Vector3.zero, Vector3.one);
    }

    private void Awake()
    {
        PathfinderHelper.Pathfinder = this;
    }

    public void SetNodes(GameObject[,] nodes, int gridBoundX, int gridBoundY)
    {
        _nodes = new WorldTile[gridBoundX + 1, gridBoundY + 1];
        _listedNodes = new List<WorldTile>();
        Debug.Log(gridBoundX);
        Debug.Log(gridBoundY);
        for (int x = 0; x <= gridBoundX; ++x)
        {
            for (int y = 0; y < gridBoundY; ++y)
            {
                _nodes[x, y] = null;
                if (nodes[x, y])
                {
                    _nodes[x, y] = nodes[x, y].GetComponent<WorldTile>();
                    if (_nodes[x, y].walkable)
                        _listedNodes.Add(_nodes[x, y]);
                }
            }
        }

        _gridBoundX = gridBoundX;
        _gridBoundY = gridBoundY;
    }

    private WorldTile GetBestNode(List<WorldTile> list)
    {
        WorldTile bestTile = list[0];

        for (int i = 1; i < list.Count; ++i)
        {
            if (list[i].fCost < bestTile.fCost)
                bestTile = list[i];
        }

        return bestTile;
    }

    public List<WorldTile> GetPath2(Vector2Int start, Vector2Int end)
    {
        WorldTile goalNode = (from node in _listedNodes where (node.gridX == end.x && node.gridY == end.y) select node)
            .First();
        
        Debug.Log($"{start.x} - {start.y}");
        
        WorldTile startNode =
            (from node in _listedNodes where (node.gridX == start.x && node.gridY == start.y) select node).First();

        var openList = new List<WorldTile>();
        var closedList = new List<WorldTile>();

        int g = 0;

        openList.Add(startNode);

        foreach (var listedNode in _listedNodes)
        {
            listedNode.parent = null;
            listedNode.gCost = 1;
            listedNode.hCost = 0;
        }

        while (openList.Count > 0)
        {
            var lowest = openList.Min(l => l.fCost);
            var current = openList.First(l => l.fCost == lowest);


            closedList.Add(current);
            openList.Remove(current);

            if (closedList.FirstOrDefault(l => l.gridX == goalNode.gridX && l.gridY == goalNode.gridY) != null)
            {
                var path = new List<WorldTile>();
                while (current.parent)
                {
                    path.Add(current);
                    current = current.parent;
                }

                path.Reverse();
                return path;
            }

            var adjacentSquares = GetWalkableAdjacentSquares(current);
            g++;

            foreach (var adjacentSquare in adjacentSquares)
            {
                if (closedList.FirstOrDefault(l => l.gridX == adjacentSquare.gridX
                                                   && l.gridY == adjacentSquare.gridY) != null)
                    continue;

                // if it's not in the open list...
                if (openList.FirstOrDefault(l => l.gridX == adjacentSquare.gridX
                                                 && l.gridY == adjacentSquare.gridY) == null)
                {
                    // compute its score, set the parent
                    adjacentSquare.gCost = g;
                    adjacentSquare.hCost = ManhattanDistance(adjacentSquare, goalNode);
                    adjacentSquare.parent = current;

                    // and add it to the open list
                    openList.Insert(0, adjacentSquare);
                }
                else
                {
                    // test if using the current G score makes the adjacent square's F score
                    // lower, if yes update the parent because it means it's a better path
                    if (g + adjacentSquare.hCost < adjacentSquare.fCost)
                    {
                        adjacentSquare.gCost = g;
                        adjacentSquare.parent = current;
                    }
                }
            }
        }

        Debug.Log("????");
        return null;
    }

    List<WorldTile> GetWalkableAdjacentSquares(WorldTile current)
    {
        return current.myNeighbours.FindAll(n => n.walkable == true);
    }


    public float ManhattanDistance(WorldTile l, WorldTile r)
    {
        return Mathf.Abs(r.gridX - l.gridX) + Mathf.Abs(r.gridY - l.gridY);
    }

    public List<WorldTile> GetPath(Vector2Int start, Vector2Int end)
    {
        WorldTile goalNode = (from node in _listedNodes where (node.gridX == end.x && node.gridY == end.y) select node)
            .First();
        WorldTile startNode =
            (from node in _listedNodes where (node.gridX == start.x && node.gridY == start.y) select node).First();

        var open = new List<WorldTile>();
        var closed = new List<WorldTile>();

        Debug.Log($"AA : {_listedNodes.Count}");
        foreach (var listedNode in _listedNodes)
        {
            listedNode.parent = null;
            listedNode.gCost = 1;
            listedNode.hCost = 0;
        }

        open.Add(startNode);

        int tries = 0;

        while (open.Count > 0)
        {
            var node = GetBestNode(open);

            if (node == goalNode)
            {
                List<WorldTile> foundPath = new List<WorldTile>();
                foundPath.Clear();
                while (node.parent && tries < 20)
                {
                    tries++;
                    foundPath.Add(node);
                    node = node.parent;
                }

                foundPath.Reverse();

                return foundPath;
            }

            open.Remove(node);
            closed.Add(node);

            foreach (var nodeMyNeighbour in node.myNeighbours)
            {
                if (nodeMyNeighbour.walkable == false)
                {
                    closed.Add(nodeMyNeighbour);
                    continue;
                }

                float gScore = node.gCost + 1;
                float hScore = ManhattanDistance(nodeMyNeighbour, goalNode);
                float fScore = gScore + hScore;

                if (closed.Contains(nodeMyNeighbour) && fScore >= nodeMyNeighbour.fCost)
                {
                    continue;
                }

                if (!open.Contains(nodeMyNeighbour) || fScore < nodeMyNeighbour.fCost)
                {
                    nodeMyNeighbour.parent = node;
                    nodeMyNeighbour.gCost = gScore;
                    nodeMyNeighbour.hCost = hScore;
                    if (!open.Contains(nodeMyNeighbour))
                    {
                        open.Add(nodeMyNeighbour);
                    }
                }
            }
        }
        return null;
    }

    public int getGridBoundX
    {
        get { return _gridBoundX; }
    }

    public int getGridBoundY
    {
        get { return _gridBoundY; }
    }

    private WorldTile[,] _nodes;
    private int _gridBoundX;
    private int _gridBoundY;
    public List<WorldTile> _listedNodes;
}