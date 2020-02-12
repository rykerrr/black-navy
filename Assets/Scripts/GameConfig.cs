using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class GameConfig : Singleton<GameConfig>
{
    [Header("Game properties")]
    [SerializeField] private GameObject showBuf;
    [SerializeField] private float waterLevel;
    [SerializeField] private float projectileLifetime;
    [SerializeField] private bool bufferSound = true;

    private void Update()
    {
        if((Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.F4)) || Input.GetKeyDown(KeyCode.B))
        {
            bufferSound = !bufferSound;
        }

        if(bufferSound)
        {
            if(!showBuf.activeSelf)
            {
                showBuf.SetActive(true);
            }
        }
        else
        {
            if(showBuf.activeSelf)
            {
                showBuf.SetActive(false);
            }
        }
    }

    public float WaterLevel => waterLevel;
    public float ProjectileLifeTime => projectileLifetime;
    public bool BufferSound => bufferSound;
}
#pragma warning restore 0649
