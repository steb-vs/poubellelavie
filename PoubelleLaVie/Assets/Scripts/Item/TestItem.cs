using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class TestItem : MonoBehaviour, IUsable
{
    public Vector3 Position => transform.position;

    public bool IsHeavy => true;

    public void Drop(GameObject sender)
    {
        transform.parent = null;
        gameObject.SetActive(true);

        Debug.Log("Drop");
    }

    public void Take(GameObject sender)
    {
        transform.parent = sender.transform;
        transform.localPosition = Vector3.zero;
        gameObject.SetActive(false);
        Debug.Log("Take");
    }

    public void Use(GameObject sender)
    {
        Debug.Log("Use");
    }
}