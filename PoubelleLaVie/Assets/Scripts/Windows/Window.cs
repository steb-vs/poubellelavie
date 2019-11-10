using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Window : MonoBehaviour
{
    public Sprite[] leftWindow;
    public Sprite[] rightWindow;

    private SpriteRenderer l;
    private SpriteRenderer r;

    private void Start()
    {
        l = transform.GetChild(0).GetComponent<SpriteRenderer>();
        r = transform.GetChild(1).GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        l.sprite = leftWindow[1];
        r.sprite = rightWindow[1];
        GameHelper.GM.playerComponent.closeToWindow = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        l.sprite = leftWindow[0];
        r.sprite = rightWindow[0];
        GameHelper.GM.playerComponent.closeToWindow = false;
    }
}
