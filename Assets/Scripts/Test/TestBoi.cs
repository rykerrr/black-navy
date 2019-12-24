using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class TestBoi : MonoBehaviour
{
    //[SerializeField] private Transform target;
    //[SerializeField] private float angle;
    //[SerializeField] private float eulerAngle;
    //[SerializeField] private float testAngle;
    //[SerializeField] private float yeet;

    //private void Update()
    //{
    //    Vector2 dir = target.position - transform.position;
    //    Debug.DrawRay(transform.position, dir, Color.magenta);
    //    angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    //    Debug.Log(transform.rotation.eulerAngles.z + " | " + transform.rotation.z * Mathf.Rad2Deg);
    //    testAngle = Mathf.Abs(transform.rotation.z * Mathf.Rad2Deg) - Mathf.Abs(angle);
    //    Quaternion angleAxis = Quaternion.AngleAxis(angle, Vector3.forward);
    //    eulerAngle = angleAxis.eulerAngles.z;
    //    transform.rotation = Quaternion.Slerp(transform.rotation, angleAxis, 0.05f);
    //}

    public void Test(string yeet)
    {
        Debug.Log(yeet);
    }
}
#pragma warning restore 0649