using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private float _prctCopsBar;

    public float decrCopsBarOverTime;
    public float timeScale;
    [HideInInspector]public GameObject player;
    [HideInInspector] public PlayerComponent playerComponent;

    public GameObject canvasGameOver;
    
    // When instiated, this object is stored in the GameHelper
    private void Awake()
    {
        GameHelper.GM = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        _prctCopsBar = 0;
        decrCopsBarOverTime = 3F;
        timeScale = 1;
    }

    // Update is called once per frame
    void Update()
    {
        float decrCopsBar = decrCopsBarOverTime * Time.deltaTime * timeScale;
        _prctCopsBar -= decrCopsBar;
        if (_prctCopsBar >= 100)
        {
            // End game
            timeScale = 0;
            canvasGameOver.SetActive(true);
            return;
        }

        // Decrement automatically the cops bar
        _prctCopsBar = (_prctCopsBar < 0) ? 0 : _prctCopsBar;

        // Setting / Unsetting pause ?
        if (Input.GetButtonDown(InputHelper.PAUSE))
            timeScale = 1 - timeScale;
    }

    public void AddPrctUntilCops(float toAdd)
    {
        _prctCopsBar += toAdd;
    }

    public float GetPrctUntilCops()
    {
        return _prctCopsBar;
    }
}
