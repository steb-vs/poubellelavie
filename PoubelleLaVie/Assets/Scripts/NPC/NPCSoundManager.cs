using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NPCSoundManager : MonoBehaviour
{
    public AudioClip[] drunkSounds;
    public AudioClip[] fallSounds;
    public float rangeMin;
    public float rangeMax;
    public float minTime;

    private NPCComponent _npc;
    private NPCDataComponent _data;
    private float _targetTime;
    private float _currentTime;
    private AudioSource _audioSrc;
    private bool _wasPlaying;

    // Start is called before the first frame update
    private void Start()
    {
        _npc = GetComponent<NPCComponent>();
        _audioSrc = GetComponent<AudioSource>();
        _data = GetComponent<NPCDataComponent>();

        _currentTime = minTime;
        _targetTime = 0;

        _npc.OnFall += NPCFallAction;
        GameHelper.GameManager.OnGameOver += GameOverAction;
    }

    private void GameOverAction()
    {
        _audioSrc.volume = 0.5f;
    }

    private void NPCFallAction(GameObject obj, NPCComponent behavior)
    {
        _audioSrc.clip = fallSounds.GetRandom();
        _audioSrc.Play();

        GameHelper.GameManager.OnGameOver -= GameOverAction;
    }

    // Update is called once per frame
    private void Update()
    {
        if(_wasPlaying && !_audioSrc.isPlaying)
        {
            GameHelper.GameManager.data.npcSoundPlayingCount--;
            _wasPlaying = false;
        }

        if (_data.falling)
            return;

        if (_data.npcState != NPCState.Drunk)
            return;

        _currentTime += Time.deltaTime;

        if (GameHelper.GameManager.data.npcSoundPlayingCount >= GameHelper.GameManager.data.maxNpcSounds || _currentTime < minTime + _targetTime)
            return;

        _targetTime = Random.Range(rangeMin, rangeMax);
        _currentTime = 0;

        _audioSrc.clip = drunkSounds.GetRandom(_data.drunkType.ToString());
        _audioSrc.Play();
        _wasPlaying = true;

        GameHelper.GameManager.data.npcSoundPlayingCount++;
    }
}