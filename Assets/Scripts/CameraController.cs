using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

#pragma warning disable 0649
public class CameraController : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] private Transform target;
    [SerializeField] private Transform selectedUnit;
    [SerializeField] private Text zoomLevelText;
    [SerializeField] private float movSensDivisor = 1f;
    [SerializeField] private float mwheelSensDivisor = 1f;

    [Header("Shown for debug purposes")]
    private Camera targCam;
    [SerializeField] private float xMovement;
    [SerializeField] private float yMovement;
    [SerializeField] private float movMult = 1;
    [SerializeField] private float mWheelMovement;

    private float camZoomLevel = 175f;
    
    private Image zoomLevelImg;
    private bool appearing = false;
    private float veloc1;
    private float veloc2;
    private float unitZoomTimer;
    private float zoomTimer;

    bool isMobile;
    
    public Transform SelectedUnit => selectedUnit;

    private void Start()
    {
        isMobile = SystemInfo.deviceType == DeviceType.Handheld;
        
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

    private void LateUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Select();
        }

        if (isMobile)
        {
            if (Input.touchCount <= 0) return;
            
            var mv = Input.GetTouch(0).deltaPosition;
            xMovement = mv.x / 3f; 
            yMovement = mv.y / 3f;
        }
        else
        {
            xMovement = Input.GetAxis("Horizontal");
            yMovement = Input.GetAxis("Vertical");
        }

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
            if (appearing)
            {
                zoomLevelText.color = new Color(zoomLevelText.color.r, zoomLevelText.color.g, zoomLevelText.color.b, Mathf.SmoothDamp(zoomLevelText.color.a, 1f, ref veloc1, 0.6f));
                zoomLevelImg.color = new Color(zoomLevelImg.color.r, zoomLevelImg.color.g, zoomLevelImg.color.b, Mathf.SmoothDamp(zoomLevelImg.color.a, 1f, ref veloc2, 0.6f));
            }

            if (Time.time > zoomTimer)
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

        if (selectedUnit)
        {
            if (xMovement != 0 || yMovement != 0)
            {
                selectedUnit = null;
            }
            else
            {
                target.position = new Vector3(selectedUnit.position.x, selectedUnit.position.y, -10f);

                if (mWheelMovement != 0)
                {
                    unitZoomTimer = Time.time + 2f;
                }
                else
                {
                    if (Time.time > unitZoomTimer)
                    {
                        targCam.orthographicSize = Mathf.MoveTowards(targCam.orthographicSize, 60f, 0.3f);
                    }
                }
            }
        }

        targCam.orthographicSize = Mathf.Clamp(isMobile ? camZoomLevel : targCam.orthographicSize - (mWheelMovement / mwheelSensDivisor) * movMult, 5, 175);
        target.position = new Vector3(target.position.x + (xMovement / movSensDivisor) * movMult, target.position.y + (yMovement / movSensDivisor) * movMult, target.position.z);
        zoomLevelText.text = "Zoom level: " + targCam.orthographicSize;
    }

    private void Select()
    {
        Vector2 selectPos = targCam.ScreenToWorldPoint(Input.mousePosition);
        LayerMask units = new LayerMask();
        units = (1 << 10) + (1 << 9);

        RaycastHit2D hit = Physics2D.Raycast(selectPos, Vector2.zero, 3f, units);

        if (hit)
        {
            UnitHumanoid tempHum;

            if (hit.collider && (tempHum = hit.collider.GetComponent<UnitHumanoid>()) != null)
            {
                if(tempHum.type == UnitType.Base)
                {
                    return;
                }

                selectedUnit = hit.collider.transform;
            }
        }
    }

    public void ClearUnitSelection()
    {
        selectedUnit = null;
    }

    public void Slider_ChangeZoomLevel(float zoomLevel)
    {
        camZoomLevel = zoomLevel;
    }
}
#pragma warning disable 0649