using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataComponent : MonoBehaviour
{
    public int dancerCount;

    public float copGauge;

    public float score;

    public bool gameOver;

    public bool paused;

    public int npcSoundPlayingCount;

    public int maxNpcSounds;

    public float decrCopGaugeOverTime;

    public float incrCopGaugeOverTime;

    public float timeScale;

    public int trashLimit;

    public GameDataComponent()
    {
        decrCopGaugeOverTime = 10.0f;
        incrCopGaugeOverTime = 10.0f;
        maxNpcSounds = 1;
        dancerCount = 0;
        copGauge = 0.0f;
        score = 0;
        gameOver = false;
        paused = false;
        npcSoundPlayingCount = 0;
        trashLimit = 10;
        timeScale = 1;
    }
}
