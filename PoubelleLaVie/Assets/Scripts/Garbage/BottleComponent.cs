using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BottleComponent : Garbage, IUsable
{
    public Vector3 Position => transform.position;

    public bool IsHeavy => false;

    public bool Drop(GameObject sender)
    {
        return true;
    }

    public bool Take(GameObject sender)
    {
        Debug.Log($"{GameHelper.GM.playerComponent.grabbedObjects} - {GameHelper.GM.playerComponent.maxGrabbedObjects}");
        if (GameHelper.GM.playerComponent.grabbedObjects <= GameHelper.GM.playerComponent.maxGrabbedObjects)
        {
            worldTile.walkable = true;
            Destroy(gameObject);
            GameHelper.GM.playerComponent.grabbedObjects++;    
        }
        return false;
    }

    public void Use(GameObject sender)
    {
    }
}