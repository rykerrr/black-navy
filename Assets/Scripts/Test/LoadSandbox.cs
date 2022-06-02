using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSandbox : MonoBehaviour
{
    public void OnClick_LoadSandbox()
    {
        SceneManager.LoadScene(1);
    }
}
