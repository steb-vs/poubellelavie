using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class TestItem : MonoBehaviour, IUsable
{
    public Vector3 Position => transform.position;

    public bool IsHeavy => false;

    public void Drop(GameObject sender)
    {
        transform.parent = null;
        Debug.Log("Drop");
    }

    public void Take(GameObject sender)
    {
        transform.parent = sender.transform;
        transform.localPosition = Vector3.zero;
        Debug.Log("Take");
    }

    public void Use(GameObject sender)
    {
        Debug.Log("Use");
    }
}