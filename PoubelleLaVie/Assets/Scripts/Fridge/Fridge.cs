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
        f.sprite = fridge[0];
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("NPC"))
            return;
        f.sprite = fridge[1];
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("NPC"))
            return;
        f.sprite = fridge[0];
        
    }
}
