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

    private NPCComponent _behavior;
    private float _targetTime = 0;
    private float _currentTime = 0;
    private AudioSource _audioSrc;
    private bool _falling;
    private bool _wasPlaying;

    // Start is called before the first frame update
    private void Start()
    {
        _behavior = GetComponent<NPCComponent>();
        _audioSrc = GetComponent<AudioSource>();
        _currentTime = minTime;

        _behavior.OnFall += NPCFallAction;
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

        _falling = true;

        GameHelper.GameManager.OnGameOver -= GameOverAction;
    }

    // Update is called once per frame
    private void Update()
    {
        if(_wasPlaying && !_audioSrc.isPlaying)
        {
            GameHelper.GameManager.npcSoundPlayingCount--;
            _wasPlaying = false;
        }

        if (_falling)
            return;

        if (!_behavior.globalState.HasFlag(GlobalState.DRUNK))
            return;

        _currentTime += Time.deltaTime;

        if (GameHelper.GameManager.npcSoundPlayingCount >= GameHelper.GameManager.maxNpcSounds || _currentTime < minTime + _targetTime)
            return;

        _targetTime = Random.Range(rangeMin, rangeMax);
        _currentTime = 0;

        _audioSrc.clip = drunkSounds.GetRandom(_behavior.drunkType.ToString());
        _audioSrc.Play();
        _wasPlaying = true;

        GameHelper.GameManager.npcSoundPlayingCount++;
    }
}

public static class AudioClipExtensions
{
    public static AudioClip GetRandom(this AudioClip[] input, string startsWith = null)
    {
        List<AudioClip> candidates;

        if(!string.IsNullOrEmpty(startsWith))
        {
            candidates = input
                .Where(x => x.name.ToUpper().StartsWith(startsWith.ToUpper()))
                .ToList();
        }
        else
        {
            candidates = input.ToList();
        }

        return candidates[Random.Range(0, candidates.Count)];
    }
}
