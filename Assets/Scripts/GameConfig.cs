using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#pragma warning disable 0649
public class GameConfig : Singleton<GameConfig>
{
    [Header("Game properties")]
    [SerializeField] private GameObject showBuf;
    [SerializeField] private float waterLevel;
    [SerializeField] private float projectileLifetime;
    [SerializeField] private bool bufferSound = true;
    [SerializeField] private bool isSandbox = false;

    public float WaterLevel => waterLevel;
    public float ProjectileLifeTime => projectileLifetime;
    public bool BufferSound => bufferSound;
    public bool IsSandbox => isSandbox;

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "Sandbox")
        {
            isSandbox = true;
            Debug.Log("sandbox");
        }
        else
        {
            isSandbox = false;
            Debug.Log("no sandbox");
        }
    }

    private void Update()
    {
        if ((Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.F4)) || Input.GetKeyDown(KeyCode.B))
        {
            bufferSound = !bufferSound;
        }

        if (Input.GetKeyDown(KeyCode.Home))
        {
            SceneManager.LoadScene("Level1");
        }
        else if (Input.GetKeyDown(KeyCode.Insert))
        {
            SceneManager.LoadScene("Sandbox");
        }

        if (showBuf)
        {
            if (bufferSound)
            {
                if (!showBuf.activeSelf)
                {
                    showBuf.SetActive(true);
                }
            }
            else
            {
                if (showBuf.activeSelf)
                {
                    showBuf.SetActive(false);
                }
            }
        }
    }
}
#pragma warning restore 0649
