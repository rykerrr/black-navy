using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class Stationary : MonoBehaviour
{
    [SerializeField] private float angle = -90f;
    [SerializeField] private float offset;

    private void OnValidate()
    {
        transform.position = new Vector2(transform.parent.position.x, transform.parent.position.y + offset);
        transform.eulerAngles = new Vector3(0f, 0f, angle);
    }

    private void LateUpdate()
    {
        transform.position = new Vector2(transform.parent.position.x, transform.parent.position.y + 10f);
        transform.eulerAngles = new Vector3(0f, 0f, angle);
    }
}
#pragma warning restore 0649