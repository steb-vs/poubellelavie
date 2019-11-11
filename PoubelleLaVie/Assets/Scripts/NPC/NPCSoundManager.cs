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

    private NPCBehaviour _behavior;
    private float _targetTime = 0;
    private float _currentTime = 0;
    private AudioSource _audioSrc;
    private bool _falling;

    // Start is called before the first frame update
    private void Start()
    {
        _behavior = GetComponent<NPCBehaviour>();
        _audioSrc = GetComponent<AudioSource>();
        _currentTime = minTime;

        _behavior.OnFall += NPCFallAction;
    }

    private void NPCFallAction(GameObject obj, NPCBehaviour behavior)
    {
        _audioSrc.clip = fallSounds.GetRandom();
        _audioSrc.Play();

        _falling = true;
    }

    // Update is called once per frame
    private void Update()
    {
        if (_falling)
            return;

        if (!_behavior.globalState.HasFlag(GlobalState.DRUNK))
            return;

        _currentTime += Time.deltaTime;

        if (_currentTime < minTime + _targetTime)
            return;

        _targetTime = Random.Range(rangeMin, rangeMax);
        _currentTime = 0;

        _audioSrc.clip = drunkSounds.GetRandom(_behavior.drunkType.ToString());
        _audioSrc.Play();
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
