using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviour
{
    public List<GameObject> pooledObjects;
    public int maxPooledObjects;
    public int numberOfNewObjects;

    #region Init

    private void Start()
    {
        _pools = new List<List<GameObject>>();
        for (var i = 0; i < pooledObjects.Count; ++i)
        {
            _pools.Add(new List<GameObject>());
        }

        CreateObjects(maxPooledObjects);
    }

    #endregion

    #region Utils

    private void CreateObjects(int number)
    {
        for (var i = 0; i < pooledObjects.Count; ++i)
        {
            CreateObjects(number, i);
        }
    }

    private void CreateObjects(int number, int index)
    {
        for (var i = 0; i < number; ++i)
        {
            var obj = Instantiate(pooledObjects[index]);
            obj.name = pooledObjects[index].name;
            obj.SetActive(false);
            _pools[index].Add(obj);
        }
    }

    public GameObject AskObject(int index = 0)
    {
        if (_pools.Count < index)
        {
            return null;
        }

        if (_pools[index].Count <= 0)
        {
            CreateObjects(numberOfNewObjects, index);
        }

        var obj = _pools[index][0];
        _pools[index].RemoveAt(0);
        return obj;
    }


    public void ReleaseObject(GameObject objectToRelease, int index)
    {
        if (!objectToRelease || _pools.Count < index || _pools[index].Contains(objectToRelease)) return;

        objectToRelease.SetActive(false);
        _pools[index].Add(objectToRelease);
    }

    public void ReleaseObject(GameObject objectToRelease)
    {
        ReleaseObject(objectToRelease, 0);
    }

    #endregion

    private List<List<GameObject>> _pools;
}