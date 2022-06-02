using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 0649
public class CarrierModule : MonoBehaviour
{
    [Header("Aircraft properties")]
    [SerializeField] private GameObject carrierMenu;
    [SerializeField] private List<Transform> planePrefabs;
    [SerializeField] private Transform aircraftSpawnPoint;
    [SerializeField] private Transform owner;
    [SerializeField] private Image spawningButton;
    [SerializeField] private Sprite[] spawningImages;
    [SerializeField] private float aircraftLaunchDelay;
    [SerializeField] private bool isLaunchingAircraft;

    [HideInInspector] private LayerMask whatAreOurProjectiles;
    [HideInInspector] private LayerMask whatIsTarget;

    private Transform currentPlane;
    private float aircraftLaunchTimer;

    private void Start()
    {
        currentPlane = planePrefabs[0];
        aircraftLaunchTimer = Time.time + aircraftLaunchDelay;
        isLaunchingAircraft = true;
    }

    private void Update()
    {
        LaunchAircraft();
    }

    private void LaunchAircraft()
    {
        if (isLaunchingAircraft)
        {
            //LogUtils.DebugLog("Launch!");
            if (Time.time > aircraftLaunchTimer)
            {
                //LogUtils.DebugLog("launching plane");
                Transform planeClone = GetPlane();

                switch (planeClone.gameObject.layer) // temporary before i commit seppuku, retard
                {
                    case 10: // player team
                             //newUnitClone.eulerAngles *= 1;
                        //planeClone.eulerAngles = new Vector3(planeClone.eulerAngles.x, 0f, planeClone.eulerAngles.z);

                        if (planeClone.GetComponentInChildren<Image>())
                        {
                            //planeClone.GetComponentInChildren<SpriteRenderer>().color = Color.blue;
                            planeClone.GetComponentInChildren<Image>().color = new Color32(0x23, 0x36, 0x8C, 132);
                        }

                        whatAreOurProjectiles = 1 << 8;
                        whatIsTarget = 1 << 9;
                        planeClone.name = planeClone.name + "Player";
                        break;
                    case 9: // enemy team
                            //newUnitClone.eulerAngles *= 1;
                        planeClone.eulerAngles = new Vector3(0f, 180f, planeClone.eulerAngles.z);

                        if (planeClone.GetComponentInChildren<Image>())
                        {
                            //planeClone.GetComponentInChildren<SpriteRenderer>().color = Color.red;
                            planeClone.GetComponentInChildren<SpriteRenderer>().flipY = !planeClone.GetComponentInChildren<SpriteRenderer>().flipY;
                            planeClone.GetComponentInChildren<SpriteRenderer>().flipX = !planeClone.GetComponentInChildren<SpriteRenderer>().flipX;
                            planeClone.GetComponentInChildren<Image>().color = new Color32(0xE3, 0x00, 0x12, 132);
                        }

                        if (planeClone.GetComponentInChildren<SpriteRenderer>())
                        {
                            planeClone.GetComponentInChildren<SpriteRenderer>().flipY = !planeClone.GetComponentInChildren<SpriteRenderer>().flipY;
                        }

                        whatAreOurProjectiles = 1 << 11;
                        whatIsTarget = 1 << 10;
                        planeClone.name = planeClone.name + "Enemy";
                        break;
                    default:
                        LogUtils.DebugLog("more headaches here we go!!!!");
                        break;
                }

                AircraftBase aircraft = planeClone.GetComponent<AircraftBase>();
                aircraft.whatAreOurProjectiles = whatAreOurProjectiles;
                aircraft.whatIsTarget = whatIsTarget;
                aircraft.TakeOff();

                //LogUtils.DebugLog("takeoff");

                aircraftLaunchTimer = Time.time + aircraftLaunchDelay;
            }
            else
            {
                //LogUtils.DebugLog("on delay");
                return;
            }
        }
        else
        {
            //LogUtils.DebugLog("wut");
            return;
        }
    }

    private void OnMouseDown()
    {
        LogUtils.DebugLog("Clicked");
        carrierMenu.SetActive(!carrierMenu.activeSelf);
    }

    private Transform GetPlane()
    {
        Transform planeClone = null;

        if (currentPlane.GetComponent<AircraftThatWorksWithWeapon>())
        {
            //LogUtils.DebugLog("1");
            planeClone = Poolable.Get<AircraftThatWorksWithWeapon>(() => Poolable.CreateObj<AircraftThatWorksWithWeapon>(currentPlane.gameObject), aircraftSpawnPoint.position, transform.rotation, null).transform;
        }
        else if (currentPlane.GetComponent<StrikeFighterThatWorksWithWeapon>())
        {
            //LogUtils.DebugLog("2");
            planeClone = Poolable.Get<StrikeFighterThatWorksWithWeapon>(() => Poolable.CreateObj<StrikeFighterThatWorksWithWeapon>(currentPlane.gameObject), aircraftSpawnPoint.position, transform.rotation, null).transform;
        }
        else if (currentPlane.GetComponent<StrategicBomberThatWorksWithWeapon>())
        {
            //LogUtils.DebugLog("3");
            planeClone = Poolable.Get<StrategicBomberThatWorksWithWeapon>(() => Poolable.CreateObj<StrategicBomberThatWorksWithWeapon>(currentPlane.gameObject), aircraftSpawnPoint.position, transform.rotation, null).transform;
        }

        return planeClone;
    }

    public void ChangePlanePrefab(int pref)
    {
        if (pref < 0 || pref >= planePrefabs.Count)
        {
            LogUtils.DebugLog("Yeet yeet motherfucker");
            LogUtils.DebugLog(planePrefabs[pref] + " | " + pref);
            Debug.Break();
        }

        currentPlane = planePrefabs[pref];
    }

    public void StopOrSpawnPlanes()
    {
        isLaunchingAircraft = !isLaunchingAircraft;

        if (isLaunchingAircraft)
        {
            spawningButton.sprite = spawningImages[1];
        }
        else
        {
            spawningButton.sprite = spawningImages[0];
        }
    }

    private void OnEnable()
    {
        isLaunchingAircraft = false;
        if (currentPlane == null)
        {
            currentPlane = planePrefabs[0]; // might need this, might not
        }
    }
}
#pragma warning restore 0649