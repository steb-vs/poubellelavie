using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private float _prctCopsBar;

    public float decrCopsBarOverTime;
    public float timeScale;
    [HideInInspector]public GameObject player;
    
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
        if (_prctCopsBar >= 100)
        {
            // End game
            return;
        }

        // Decrement automatically the cops bar
        _prctCopsBar = (decrCopsBar < 0) ? 0 : decrCopsBar;

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
