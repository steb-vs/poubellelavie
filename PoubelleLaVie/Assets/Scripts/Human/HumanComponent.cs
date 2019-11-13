using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanComponent : MonoBehaviour
{
    private IController<HumanAction> _controller;
    private Rigidbody2D _body;
    private Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        _controller = GetComponent<IController<HumanAction>>();
        _body = GetComponent<Rigidbody2D>();
        _animator = playerSprite.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
