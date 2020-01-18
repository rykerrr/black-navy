﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

#pragma warning disable 0649
public class CameraController : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] private Transform target;
    [SerializeField] private Text zoomLevelText;
    [SerializeField] private float movSensDivisor = 1f;
    [SerializeField] private float mwheelSensDivisor = 1f;

    [Header("Shown for debug purposes")]
    private Camera targCam;
    [SerializeField] private float xMovement;
    [SerializeField] private float yMovement;
    [SerializeField] private float movMult = 1;
    [SerializeField] private float mWheelMovement;

    private Image zoomLevelImg;
    private bool appearing = false;
    private float veloc1;
    private float veloc2;
    private float zoomTimer;

    private void Start()
    {
        if (target.GetComponent<Camera>() == null)
        {
            targCam = Camera.main;
            throw new System.Exception("No camera dip, assigning main camera to targCam...");
        }
        else
        {
            targCam = target.GetComponent<Camera>();
        }

        zoomLevelImg = zoomLevelText.GetComponentInParent<Image>();
        zoomLevelImg.color = new Color(zoomLevelImg.color.r, zoomLevelImg.color.g, zoomLevelImg.color.b, 0);
        zoomLevelText.color = new Color(zoomLevelText.color.r, zoomLevelText.color.g, zoomLevelText.color.b, 0);
    }

    private void Update()
    {
        xMovement = Input.GetAxis("Horizontal");
        yMovement = Input.GetAxis("Vertical");

        if (!EventSystem.current.IsPointerOverGameObject())
        {
            mWheelMovement = Input.mouseScrollDelta.y;
        }
        else
        {
            mWheelMovement = 0;
        }

        if (mWheelMovement != 0)
        {
            appearing = true;
            zoomTimer = Time.time + 7f;
        }
        else
        {
            if(appearing)
            {
                zoomLevelText.color = new Color(zoomLevelText.color.r, zoomLevelText.color.g, zoomLevelText.color.b, Mathf.SmoothDamp(zoomLevelText.color.a, 1f, ref veloc1, 0.6f));
                zoomLevelImg.color = new Color(zoomLevelImg.color.r, zoomLevelImg.color.g, zoomLevelImg.color.b, Mathf.SmoothDamp(zoomLevelImg.color.a, 1f, ref veloc2, 0.6f));
            }

            if(Time.time > zoomTimer)
            {
                appearing = false;

                zoomLevelText.color = new Color(zoomLevelText.color.r, zoomLevelText.color.g, zoomLevelText.color.b, Mathf.SmoothDamp(zoomLevelText.color.a, 0f, ref veloc1, 0.6f));
                zoomLevelImg.color = new Color(zoomLevelImg.color.r, zoomLevelImg.color.g, zoomLevelImg.color.b, Mathf.SmoothDamp(zoomLevelImg.color.a, 0f, ref veloc2, 0.6f));
            }
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            movMult = 2f;
        }
        else
        {
            movMult = 1;
        }

        targCam.orthographicSize = Mathf.Clamp(targCam.orthographicSize - (mWheelMovement / mwheelSensDivisor) * movMult, 5, 100);
        target.position = new Vector3(target.position.x + (xMovement / movSensDivisor) * movMult, target.position.y + (yMovement / movSensDivisor) * movMult, target.position.z);
        zoomLevelText.text = "Zoom level: " + targCam.orthographicSize;
    }
}
#pragma warning disable 0649