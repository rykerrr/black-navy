using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class PartSysTest : MonoBehaviour
{
    [SerializeField] private ParticleSystem partSys;
    [SerializeField] private int count;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.G))
        {
            partSys.Emit(count);
        }
    }
}
#pragma warning restore 0649