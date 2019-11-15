using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PukeComponent : Garbage, ISpeedModifier, IUsable
{
    public float SpeedModifier => 0.25f;
    
    public Vector3 Position => transform.position;

    public bool IsHeavy => false;

    public bool Drop(GameObject sender)
    {
        return true;
    }

    public bool Take(GameObject sender)
    {
        PlayerDataComponent data = sender.GetComponent<PlayerDataComponent>();

        if (data == null)
            return false;

        if (data.trashCount <= data.trashLimit)
        {
            worldTile.walkable = true;
            Destroy(gameObject);
            data.trashCount++;
        }

        return false;
    }

    public void Use(GameObject sender)
    {
    }
}
