﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NPCSoundManager : MonoBehaviour
{
    public AudioClip[] drunkSounds;
    public float rangeMin;
    public float rangeMax;
    public float minTime;

    private NPCBehaviour _behavior;
    private float _targetTime = 0;
    private float _currentTime = 0;
    private AudioSource _audioSrc;

    // Start is called before the first frame update
    private void Start()
    {
        _behavior = GetComponent<NPCBehaviour>();
        _audioSrc = GetComponent<AudioSource>();
        _currentTime = minTime;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!_behavior.globalState.HasFlag(GlobalState.DRUNK))
            return;

        _currentTime += Time.deltaTime;

        if (_currentTime < minTime + _targetTime)
            return;

        _targetTime = Random.Range(rangeMin, rangeMax);

        _audioSrc.clip = GetRandomClip(_behavior.drunkType.ToString());
        _audioSrc.Play();
    }

    private AudioClip GetRandomClip(string nameStart)
    {
        List<AudioClip> candidates = drunkSounds
                .Where(x => x.name.ToUpper().StartsWith(nameStart.ToUpper()))
                .ToList();

        return candidates[Random.Range(0, candidates.Count)];
    }
}
