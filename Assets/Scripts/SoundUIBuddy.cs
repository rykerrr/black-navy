using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 0649
public class SoundUIBuddy : MonoBehaviour
{
    public struct AudioSettings
    {
        public string name;
        public float volume;
        public float pitch;
        public bool loop;
    }

    [Header("props")]
    [SerializeField] private GameObject musicMenu;
    [SerializeField] private Dropdown songPickDropdown;

    [Header("set by script, shown for debug")]
    [SerializeField] private AudioSettings curAudio;

    private SoundManager soundMngr;

    private void Start()
    {
        soundMngr = SoundManager.Instance;

        if (soundMngr == null)
        {
            Debug.Log("REEEEEEEEEEEEEE");
        }

        SetSong(0);
        ChangeVolume("1");
        ChangePitch("1");
        SetLoop(false);
    }

    public void PlayAudio()
    {
        soundMngr.Play(curAudio.name, curAudio.volume, curAudio.pitch, curAudio.loop);
    }

    public void SetSong(int songNum)
    {
        string songName = songPickDropdown.options[songNum].text;
        curAudio.name = songName.ToLower().Replace(" ", string.Empty);
        Debug.Log(curAudio.name);

    }

    public void ChangeVolume(string vol)
    {
        float volNum = System.Convert.ToSingle(vol);
        Debug.Log(volNum + " | " + vol);
        curAudio.volume = Mathf.Clamp(volNum, 0f, 1f);
    }

    public void ChangePitch(string pitch)
    {
        float pitchNum = System.Convert.ToSingle(pitch);
        Debug.Log(pitchNum + " | " + pitch);
        curAudio.pitch = Mathf.Clamp(pitchNum, 0.1f, 10f);
    }

    public void SetLoop(bool loop)
    {
        curAudio.loop = loop;
    }

    public void OpenCloseMusicMenu()
    {
        musicMenu.SetActive(!musicMenu.activeSelf);
    }
}
#pragma warning restore 0649