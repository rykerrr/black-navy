using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 0649
public class GameSpeedChanger : MonoBehaviour
{
    [SerializeField] private Text speedText;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Yeet");
            Application.Quit();
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