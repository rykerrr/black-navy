using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class Stationary : MonoBehaviour
{
    [SerializeField] private float angle = -90f;

    private void LateUpdate()
    {
        transform.eulerAngles = new Vector3(0f, 0f, angle);
    }
}
#pragma warning restore 0649