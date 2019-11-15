using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BackgroundMusicManager : MonoBehaviour
{
    public AudioClip[] musicParts;

    private AudioSource _audioSrc;
    private int clipIndex = 1;

    // Start is called before the first frame update
    void Start()
    {
        _audioSrc = GetComponent<AudioSource>();

        _audioSrc.clip = musicParts[0];
        _audioSrc.Play();

        GameHelper.GameManager.OnGameOver += GameOverAction;
    }

    private void GameOverAction()
    {
        _audioSrc.volume = 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        if(!_audioSrc.isPlaying)
        {
            if (clipIndex >= musicParts.Length)
                _audioSrc.clip = musicParts.Last();
            else
                _audioSrc.clip = musicParts[clipIndex++];

            _audioSrc.Play();
        }
    }
}
