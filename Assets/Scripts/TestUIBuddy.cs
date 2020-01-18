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
        //List<GameObject> listOfEverything = new List<GameObject>();

        //foreach(AirSuperiorityFighter obj in FindObjectsOfType<AirSuperiorityFighter>())
        //{
        //    listOfEverything.Add(obj.gameObject);
        //}
        //foreach (NormalBomber obj in FindObjectsOfType<NormalBomber>())
        //{
        //    listOfEverything.Add(obj.gameObject);
        //}
        //foreach (StrikeFighter obj in FindObjectsOfType<StrikeFighter>())
        //{
        //    listOfEverything.Add(obj.gameObject);
        //}
        //foreach (AircraftCarrier obj in FindObjectsOfType<AircraftCarrier>())
        //{
        //    listOfEverything.Add(obj.gameObject);
        //}
        //foreach (NormalShipWeapons obj in FindObjectsOfType<NormalShipWeapons>())
        //{
        //    listOfEverything.Add(obj.gameObject);
        //}
        //foreach (Submarine obj in FindObjectsOfType<Submarine>())
        //{
        //    listOfEverything.Add(obj.gameObject);
        //}
        //foreach (BallisticSubmarine obj in FindObjectsOfType<BallisticSubmarine>())
        //{
        //    listOfEverything.Add(obj.gameObject);
        //}
        //foreach (ParasiteFighter obj in FindObjectsOfType<ParasiteFighter>())
        //{
        //    listOfEverything.Add(obj.gameObject);
        //}
        //foreach (ParasiteCarrier obj in FindObjectsOfType<ParasiteCarrier>())
        //{
        //    listOfEverything.Add(obj.gameObject);
        //}

        //foreach (Missile obj in FindObjectsOfType<Missile>())
        //{
        //    listOfEverything.Add(obj.gameObject);
        //}
        //foreach (UnguidedRocket obj in FindObjectsOfType<UnguidedRocket>())
        //{
        //    listOfEverything.Add(obj.gameObject);
        //}
        //foreach (Shell obj in FindObjectsOfType<Shell>())
        //{
        //    listOfEverything.Add(obj.gameObject);
        //}
        //foreach (Torpedo obj in FindObjectsOfType<Torpedo>())
        //{
        //    listOfEverything.Add(obj.gameObject);
        //}
        //foreach (Rocket obj in FindObjectsOfType<Rocket>())
        //{
        //    listOfEverything.Add(obj.gameObject);
        //}

        //foreach(GameObject obj in listOfEverything)
        //{
        //    Destroy(obj);
        //}
    }

    private void LoadUnitWeapons(Transform unitClone) // CALLED WHEN SPAWNING UNITS
    {
        if (unitClone.GetComponent<AircraftThatWorksWithWeapon>())
        {
            UnitWeaponLoadout[] weaponLoadouts = null;

            if (currentUnit.teamLayer == 9)
            {
                weaponLoadouts = LoadoutSwitcharoo.Instance.GetTeam2UnitLoadouts;
            }
            else if (currentUnit.teamLayer == 10)
            {
                weaponLoadouts = LoadoutSwitcharoo.Instance.GetTeam1UnitLoadouts;
            }

            AircraftThatWorksWithWeapon aircr = unitClone.GetComponent<AircraftThatWorksWithWeapon>();

            for (int i = 0; i < aircr.weapons.Length; i++)
            {
                WeaponBase weaponClone = Instantiate(weaponLoadouts[0].weapons[i]);
                weaponClone.transform.parent = unitClone;
                weaponClone.transform.localPosition = Vector3.zero;

                aircr.weapons[i] = weaponClone;
                aircr.weapons[i].owner = unitClone;
            }
        }
        else if (unitClone.GetComponent<StrikeFighterThatWorksWithWeapon>())
        {
            UnitWeaponLoadout[] weaponLoadouts = null;

            if (currentUnit.teamLayer == 9)
            {
                weaponLoadouts = LoadoutSwitcharoo.Instance.GetTeam2UnitLoadouts;
            }
            else if (currentUnit.teamLayer == 10)
            {
                weaponLoadouts = LoadoutSwitcharoo.Instance.GetTeam1UnitLoadouts;
            }

            StrikeFighterThatWorksWithWeapon aircr = unitClone.GetComponent<StrikeFighterThatWorksWithWeapon>();
            Debug.Log("Yeet");

            for (int i = 0; i < aircr.weapons.Length; i++)
            {
                WeaponBase weaponClone = Instantiate(weaponLoadouts[1].weapons[i]);
                weaponClone.transform.parent = unitClone;
                weaponClone.transform.localPosition = Vector3.zero;

                aircr.weapons[i] = weaponClone;
                aircr.weapons[i].owner = unitClone;
            }
        }
        else
        {
            return;
        }
    }

    private IEnumerator SpawnUnitLogic(UnitSettings newUnit)
    {
        for (int i = 0; i < currentUnit.amountOfUnit; i++)
        {
            Transform newUnitClone = Instantiate(newUnit.unitPrefab, playerBaseSpawn.position, newUnit.unitPrefab.rotation) as Transform; // player team
            newUnitClone.gameObject.layer = (int)newUnit.teamLayer;
            LoadUnitWeapons(newUnitClone);

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
            ;
            AircraftBase aircraft = cloneUnit as AircraftBase;

            if (aircraft != null)
            {
                aircraft.LoadInAir();
            }


            //if (newUnitClone.GetComponent<AirSuperiorityFighter>())
            //{
            //    newUnitClone.position = new Vector2(newUnitClone.position.x, newUnitClone.position.y + Random.Range(45f, 55f));
            //    AirSuperiorityFighter unit = newUnitClone.GetComponent<AirSuperiorityFighter>();
            //    unit.whatAreOurProjectiles = whatAreOurProjectiles;
            //    unit.whatIsTarget = whatIsTarget;
            //    unit.LoadInAir();
            //    //Debug.Log((int)whatAreOurProjectiles + " | " + (int)unit.whatAreOurProjectiles);
            //    //Debug.Log((int)whatIsTarget + " | " + (int)unit.whatIsTarget);
            //}
            //else if (newUnitClone.GetComponent<NormalBomber>())
            //{
            //    newUnitClone.position = new Vector2(newUnitClone.position.x, newUnitClone.position.y + Random.Range(40f, 60f));
            //    NormalBomber unit = newUnitClone.GetComponent<NormalBomber>();
            //    unit.whatAreOurProjectiles = whatAreOurProjectiles;
            //    unit.whatIsTarget = whatIsTarget;
            //    unit.LoadInAir();
            //    //Debug.Log((int)whatAreOurProjectiles + " | " + (int)unit.whatAreOurProjectiles);
            //    //Debug.Log((int)whatIsTarget + " | " + (int)unit.whatIsTarget);
            //}
            //else if (newUnitClone.GetComponent<StrikeFighter>())
            //{
            //    newUnitClone.position = new Vector2(newUnitClone.position.x, newUnitClone.position.y + Random.Range(27f, 42f));
            //    StrikeFighter unit = newUnitClone.GetComponent<StrikeFighter>();
            //    unit.whatAreOurProjectiles = whatAreOurProjectiles;
            //    unit.whatIsTarget = whatIsTarget;
            //    unit.LoadInAir();
            //    //Debug.Log((int)whatAreOurProjectiles + " | " + (int)unit.whatAreOurProjectiles);
            //    //Debug.Log((int)whatIsTarget + " | " + (int)unit.whatIsTarget);
            //}
            //else if (newUnitClone.GetComponent<ParasiteFighter>())
            //{
            //    newUnitClone.position = new Vector2(newUnitClone.position.x, newUnitClone.position.y + Random.Range(19f, 24f));
            //    ParasiteFighter unit = newUnitClone.GetComponent<ParasiteFighter>();
            //    unit.whatAreOurProjectiles = whatAreOurProjectiles;
            //    unit.whatIsTarget = whatIsTarget;
            //}
            //else if (newUnitClone.GetComponent<NormalShipWeapons>())
            //{
            //    NormalShipWeapons unit = newUnitClone.GetComponent<NormalShipWeapons>();
            //    unit.whatAreOurProjectiles = whatAreOurProjectiles;
            //    unit.whatIsTarget = whatIsTarget;
            //    unit.rocketFireAngle = fireAngle;
            //    //Debug.Log((int)whatAreOurProjectiles + " | " + (int)unit.whatAreOurProjectiles);
            //    //Debug.Log((int)whatIsTarget + " | " + (int)unit.whatIsTarget);
            //}
            //else if(newUnitClone.GetComponent<AircraftCarrier>())
            //{
            //    AircraftCarrier unit = newUnitClone.GetComponent<AircraftCarrier>();
            //    unit.whatAreOurProjectiles = whatAreOurProjectiles;
            //    unit.whatIsTarget = whatIsTarget;
            //    unit.rocketFireAngle = fireAngle;
            //    unit.menu.transform.localPosition = new Vector3(0f, -3f, 0f);
            //    unit.menu.transform.localEulerAngles = new Vector3(0f, newUnitClone.transform.localEulerAngles.y, newUnitClone.transform.localEulerAngles.y);
            //    //Debug.Log((int)whatAreOurProjectiles + " | " + (int)unit.whatAreOurProjectiles);
            //    //Debug.Log((int)whatIsTarget + " | " + (int)unit.whatIsTarget);
            //}
            //else if (newUnitClone.GetComponent<Submarine>())
            //{
            //    newUnitClone.position = new Vector2(newUnitClone.position.x, newUnitClone.position.y - Random.Range(5f, 17f));
            //    Submarine unit = newUnitClone.GetComponent<Submarine>();
            //    unit.whatAreOurProjectiles = whatAreOurProjectiles;
            //    unit.whatIsTarget = whatIsTarget;
            //    unit.rocketFireAngle = fireAngle;
            //    //Debug.Log((int)whatAreOurProjectiles + " | " + (int)unit.whatAreOurProjectiles);
            //    //Debug.Log((int)whatIsTarget + " | " + (int)unit.whatIsTarget);
            //}
            //else if (newUnitClone.GetComponent<BallisticSubmarine>())
            //{
            //    newUnitClone.position = new Vector2(newUnitClone.position.x, newUnitClone.position.y - Random.Range(20f, 45f));
            //    BallisticSubmarine unit = newUnitClone.GetComponent<BallisticSubmarine>();
            //    unit.whatAreOurProjectiles = whatAreOurProjectiles;
            //    unit.whatIsTarget = whatIsTarget;
            //    unit.rocketFireAngle = fireAngle;
            //    //Debug.Log((int)whatAreOurProjectiles + " | " + (int)unit.whatAreOurProjectiles);
            //    //Debug.Log((int)whatIsTarget + " | " + (int)unit.whatIsTarget);
            //}
            #endregion

            yield return new WaitForSecondsRealtime((float)newUnit.delayBetweenEachSpawn);
        }
    }
}
#pragma warning restore 0649