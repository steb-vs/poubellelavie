using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldTile : MonoBehaviour
{
    public float gCost;
    public float hCost;
    public int gridX, gridY;
    public bool walkable = true;
    public List<WorldTile> myNeighbours;
    public WorldTile parent;

    public float fCost
    {
        get { return gCost + hCost; }
    }
}
