using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class GameManager : MonoBehaviour
{
    public GameObject canvasGameOver;
    public GameObject canvasUI;
	public Animator lightAtmoAnimator;
	public Image trashImage;
    public Sprite[] trashImages;
    public GameObject thrownTrash;
    public TextMeshProUGUI scoreMesh;
    public GameObject playerPrefab;
    public TextMeshProUGUI endScore;

    [HideInInspector] public GameDataComponent data;

    public event Action OnGameOver;

	private AudioSource[] _audioSrcs;
    private PlayerDataComponent _playerData;

    public void UpdateCopGauge()
    {
        // Decrement automatically the cops bar
        float coeff = Time.deltaTime * data.timeScale;
        float delta = data.dancerCount > 0 ? data.incrCopGaugeOverTime * coeff : -data.decrCopGaugeOverTime * coeff;
        float oldCopGauge = data.copGauge;

        data.copGauge += delta;
        data.copGauge = data.copGauge.Wrap(0, 100);

        if (data.copGauge.IsBetween(0, 1) && !oldCopGauge.IsBetween(0, 1))
        {
            lightAtmoAnimator.SetInteger("StateNeighborGauge", 0);
            lightAtmoAnimator.SetBool("bStateHasChanged", true);
        }
        else if (data.copGauge.IsBetween(1, 33) && !oldCopGauge.IsBetween(1, 33))
        {
            lightAtmoAnimator.SetInteger("StateNeighborGauge", 1);
            lightAtmoAnimator.SetBool("bStateHasChanged", true);

            if (data.copGauge > oldCopGauge)
                _audioSrcs[2].Play();
        }
        else if (data.copGauge.IsBetween(33, 66) && !oldCopGauge.IsBetween(33, 66))
        {
            if (data.copGauge > oldCopGauge)
                _audioSrcs[2].Play();

            lightAtmoAnimator.SetInteger("StateNeighborGauge", 2);
            lightAtmoAnimator.SetBool("bStateHasChanged", true);
        }
        else if (data.copGauge.IsBetween(66, 100) && !oldCopGauge.IsBetween(66, 100))
        {
            if (data.copGauge > oldCopGauge)
                _audioSrcs[1].Play();

            lightAtmoAnimator.SetInteger("StateNeighborGauge", 3);
            lightAtmoAnimator.SetBool("bStateHasChanged", true);
        }
    }

    // When instiated, this object is stored in the GameHelper
    private void Awake()
    {
        GameHelper.GameManager = this;
        CreatePlayer();
    }

    // Start is called before the first frame update
    private void Start()
    {
        data = GetComponent<GameDataComponent>();
        _audioSrcs = GetComponents<AudioSource>();
    }

    private void CreatePlayer()
    {
        GameHelper.Player = Instantiate(playerPrefab, new Vector3(5, 5, 5), Quaternion.identity);
        _playerData = GameHelper.Player.GetComponent<PlayerDataComponent>();
    }

    // Update is called once per frame
    private void Update()
    {
		lightAtmoAnimator.SetBool("bStateHasChanged", false);

        UpdateCopGauge();

        if (data.copGauge >= 100 && !data.gameOver)
        {
            data.gameOver = true;
			lightAtmoAnimator.SetInteger("StateNeighborGauge", 4);
			lightAtmoAnimator.SetBool("bStateHasChanged", true);
			_audioSrcs[0].Play();

            OnGameOver?.Invoke();

            // End game
            data.timeScale = 0;
            canvasGameOver.SetActive(true);
            canvasUI.SetActive(false);
            endScore.text = "Score: " + (int)data.score;
            return;
        }

        // Setting / Unsetting pause ?
        if (Input.GetButtonDown(InputHelper.PAUSE))
            data.paused = !data.paused;

        if(data.paused && data.timeScale > 0)
        {
            data.timeScale -= Time.deltaTime;
            data.timeScale = data.timeScale.Wrap(0, 1);
        }
        else if(!data.paused && data.timeScale < 1)
        {
            data.timeScale += Time.deltaTime;
            data.timeScale = data.timeScale.Wrap(0, 1);
        }

        if (_playerData.trashCount >= _playerData.trashLimit)
        {
            trashImage.sprite = trashImages[trashImages.Length - 1];
        }
        else
        {
            trashImage.sprite =
                trashImages[(_playerData.trashCount * (trashImages.Length - 1)) / _playerData.trashLimit];
        }

        // Update score
        data.score += (Time.deltaTime * data.timeScale) * 10;
        scoreMesh.text = "Score: " + data.score.ToString("000000000");
    }
}
