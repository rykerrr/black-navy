using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class Follow : MonoBehaviour
{
    [SerializeField] private Transform objToFollow;
    [SerializeField] private Vector3 offset;

    private void LateUpdate()
    {
        transform.position = objToFollow.position + offset;
    }
}
#pragma warning disable 0649