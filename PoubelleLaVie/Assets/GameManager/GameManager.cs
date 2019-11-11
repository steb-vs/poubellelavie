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
	public Animator lightAtmoAnimator;

	public Image trashImage;
    public Sprite[] trashImages;
    public GameObject thrownTrash;

    public float score;
    
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
        score = 0;
    }

    // Update is called once per frame
    void Update()
    {
		lightAtmoAnimator.SetBool("bStateHasChanged", false);

		float decrCopsBar = decrCopsBarOverTime * Time.deltaTime * timeScale;
        if (_prctCopsBar >= 100)
        {
            // End game
            timeScale = 0;
            canvasGameOver.SetActive(true);
            return;
        }

		// Decrement automatically the cops bar
		AddPrctUntilCops(-decrCopsBar);

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

        score += Time.deltaTime * 3.5F * timeScale;

		
    }

	bool IsPrctInRange(float rangeBegin, float rangeEnd, float value)
	{
		return value >= rangeBegin && value <= rangeEnd;
	}

    public void AddPrctUntilCops(float toAdd)
	{
		float oldPrctbar = _prctCopsBar;

		_prctCopsBar += toAdd;
		_prctCopsBar = (_prctCopsBar < 0) ? 0 : _prctCopsBar;

		if (IsPrctInRange(0,0, _prctCopsBar) && ! IsPrctInRange(0, 0, oldPrctbar))
		{
			lightAtmoAnimator.SetInteger("StateNeighborGauge", 0);
			lightAtmoAnimator.SetBool("bStateHasChanged", true);
		}
		else if (IsPrctInRange(1, 33, _prctCopsBar) && !IsPrctInRange(1, 33, oldPrctbar))
		{
			lightAtmoAnimator.SetInteger("StateNeighborGauge", 1);
			lightAtmoAnimator.SetBool("bStateHasChanged", true);
		}
		else if (IsPrctInRange(34, 66, _prctCopsBar) && !IsPrctInRange(34, 66, oldPrctbar))
		{
			lightAtmoAnimator.SetInteger("StateNeighborGauge", 2);
			lightAtmoAnimator.SetBool("bStateHasChanged", true);
		}
		else if (IsPrctInRange(66, 99, _prctCopsBar) && !IsPrctInRange(66, 99, oldPrctbar))
		{
			lightAtmoAnimator.SetInteger("StateNeighborGauge", 3);
			lightAtmoAnimator.SetBool("bStateHasChanged", true);
		}
		else if (oldPrctbar < 100 && _prctCopsBar >= 100)
		{
			lightAtmoAnimator.SetInteger("StateNeighborGauge", 4);
			lightAtmoAnimator.SetBool("bStateHasChanged", true);
		}
		Debug.Log("Old : " + oldPrctbar + " new " + _prctCopsBar + "state" + lightAtmoAnimator.GetInteger("StateNeighborGauge"));


	}

	public float GetPrctUntilCops()
    {
        return _prctCopsBar;
    }
}
