using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] private Sound[] sounds;
    [SerializeField] private GameObject objToPlayAudioOn;
    [SerializeField] private AudioSource explosionAudioObj;

    private Sound currentSound = null;

    protected override void Awake()
    {
        if (!objToPlayAudioOn)
        {
            objToPlayAudioOn = new GameObject("Theme song sound slave object");
            AudioSource sourc = objToPlayAudioOn.AddComponent<AudioSource>();
            sourc.dopplerLevel = 0f;
            sourc.spatialBlend = 0.5f;
            sourc.panStereo = 0f;
        }
    }

    private void Start()
    {
        // play beginning theme or smth?
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            objToPlayAudioOn.GetComponent<AudioSource>().Stop(); // maybe turn this into a button or smth?
        }
    }

    private Sound FindSound(string soundName)
    {
        Sound s = Array.Find(sounds, sound => sound.name == soundName);

        if (s == null)
        {
            LogUtils.DebugLogWarning("Sound not found by said name, name: " + soundName);
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
    public void PlayEnviroSound(GameObject soundObj, string name, float dist, float volume = 1f, float pitch = 1f, bool loop = false)
    {
        Sound s = FindSound(name);

        if (s == null)
        {
            return;
        }

        AudioSource sourc = null;

        if (objToPlayAudioOn.GetComponents<AudioSource>() != null)
        {
            AudioSource[] possibleSources = soundObj.GetComponents<AudioSource>();

            foreach (AudioSource source in possibleSources)
            {
                if (!source.isPlaying)
                {
                    sourc = source;
                    break;
                }
            }
        }

        if (sourc == null)
        {
            sourc = soundObj.AddComponent<AudioSource>();
        }

        sourc.dopplerLevel = 0;
        sourc.spatialBlend = 1f;
        sourc.panStereo = 0f;

        sourc.maxDistance = dist;
        sourc.clip = s.clip;
        sourc.playOnAwake = false;
        sourc.volume = volume;
        sourc.pitch = pitch;
        sourc.loop = loop;
        currentSound = s;
        sourc.Play();
    }
    public void PlayEnviroSound(GameObject soundObj, string name, float dist, bool loop)
    {
        Sound s = FindSound(name);

        if (s == null)
        {
            return;
        }

        AudioSource sourc = null;

        if (objToPlayAudioOn.GetComponents<AudioSource>() != null)
        {
            AudioSource[] possibleSources = soundObj.GetComponents<AudioSource>();

            foreach (AudioSource source in possibleSources)
            {
                if (!source.isPlaying)
                {
                    sourc = source;
                    break;
                }
            }
        }

        if (sourc == null)
        {
            sourc = soundObj.AddComponent<AudioSource>();
        }

        sourc.dopplerLevel = 0;
        sourc.spatialBlend = 1f;
        sourc.panStereo = 0f;

        sourc.maxDistance = dist;
        sourc.clip = s.clip;
        sourc.playOnAwake = false;
        sourc.loop = loop;
        currentSound = s;
        sourc.Play();
    }
    public void PlayEnviroSound(Vector3 position, string name, float dist, float volume = 1f, float pitch = 1f, bool loop = false)
    {
        Sound s = FindSound(name);

        if (s == null)
        {
            return;
        }

        AudioSource sourc = new GameObject("sound").AddComponent<AudioSource>();
        sourc.transform.position = position;

        if (!loop)
        {
            Destroy(sourc.gameObject, sourc.clip.length + 1f);
        }

        sourc.dopplerLevel = 0;
        sourc.spatialBlend = 1f;
        sourc.panStereo = 0f;

        sourc.maxDistance = dist;
        sourc.clip = s.clip;
        sourc.playOnAwake = false;
        sourc.volume = volume;
        sourc.pitch = pitch;
        sourc.loop = loop;
        currentSound = s;
        sourc.Play();
    }
    public void PlayEnviroSound(Vector3 position, string name, float dist, bool loop)
    {
        Sound s = FindSound(name);

        if (s == null)
        {
            return;
        }

        AudioSource sourc = new GameObject("sound").AddComponent<AudioSource>();
        sourc.transform.position = position;

        if (!loop)
        {
            Destroy(sourc.gameObject, sourc.clip.length + 1f);
        }

        sourc.dopplerLevel = 0;
        sourc.spatialBlend = 1f;
        sourc.panStereo = 0f;

        sourc.maxDistance = dist;
        sourc.clip = s.clip;
        sourc.playOnAwake = false;
        sourc.loop = loop;
        currentSound = s;
        sourc.Play();
    }
    public void PlayExplosion(Vector3 position, AudioSource sound)
    {
        explosionAudioObj.Stop();

        explosionAudioObj.transform.position = position;

        explosionAudioObj.clip = sound.clip;
        explosionAudioObj.volume = sound.volume;
        explosionAudioObj.pitch = sound.pitch;
        explosionAudioObj.spatialBlend = sound.spatialBlend;
        explosionAudioObj.dopplerLevel = sound.dopplerLevel;
        explosionAudioObj.maxDistance = sound.maxDistance;
        explosionAudioObj.minDistance = sound.minDistance;
        explosionAudioObj.priority = sound.priority;

        explosionAudioObj.Play();
    }
    #endregion
}
#pragma warning restore 0649