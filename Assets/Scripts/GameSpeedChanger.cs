using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

#pragma warning disable 0649
public class GameSpeedChanger : MonoBehaviour
{
    [SerializeField] private Text speedText;

    private void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            float speed = Time.timeScale;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }

            if (Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Space))
            {
                speed = 0f;
                Time.timeScale = speed;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                speed = 1f;
                Time.timeScale = speed;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                speed = 2f;
                Time.timeScale = speed;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                speed = 4f;
                Time.timeScale = speed;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                speed = 6f;
                Time.timeScale = speed;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                speed = 8f;
                Time.timeScale = speed;
            }

            speedText.text = "Current Speed: " + System.Convert.ToInt32(speed) + "x";
        }
    }

    public void ChangeGameSpeed(float speed)
    {
        Time.timeScale = speed;
        //Time.fixedDeltaTime = 0.02f * Time.timeScale;
        speedText.text = "Current Speed: " + System.Convert.ToInt32(speed) + "x";
    }
}
#pragma warning restore 0649