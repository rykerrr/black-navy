﻿using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
#pragma warning disable 0649
public class Sound
{
    public string name;

    public AudioClip clip;

    public bool loop;

    [Range(0f, 1f)] public float volume;
    [Range(0.1f, 5f)] public float pitch;
    [HideInInspector] public AudioSource source;
}
#pragma warning restore 0649