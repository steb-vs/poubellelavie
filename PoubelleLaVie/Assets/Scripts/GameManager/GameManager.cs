using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private float _prctUntilCops;

    // When instiated, this object is stored in the GameHelper
    private void Awake()
    {
        GameHelper.GM = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        _prctUntilCops = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (_prctUntilCops >= 100)
        {
            // End game
            return;
        }
    }

    public void AddPrctUntilCops(float toAdd)
    {
        _prctUntilCops += toAdd;
    }

    public float GetPrctUntilCops()
    {
        return _prctUntilCops;
    }
}
