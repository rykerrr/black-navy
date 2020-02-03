using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class Stationary : MonoBehaviour
{
    private void LateUpdate()
    {
        transform.eulerAngles = new Vector3(0f, 0f, -90f);
    }
}
#pragma warning restore 0649