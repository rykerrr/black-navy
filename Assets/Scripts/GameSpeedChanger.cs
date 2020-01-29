using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

#pragma warning disable 0649
public class GameSpeedChanger : MonoBehaviour
{
    [SerializeField] private Text speedText;
    float lastGameSpeed = 1f;

    private void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }

            if (Input.GetKeyDown(KeyCode.Alpha0) || (Input.GetKeyDown(KeyCode.Space) && Time.timeScale >= 0.0001f))
            {
                ChangeGameSpeed(0f);
            }
            else if((Input.GetKeyDown(KeyCode.Space) && Time.timeScale <= 0.0001f))
            {
                ChangeGameSpeed(lastGameSpeed);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                ChangeGameSpeed(1f);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                ChangeGameSpeed(2f);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                ChangeGameSpeed(4f);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                ChangeGameSpeed(6f);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                ChangeGameSpeed(8f);
            }
        }
    }

    public void ChangeGameSpeed(float speed)
    {
        float lastGameSpeed = Time.timeScale;
        Time.timeScale = speed;
        //Time.fixedDeltaTime = 0.02f * Time.timeScale;
        speedText.text = "Current Speed: " + System.Convert.ToInt32(speed) + "x";
    }
}
#pragma warning restore 0649