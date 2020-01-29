using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 0649
public class TestUIBuddy : MonoBehaviour
{
    [System.Serializable]
    public struct UnitSettings
    {
        public Transform unitPrefab;
        public UnitType typeOfUnit;

        public int teamLayer;
        public int amountOfUnit;
        public float delayBetweenEachSpawn;

        //public void ClearSettings()
        //{
        //    typeOfUnit = UnitType.Aircraft;
        //    teamLayer = 10;
        //    amountOfUnit = 1;
        //    delayBetweenEachSpawn = 1;
        //}
    }

    [Header("Properties")]
    [SerializeField] private Transform playerBaseSpawn;
    [SerializeField] private Transform enemyBaseSpawn;
    [SerializeField] private Transform menu;
    [SerializeField] private Transform loadouts;
    [SerializeField] private List<Transform> unitPrefabs;
    [SerializeField] private Material enemyMat;
    [SerializeField] private Material plrMat;

    [Header("Shown for debug purposes")]
    [SerializeField] private UnitSettings currentUnit;

    private void Start()
    {
        currentUnit.unitPrefab = unitPrefabs[0];
        currentUnit.typeOfUnit = UnitType.Aircraft;
        currentUnit.teamLayer = 10;
        currentUnit.amountOfUnit = 1;
        currentUnit.delayBetweenEachSpawn = 1f;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ClearBattlefield();
        }
    }

    public void ChangeUnitTeamLayer(int team)
    {
        currentUnit.teamLayer = new int();

        switch (team)
        {
            case 0:
                currentUnit.teamLayer = 10; // player team
                break;
            case 1:
                currentUnit.teamLayer = 9; // enemy team
                break;
            default:
                return;
        }
    }

    public void ChangeUnitType(int type)
    {
        currentUnit.unitPrefab = unitPrefabs[type];
        currentUnit.typeOfUnit = new UnitType();

        //if (currentUnit.unitPrefab.GetComponent<AirSuperiorityFighter>() || currentUnit.unitPrefab.GetComponent<StrikeFighter>() || currentUnit.unitPrefab.GetComponent<NormalBomber>()
        //    || currentUnit.unitPrefab.GetComponent<ParasiteFighter>())
        //{
        //    currentUnit.typeOfUnit = UnitType.Aircraft;
        //}
        //else if (currentUnit.unitPrefab.GetComponent<Submarine>() || currentUnit.unitPrefab.GetComponent<BallisticSubmarine>())
        //{
        //    currentUnit.typeOfUnit = UnitType.Submarine;
        //}
        //else if (currentUnit.unitPrefab.GetComponent<NormalShipWeapons>() || currentUnit.unitPrefab.GetComponent<AircraftCarrier>())
        //{
        //    currentUnit.typeOfUnit = UnitType.Ship;
        //}
    }

    public void ChangeUnitAmount(string amnStr)
    {
        if (amnStr.Length < 1)
        {
            return;
        }

        currentUnit.amountOfUnit = new int();

        int amn = System.Convert.ToInt32(amnStr);
        currentUnit.amountOfUnit = amn;
    }

    public void ChangeUnitSpawnDelay(string delayStr)
    {
        if (delayStr.Length < 1)
        {
            return;
        }

        currentUnit.delayBetweenEachSpawn = new float();

        float delay = System.Convert.ToSingle(delayStr);
        currentUnit.delayBetweenEachSpawn = delay;
    }

    public void OpenCloseMenu()
    {
        //currentUnit.ClearSettings();
        //currentUnit.unitPrefab = unitPrefabs[0];
        menu.gameObject.SetActive(!menu.gameObject.activeSelf);
    }

    public void OpenCloseLoadouts()
    {
        loadouts.gameObject.SetActive(!loadouts.gameObject.activeSelf);
    }

    public void SpawnUnit()
    {
        UnitSettings newUnitCache = currentUnit;

        StartCoroutine(SpawnUnitLogic(newUnitCache));
        // unit spawn logic
    }

    public void ClearBattlefield()
    {
        List<Poolable> listOfEverything = FindObjectsOfType<Poolable>().ToList<Poolable>();

        foreach (Poolable obj in listOfEverything)
        {
            obj.ReturnToPool();
        }

    }

    private void LoadUnitWeapons(Transform unitClone, string unitName) // CALLED WHEN SPAWNING UNITS
    {
        UnitWeaponLoadout[] weaponLoadouts = null;
        UnitWeaponLoadout? curLoadout = null;
        int curUnit;

        if (currentUnit.teamLayer == 9)
        {
            weaponLoadouts = LoadoutSwitcharoo.Instance.GetTeam2UnitLoadouts;
        }
        else if (currentUnit.teamLayer == 10)
        {
            weaponLoadouts = LoadoutSwitcharoo.Instance.GetTeam1UnitLoadouts;
        }

        foreach (UnitWeaponLoadout loadout in weaponLoadouts)
        {
            if (loadout.name == unitName)
            {
                curLoadout = loadout;
                Debug.Log("Loadout exists");
                break;
            }
        }

        UnitBase unit = unitClone.GetComponent<UnitBase>();
        curUnit = LoadoutSwitcharoo.Instance.GetUnit;
        for (int i = 0; i < unit.weapons.Length; i++)
        {
            if (curLoadout.HasValue)
            {
                WeaponBase weaponClone = Instantiate(curLoadout.Value.weapons[i]); // here
                weaponClone.transform.parent = unitClone;
                weaponClone.transform.localPosition = Vector3.zero;

                unit.weapons[i] = weaponClone;
                unit.weapons[i].owner = unitClone;
            }
            else
            {
                Debug.Log(curLoadout);
                Debug.Log("Error..");
                Debug.Break();

                return;
            }
        }

        unit.InitializeWeapons();
    }

    private IEnumerator SpawnUnitLogic(UnitSettings newUnit)
    {
        for (int i = 0; i < currentUnit.amountOfUnit; i++)
        {
            Transform newUnitClone = GetUnit(newUnit);
            newUnitClone.gameObject.layer = (int)newUnit.teamLayer;

            //Debug.Log(newUnitClone.gameObject.layer + " | " + (int)newUnit.teamLayer);

            LayerMask whatAreOurProjectiles;
            LayerMask whatIsTarget;

            switch (newUnit.teamLayer)
            {
                case 10: // player team
                    newUnitClone.position = playerBaseSpawn.position;
                    //newUnitClone.eulerAngles *= 1;
                    newUnitClone.eulerAngles = new Vector3(newUnitClone.eulerAngles.x, 0f, newUnitClone.eulerAngles.z);

                    if (newUnitClone.GetComponent<MeshRenderer>())
                    {
                        newUnitClone.GetComponent<MeshRenderer>().material = plrMat;
                    }

                    if (newUnitClone.GetComponentInChildren<Image>())
                    {
                        //newUnitClone.GetComponentInChildren<SpriteRenderer>().color = Color.blue;
                        newUnitClone.GetComponentInChildren<Image>().color = new Color32(0x23, 0x36, 0x8C, 132);
                    }

                    whatAreOurProjectiles = 1 << 8;
                    whatIsTarget = 1 << 9;
                    newUnitClone.name = newUnitClone.name + "Player";
                    break;
                case 9: // enemy team
                    newUnitClone.position = enemyBaseSpawn.position;
                    //newUnitClone.eulerAngles *= 1;
                    newUnitClone.eulerAngles = new Vector3(0f, 180f, newUnitClone.eulerAngles.z);

                    if (newUnitClone.GetComponent<MeshRenderer>())
                    {
                        newUnitClone.GetComponent<MeshRenderer>().material = enemyMat;
                    }

                    if (newUnitClone.GetComponentInChildren<Image>())
                    {
                        //newUnitClone.GetComponentInChildren<SpriteRenderer>().color = Color.red;
                        //newUnitClone.GetComponentInChildren<SpriteRenderer>().flipY = !newUnitClone.GetComponentInChildren<SpriteRenderer>().flipY;
                        //newUnitClone.GetComponentInChildren<SpriteRenderer>().flipX = !newUnitClone.GetComponentInChildren<SpriteRenderer>().flipX;
                        newUnitClone.GetComponentInChildren<Image>().color = new Color32(0xE3, 0x00, 0x12, 132);
                    }

                    if (newUnitClone.GetComponentInChildren<SpriteRenderer>())
                    {
                        //newUnitClone.GetComponentInChildren<SpriteRenderer>().flipY = !newUnitClone.GetComponentInChildren<SpriteRenderer>().flipY;
                    }

                    whatAreOurProjectiles = 1 << 11;
                    whatIsTarget = 1 << 10;
                    newUnitClone.name = newUnitClone.name + "Enemy";
                    break;
                default:
                    Debug.Log("more headaches here we go!!!!");
                    yield break;
            }

            #region RETARD = OVERDRIVE LITERALLY
            UnitBase cloneUnit = newUnitClone.GetComponent<UnitBase>();
            cloneUnit.whatAreOurProjectiles = whatAreOurProjectiles;
            cloneUnit.whatIsTarget = whatIsTarget;
            cloneUnit.transform.position = new Vector2(newUnitClone.position.x, cloneUnit.BaseAltitude);
            AircraftBase aircraft = cloneUnit as AircraftBase;

            if (aircraft != null)
            {
                aircraft.LoadInAir();
            }
            #endregion

            LoadUnitWeapons(newUnitClone, newUnit.unitPrefab.name);
            yield return new WaitForSecondsRealtime((float)newUnit.delayBetweenEachSpawn);
        }
    }

    private Transform GetUnit(UnitSettings newUnit)
    {
        Transform retunit = null;

        if (newUnit.unitPrefab.GetComponent<AircraftThatWorksWithWeapon>())
        {
            retunit = Poolable.Get<AircraftThatWorksWithWeapon>(() => Poolable.CreateObj<AircraftThatWorksWithWeapon>(newUnit.unitPrefab.gameObject), playerBaseSpawn.position, newUnit.unitPrefab.rotation).transform;
        }
        else if (newUnit.unitPrefab.GetComponent<StrikeFighterThatWorksWithWeapon>())
        {
            retunit = Poolable.Get<StrikeFighterThatWorksWithWeapon>(() => Poolable.CreateObj<StrikeFighterThatWorksWithWeapon>(newUnit.unitPrefab.gameObject), playerBaseSpawn.position, newUnit.unitPrefab.rotation).transform;
        }
        else if (newUnit.unitPrefab.GetComponent<StrategicBomberThatWorksWithWeapon>())
        {
            retunit = Poolable.Get<StrategicBomberThatWorksWithWeapon>(() => Poolable.CreateObj<StrategicBomberThatWorksWithWeapon>(newUnit.unitPrefab.gameObject), playerBaseSpawn.position, newUnit.unitPrefab.rotation).transform;
        }
        else if (newUnit.unitPrefab.GetComponent<CarrierModule>())
        {
            retunit = Poolable.Get<FrigateThatWorksWithWeapon>(() => Poolable.CreateObj<FrigateThatWorksWithWeapon>(newUnit.unitPrefab.gameObject), playerBaseSpawn.position, newUnit.unitPrefab.rotation).transform;
        }
        else if (newUnit.unitPrefab.GetComponent<FrigateThatWorksWithWeapon>())
        {
            retunit = Poolable.Get<FrigateThatWorksWithWeapon>(() => Poolable.CreateObj<FrigateThatWorksWithWeapon>(newUnit.unitPrefab.gameObject), playerBaseSpawn.position, newUnit.unitPrefab.rotation).transform;
        }
        else if (newUnit.unitPrefab.GetComponent<SubmarineThatWorksWithWeapon>())
        {
            retunit = Poolable.Get<SubmarineThatWorksWithWeapon>(() => Poolable.CreateObj<SubmarineThatWorksWithWeapon>(newUnit.unitPrefab.gameObject), playerBaseSpawn.position, newUnit.unitPrefab.rotation).transform;
        }
        else if (newUnit.unitPrefab.GetComponent<BallisticSubmarineThatWorksWithWeapon>())
        {
            retunit = Poolable.Get<BallisticSubmarineThatWorksWithWeapon>(() => Poolable.CreateObj<BallisticSubmarineThatWorksWithWeapon>(newUnit.unitPrefab.gameObject), playerBaseSpawn.position, newUnit.unitPrefab.rotation).transform;
        }
        else if (newUnit.unitPrefab.GetComponent<DestroyerThatWorksWithWeapon>())
        {
            retunit = Poolable.Get<DestroyerThatWorksWithWeapon>(() => Poolable.CreateObj<DestroyerThatWorksWithWeapon>(newUnit.unitPrefab.gameObject), playerBaseSpawn.position, newUnit.unitPrefab.rotation).transform;
        }
        else if (newUnit.unitPrefab.GetComponent<PatrolBoatThatWorksWithWeapon>())
        {
            retunit = Poolable.Get<PatrolBoatThatWorksWithWeapon>(() => Poolable.CreateObj<PatrolBoatThatWorksWithWeapon>(newUnit.unitPrefab.gameObject), playerBaseSpawn.position, newUnit.unitPrefab.rotation).transform;
        }

        return retunit;
    }
}
#pragma warning restore 0649