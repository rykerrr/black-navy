using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] private Sound[] sounds;
    [SerializeField] private GameObject objToPlayAudioOn;

    private Sound currentSound = null;

    public void Awake()
    {
        if (!objToPlayAudioOn)
        {
            objToPlayAudioOn = new GameObject("Sound slave object");
            AudioSource sourc = objToPlayAudioOn.AddComponent<AudioSource>();
            sourc.dopplerLevel = 0f;
            sourc.spatialBlend = 1f;
            sourc.panStereo = 0f;
        }
    }

    private void Start()
    {
        // play beginning theme or smth?
    }

    private Sound FindSound(string soundName)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null)
        {
            Debug.LogWarning("Sound not found by said name, name: " + name);
            return null;
        }
        else
        {
            return s;
        }
    }

    #region play overloads
    public void Play(string name)
    {
        Sound s = FindSound(name);

        if (s == null)
        {
            return;
        }

        if (currentSound != null)
        {
            AudioSource oldSourc = objToPlayAudioOn.GetComponent<AudioSource>();
            oldSourc.Stop();
            oldSourc.clip = null;
        }

        AudioSource sourc = objToPlayAudioOn.GetComponent<AudioSource>();
        sourc.clip = s.clip;
        sourc.playOnAwake = false;
        currentSound = s;
        sourc.Play();
    }
    public void Play(string name, bool loop)
    {
        Sound s = FindSound(name);

        if (s == null)
        {
            return;
        }

        if (currentSound != null)
        {
            AudioSource oldSourc = objToPlayAudioOn.GetComponent<AudioSource>();
            oldSourc.Stop();
            oldSourc.clip = null;
        }

        AudioSource sourc = objToPlayAudioOn.GetComponent<AudioSource>();
        sourc.clip = s.clip;
        sourc.playOnAwake = false;
        sourc.loop = loop;
        currentSound = s;
        sourc.Play();
    }
    public void Play(string name, float volume = 1f)
    {
        Sound s = FindSound(name);

        if (s == null)
        {
            return;
        }

        if (currentSound != null)
        {
            AudioSource oldSourc = objToPlayAudioOn.GetComponent<AudioSource>();
            oldSourc.Stop();
            oldSourc.clip = null;
        }

        AudioSource sourc = objToPlayAudioOn.GetComponent<AudioSource>();
        sourc.clip = s.clip;
        sourc.playOnAwake = false;
        sourc.volume = volume;
        currentSound = s;
        sourc.Play();
    }
    public void Play(string name, float volume = 1f, float pitch = 1f)
    {
        Sound s = FindSound(name);

        if (s == null)
        {
            return;
        }

        if (currentSound != null)
        {
            AudioSource oldSourc = objToPlayAudioOn.GetComponent<AudioSource>();
            oldSourc.Stop();
            oldSourc.clip = null;
        }

        AudioSource sourc = objToPlayAudioOn.GetComponent<AudioSource>();
        sourc.clip = s.clip;
        sourc.playOnAwake = false;
        sourc.volume = volume;
        sourc.pitch = pitch;
        currentSound = s;
        sourc.Play();
    }
    public void Play(string name, float volume = 1f, float pitch = 1f, bool loop = false)
    {
        Sound s = FindSound(name);

        if (s == null)
        {
            return;
        }

        if (currentSound != null)
        {
            AudioSource oldSourc = objToPlayAudioOn.GetComponent<AudioSource>();
            oldSourc.Stop();
            oldSourc.clip = null;
        }

        AudioSource sourc = objToPlayAudioOn.GetComponent<AudioSource>();
        sourc.clip = s.clip;
        sourc.playOnAwake = false;
        sourc.volume = volume;
        sourc.pitch = pitch;
        sourc.loop = loop;
        currentSound = s;
        sourc.Play();
    }
    public void PlayEnviroSound(GameObject objToPlayOn, string name, float volume = 1f, float pitch = 1f, bool loop = false)
    {
        Sound s = FindSound(name);

        if (s == null)
        {
            return;
        }

        AudioSource sourc = objToPlayOn.AddComponent<AudioSource>();

        sourc.dopplerLevel = 0;
        sourc.spatialBlend = 1f;
        sourc.panStereo = 0f;

        sourc.clip = s.clip;
        sourc.playOnAwake = false;
        sourc.volume = volume;
        sourc.pitch = pitch;
        sourc.loop = loop;
        currentSound = s;
        sourc.Play();
    }
    #endregion
}
#pragma warning restore 0649