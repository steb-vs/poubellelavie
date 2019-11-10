using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fridge : MonoBehaviour
{
    public Sprite[] fridge;

    private SpriteRenderer f;

    private void Start()
    {
        f = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        f.sprite = fridge[1];
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        f.sprite = fridge[0];
        
    }
}
