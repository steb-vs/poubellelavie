using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Extensions
{
    public static float Wrap(this float input, float min, float max)
    {
        return
            input < min ? min :
            input > max ? max :
            input;
    }

    public static bool IsBetween(this float input, float min, float max)
    {
        return input >= min && input < max;
    }

    public static AudioClip GetRandom(this AudioClip[] input, string startsWith = null)
    {
        List<AudioClip> candidates;

        if (!string.IsNullOrEmpty(startsWith))
        {
            candidates = input
                .Where(x => x.name.ToUpper().StartsWith(startsWith.ToUpper()))
                .ToList();
        }
        else
        {
            candidates = input.ToList();
        }

        return candidates[UnityEngine.Random.Range(0, candidates.Count)];
    }
}
