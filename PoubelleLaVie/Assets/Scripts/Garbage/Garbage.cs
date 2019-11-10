using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Garbage : MonoBehaviour, ISpeedModifier
{
    public WorldTile worldTile;
    public Sprite[] sprites;

    public float SpeedModifier => 0.25f;

    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = sprites[UnityEngine.Random.Range(0, sprites.Length)];
    }
}
