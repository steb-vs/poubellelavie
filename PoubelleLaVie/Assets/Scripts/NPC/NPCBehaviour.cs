﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBehaviour : MonoBehaviour
{
    private BoxCollider2D _boxCollider;
    private Animator _animatorNPC;

    private GlobalState _globalState;
    private ActionState _actionState;
    public DrunkState drunkType;

    public float prctUntilDrunk;
    public float incrDrunkOverTime;

    public float incrCopsBarOverTime;

    private bool isWaling;

    // Start is called before the first frame update
    void Start()
    {
        _boxCollider = GetComponent<BoxCollider2D>();
        _animatorNPC = GetComponent<Animator>();

        Random rnd = new Random();
        drunkType = (DrunkState) Random.Range(0, (int) DrunkState.TOTAL_DRUNK_STATES);
        _globalState = GlobalState.NEED_DRINKING;

        prctUntilDrunk = 0; // Over 100 (100%), the NPC becomes drunk
        incrDrunkOverTime = 20.5F; // How much the NPC gets drunk each seconds

        incrCopsBarOverTime = 5.8F; // How much the NPC fill the cops bar each seconds

        isWaling = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (_globalState.HasFlag(GlobalState.BEING_CARRIED))
        {
            print("DO THE THINGS WHILE I AM CARRIED");
            return; // We stop here
        }

        // NPC is not being carried by the player: he can do his things
        switch (_globalState)
        {
            case GlobalState.NEED_DRINKING:
                print("I NEED DRINKING");
                _globalState = GlobalState.FINE;
                break;
            case GlobalState.FINE:
                _boxCollider.enabled = false;

                // Increments the drunk bar
                prctUntilDrunk += incrDrunkOverTime * Time.deltaTime * GameHelper.GM.timeScale;
                if (prctUntilDrunk >= 100)
                    _globalState = GlobalState.DRUNK;

                print("CURRENT DRINK BAR: " + prctUntilDrunk);
                break;
            case GlobalState.DRUNK:
                HandleDrunk();
                break;
            default:
                print("Unexpected global state of an NPC !");
                print("Actual global state value: " + _globalState);
                break;
        }

        // Set variables for the state machine
        _animatorNPC.SetBool("isWalking", _actionState == ActionState.WALKING);
        _animatorNPC.SetBool("isDrinking", _actionState == ActionState.DRINKING);
        _animatorNPC.SetBool("isDrunk", _globalState == GlobalState.FINE);
        _animatorNPC.SetBool("isTrash", _globalState == GlobalState.DRUNK);
        _animatorNPC.SetInteger("drunkState", (int) drunkType);
    }

    // Called when NPC's _globalState is DRUNK
    private void HandleDrunk()
    {
        switch (drunkType)
        {
            case DrunkState.DANCER:
                GameHelper.GM.AddPrctUntilCops(incrCopsBarOverTime * Time.deltaTime * GameHelper.GM.timeScale);
                break;
            case DrunkState.LOVER:
                break;
            case DrunkState.PUKER:
                break;
            default:
                print("Unexpected drunk state of an NPC !");
                print("Actual drunk state value:" + drunkType);
                break;
        }
    }

    /*
     * Add to the NPC the state of 'BEING_CARRIED'.
     * Doesn't change the inner state.
     */
    public void ToCarried()
    {
        _globalState |= GlobalState.BEING_CARRIED;
    }

    /*
     * Retire to the NPC the state of 'BEING_CARRIED'.
     */
    public void ToTheGround()
    {
        _globalState ^= GlobalState.BEING_CARRIED;
    }
}
