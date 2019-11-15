using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Fridge : MonoBehaviour
{
    public Sprite openSprite;
    public Sprite closeSprite;

    private SpriteRenderer _renderer;
    private HashSet<NPCComponent> _closeNPCs;

    private void Start()
    {
        _closeNPCs = new HashSet<NPCComponent>();
        _renderer = transform.GetChild(0).GetComponent<SpriteRenderer>();

        _renderer.sprite = closeSprite;
    }

    private void Update()
    {
        if(_closeNPCs.Any())
            _renderer.sprite = openSprite;
        else
            _renderer.sprite = closeSprite;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        NPCComponent npc = other.GetComponent<NPCComponent>();

        if (npc == null)
            return;

        _closeNPCs.Add(npc);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        NPCComponent npc = other.GetComponent<NPCComponent>();

        if (npc == null)
            return;

        _closeNPCs.Remove(npc);
    }
}
