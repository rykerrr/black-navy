using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
#pragma warning disable 0649
public class AircraftCarrier : MonoBehaviour
{
    [Header("Set by script, shown for debug purposes")]
    [SerializeField] private Transform target;
    [SerializeField] private Rigidbody2D thisRb;
    [SerializeField] private Transform currentPlane;
    [Header("Properties")]
    [SerializeField] public UnitLayerMask whatUnitsToTarget;
    [SerializeField] public LayerMask whatAreOurProjectiles;
    [SerializeField] public LayerMask whatIsTarget;
    [SerializeField] private List<Transform> planePrefabs;
    [SerializeField] private Sprite[] spawningImages;
    [SerializeField] private Image spawningButton;
    [SerializeField] private GameObject carrierMenu;
    [SerializeField] private Transform aircraftSpawnPoint;
    [SerializeField] private Transform missileFirePoints;
    [SerializeField] private Transform cannonFirePoint;
    [SerializeField] private Transform rocketPrefab;
    [SerializeField] private Transform shellPrefab;
    [SerializeField] private Material targMat;
    [SerializeField] public float rocketFireAngle;
    [SerializeField] private float speed;
    [SerializeField] private float rocketDelay;
    [SerializeField] private float aircraftLaunchDelay;
    [SerializeField] private float cannonDelay;
    [SerializeField] private float rocketYOffset;
    [SerializeField] private float targetCheckRadius;
    [Header("Debug Properties")]
    [SerializeField] private bool isLaunchingAircraft = false;

    public GameObject menu => carrierMenu;
    private Vector3 veloc1;
    private Material normMat;
    private float rocketTimer;
    private float cannonTimer;
    private float aircraftLaunchTimer;

    private void Start()
    {
        normMat = transform.GetComponent<MeshRenderer>().material;
        thisRb = GetComponent<Rigidbody2D>();
        currentPlane = planePrefabs[0];
    }

    private void Update()
    {
        FindTarget();

        if (Time.time > rocketTimer)
        {
            FireRocket();
        }

        if (target)
        {
            if (Time.time > cannonTimer)
            {
                FireCannon();
            }
        }

        if (Time.time > aircraftLaunchTimer)
        {
            LaunchAircraft();
        }
    }

    private void FixedUpdate()
    {
        if (target && (target.GetComponent<ShipHumanoid>().whatAmI != UnitType.Aircraft || target.GetComponent<ShipHumanoid>().whatAmI != UnitType.Submarine))
        {
            thisRb.velocity = Vector3.SmoothDamp(thisRb.velocity, Vector3.zero, ref veloc1, 2f);
        }
        else
        {
            thisRb.velocity = transform.right * speed * Time.fixedDeltaTime;
        }
    }

    private void Test()
    {
        LogUtils.DebugLog(UnitLayerMask.CheckIfUnitIsInMask(UnitType.Ship, whatUnitsToTarget));
    }

    private void FireRocket()
    {
        int missileSpawnIndex = Random.Range(0, missileFirePoints.childCount - 1);
        Transform missileSpawnTransf = missileFirePoints.GetChild(missileSpawnIndex);
        Transform rocketClone = Instantiate(rocketPrefab, new Vector3(missileSpawnTransf.position.x, missileSpawnTransf.position.y + rocketYOffset, missileSpawnTransf.position.z), Quaternion.identity) as Transform;
        rocketClone.localEulerAngles = new Vector3(0f, 0f, rocketFireAngle);
        int layerValue = whatAreOurProjectiles.layermask_to_layer();
        rocketClone.gameObject.layer = layerValue;
        Rocket rocket = rocketClone.GetComponent<Rocket>();
        rocket.whatIsTarget = whatIsTarget;
        rocket.BoostStage();
        rocketTimer = rocketDelay + Time.time;
    }

    private void FireCannon()
    {
        Transform shellClone = Instantiate(shellPrefab, cannonFirePoint.position, Quaternion.identity) as Transform;
        shellClone.gameObject.layer = whatAreOurProjectiles.layermask_to_layer();
        shellClone.right = -(shellClone.position - target.position).normalized;
        Shell shell = shellClone.GetComponent<Shell>();
        cannonTimer = Time.time + cannonDelay;
    }

    private void LaunchAircraft()
    {
        if (isLaunchingAircraft)
        {
            Transform planeClone = Instantiate(currentPlane, aircraftSpawnPoint.position, currentPlane.rotation) as Transform;

            //if (planeClone.GetComponent<StrikeFighter>())
            //{
            //    StrikeFighter unit = planeClone.GetComponent<StrikeFighter>();
            //    unit.whatAreOurProjectiles = whatAreOurProjectiles;
            //    unit.whatIsTarget = whatIsTarget;
            //    unit.TakeOff();
            //    unit.gameObject.layer = gameObject.layer;
            //}
            //else if (planeClone.GetComponent<NormalBomber>())
            //{
            //    NormalBomber unit = planeClone.GetComponent<NormalBomber>();
            //    unit.whatAreOurProjectiles = whatAreOurProjectiles;
            //    unit.whatIsTarget = whatIsTarget;
            //    unit.TakeOff();
            //    unit.gameObject.layer = gameObject.layer;
            //}
            //else if (planeClone.GetComponent<AirSuperiorityFighter>())
            //{
            //    AirSuperiorityFighter unit = planeClone.GetComponent<AirSuperiorityFighter>();
            //    unit.whatAreOurProjectiles = whatAreOurProjectiles;
            //    unit.whatIsTarget = whatIsTarget;
            //    unit.TakeOff();
            //    unit.gameObject.layer = gameObject.layer;
            //}

            switch (planeClone.gameObject.layer) // temporary before i commit seppuku, retard
            {
                case 10: // player team
                    //newUnitClone.eulerAngles *= 1;
                    planeClone.eulerAngles = new Vector3(planeClone.eulerAngles.x, 0f, planeClone.eulerAngles.z);

                    if (planeClone.GetComponentInChildren<Image>())
                    {
                        //newUnitClone.GetComponentInChildren<SpriteRenderer>().color = Color.blue;
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
                        //newUnitClone.GetComponentInChildren<SpriteRenderer>().color = Color.red;
                        //newUnitClone.GetComponentInChildren<SpriteRenderer>().flipY = !newUnitClone.GetComponentInChildren<SpriteRenderer>().flipY;
                        //newUnitClone.GetComponentInChildren<SpriteRenderer>().flipX = !newUnitClone.GetComponentInChildren<SpriteRenderer>().flipX;
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

            aircraftLaunchTimer = Time.time + aircraftLaunchDelay;
        }
        else
        {
            return;
        }
    }

    private bool FindTarget()
    {
        List<Collider2D> hit = (Physics2D.OverlapCircleAll(transform.position, targetCheckRadius, whatIsTarget)).ToList();
        List<Collider2D> availableTargets = new List<Collider2D>();

        foreach (Collider2D en in hit)
        {
            if (UnitLayerMask.CheckIfUnitIsInMask(en.GetComponent<ShipHumanoid>().whatAmI, whatUnitsToTarget) == true)
            {
                availableTargets.Add(en);
            }
        }

        /*for (int i = 0; i < availableTargets.Count; i++)
        {
            LogUtils.DebugLog(" Index: " + i + " Name: " + hit[i].name + " Dist: " + (hit[i].transform.position - transform.position).magnitude);
        }*/

        availableTargets = availableTargets.OrderBy(en => Mathf.Abs((en.transform.position - transform.position).magnitude)).ToList();
        if (availableTargets.Count > 0)
        {
            target = availableTargets[0].transform;

            return true;
        }

        /*if (hit != null)
        {
            target = hit.transform;
            return true;
        }*/

        return false;
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

    private void OnMouseDown()
    {
        LogUtils.DebugLog("Clicked");
        carrierMenu.SetActive(!carrierMenu.activeInHierarchy);
    }

    public void StopOrSpawnPlanes()
    {
        isLaunchingAircraft = !isLaunchingAircraft;

        if(isLaunchingAircraft)
        {
            spawningButton.sprite = spawningImages[1];
        }
        else
        {
            spawningButton.sprite = spawningImages[0];
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, targetCheckRadius);
    }
}
#pragma warning restore 0649