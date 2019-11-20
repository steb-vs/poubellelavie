using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowComponent : MonoBehaviour
{
    public Sprite openLeftSprite;
    public Sprite openRightSprite;
    public Sprite closeSprite;

    private SpriteRenderer _leftPartRenderer;
    private SpriteRenderer _rightPartRenderer;

    private void Start()
    {
        _leftPartRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        _rightPartRenderer = transform.GetChild(1).GetComponent<SpriteRenderer>();

        _leftPartRenderer.sprite = closeSprite;
        _rightPartRenderer.sprite = closeSprite;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerDataComponent data = other.GetComponent<PlayerDataComponent>();

        if (data == null)
            return;

        _leftPartRenderer.sprite = openLeftSprite;
        _rightPartRenderer.sprite = openRightSprite;

        data.closeWindows.Add(this);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerDataComponent data = other.GetComponent<PlayerDataComponent>();

        if (data == null)
            return;

        _leftPartRenderer.sprite = closeSprite;
        _rightPartRenderer.sprite = closeSprite;

        data.closeWindows.Remove(this);
    }
}
