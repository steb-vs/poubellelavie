using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Pathfinder : MonoBehaviour
{
    public void Update()
    {
        if (Input.GetKeyUp("k"))
        {
            GetPath(new Vector2Int(0, 0), new Vector2Int(8, 5));
        }
    }

    public void SetNodes(GameObject[,] nodes, int gridBoundX, int gridBoundY)
    {
        Debug.Log("COucou");
        _nodes = new WorldTile[gridBoundX, gridBoundY];
        _listedNodes = new List<WorldTile>();
        for (int x = 0; x < gridBoundX; ++x)
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

    public WorldTile GetBestNode(List<WorldTile> list)
    {
        WorldTile bestTile = list[0];

        for (int i = 1; i < list.Count; ++i)
        {
            if (list[i].fCost < bestTile.fCost)
                bestTile = list[i];
        }

        return bestTile;
    }

    public float ManhattanDistance(WorldTile l, WorldTile r)
    {
        return (r.gridX - l.gridX) + (r.gridY - l.gridY);
    }

    private List<WorldTile> _path = new List<WorldTile>();

    private void OnDrawGizmos()
    {
        foreach (var worldTile in _path)
        {
            Gizmos.DrawCube(worldTile.transform.position, Vector3.one / 3);
        }
    }

    public List<WorldTile> GetPath(Vector2Int start, Vector2Int end)
    {
        WorldTile goalNode = (from node in _listedNodes where (node.gridX == end.x && node.gridY == end.y) select node)
            .First();
        WorldTile startNode =
            (from node in _listedNodes where (node.gridX == start.x && node.gridY == start.y) select node).First();

        var open = new List<WorldTile>();
        var closed = new List<WorldTile>();

        open.Add(startNode);

        while (open.Count > 0)
        {
            var node = GetBestNode(open);

            if (node == goalNode)
            {
                _path.Clear();
                while (node.parent)
                {
                    _path.Add(node);
                    node = node.parent;
                }

                break;
            }

            open.Remove(node);
            closed.Add(node);

            foreach (var nodeMyNeighbour in node.myNeighbours)
            {
                if (nodeMyNeighbour.walkable == false) continue;
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

    private WorldTile[,] _nodes;
    private int _gridBoundX;
    private int _gridBoundY;
    private List<WorldTile> _listedNodes;
}