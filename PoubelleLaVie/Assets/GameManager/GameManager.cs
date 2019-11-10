using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private float _prctCopsBar;

    public float decrCopsBarOverTime;
    public float timeScale;
    [HideInInspector]public GameObject player;
    [HideInInspector] public PlayerComponent playerComponent;

    public GameObject canvasGameOver;
    
    public Image trashImage;
    public Sprite[] trashImages;
    
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

        if (playerComponent.grabbedObjects == playerComponent.maxGrabbedObjects)
        {
            trashImage.sprite = trashImages[trashImages.Length - 1];
        }
        else
        {
            trashImage.sprite =
                trashImages[(playerComponent.grabbedObjects * (trashImages.Length - 1)) / playerComponent.maxGrabbedObjects];
        }
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
