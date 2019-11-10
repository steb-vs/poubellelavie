using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Garbage : MonoBehaviour
{
    public WorldTile worldTile;
    public Sprite[] sprites;


    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = sprites[UnityEngine.Random.Range(0, sprites.Length - 1)];
    }
}
