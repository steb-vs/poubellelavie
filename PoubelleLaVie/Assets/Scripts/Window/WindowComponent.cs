using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowComponent : MonoBehaviour
{
    public Sprite leftWindowSprite;
    public Sprite rightWindowSprite;

    private SpriteRenderer _leftPartRenderer;
    private SpriteRenderer _rightPartRenderer;

    private void Start()
    {
        _leftPartRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        _rightPartRenderer = transform.GetChild(1).GetComponent<SpriteRenderer>();

        _leftPartRenderer.sprite = leftWindowSprite;
        _rightPartRenderer.sprite = rightWindowSprite;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerDataComponent data = other.GetComponent<PlayerDataComponent>();

        if (data == null)
            return;

        _leftPartRenderer.sprite = leftWindowSprite;
        _rightPartRenderer.sprite = rightWindowSprite;

        data.closeWindows.Add(this);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerDataComponent data = other.GetComponent<PlayerDataComponent>();

        if (data == null)
            return;

        _leftPartRenderer.sprite = leftWindowSprite;
        _rightPartRenderer.sprite = rightWindowSprite;

        data.closeWindows.Remove(this);
    }
}
