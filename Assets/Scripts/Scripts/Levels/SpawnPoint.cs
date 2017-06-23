using UnityEngine;
using Utils;
using System.Collections.Generic;

/// <summary>
/// This class 
/// </summary>
public class SpawnPoint : MonoBehaviour {

    public enum SPAWN_TYPE { FRACTION_ALTAIR, LIONS, PALM, BOX, BUSH, HUMAN_ENEMY, WOLF, WINTER_EVIROMENT }

    public static Dictionary<SPAWN_TYPE, float> SpawnDensityDict = new Dictionary<SPAWN_TYPE, float>()
    {
        { SPAWN_TYPE.FRACTION_ALTAIR, 0.5f},
        { SPAWN_TYPE.LIONS, 0.5f},
        { SPAWN_TYPE.PALM, 1f},
        { SPAWN_TYPE.BOX, 0.5f},
        { SPAWN_TYPE.BUSH, 0.6f},
        { SPAWN_TYPE.HUMAN_ENEMY, 0.5f },
        { SPAWN_TYPE.WOLF, 0.5f },
        { SPAWN_TYPE.WINTER_EVIROMENT, 0.6f },
    };

    public static Dictionary<SPAWN_TYPE, float> SpawnChanceToAppear = new Dictionary<SPAWN_TYPE, float>()
    {
        { SPAWN_TYPE.FRACTION_ALTAIR, 0.3f},
        { SPAWN_TYPE.LIONS, 0.005f},
        { SPAWN_TYPE.PALM, 0.1f},
        { SPAWN_TYPE.BOX, 0.2f},
        { SPAWN_TYPE.BUSH, 0.01f},
        { SPAWN_TYPE.HUMAN_ENEMY, 0.005f },
        { SPAWN_TYPE.WOLF, 0.005f },
        { SPAWN_TYPE.WINTER_EVIROMENT, 0.01f },
    };


    //Static prefabs
    //Items prefabs
    static GameObject fractionAltairPrefab;
    static GameObject boxPrefab;

    //Vegetation prefabs
    //Summer
    static GameObject palmPrefab;
    static GameObject bushPrefab;

    //Winter
    static GameObject WinterSmallBush;
    static GameObject WinterStone1;
    static GameObject WinterStone2;
    static GameObject WinterStone3;
    static GameObject WinterThinBush;
    static GameObject WinterSprunce;

    //Human Prefabs
    static GameObject HumanEnemyOne;
    static GameObject HumanEnemyTwo;
    static GameObject JohnTheBuilder;

    //Animal Prefabs
    static GameObject lionPrefab;
    static GameObject Wolf;
    static GameObject Bear;

    public SPAWN_TYPE type = SPAWN_TYPE.FRACTION_ALTAIR;

    private List<GameObject> _spawnedObjects;
    private List<Vector3> _possibleLocations;

    int _numSpawnedObjects = 0;

    private MeshRenderer _meshRenderer;
    private bool canSpawn = true;

    public static int NumFractionAltairsSpawned = 0;

    void Awake()
    {
        _spawnedObjects = new List<GameObject>();
        _possibleLocations = new List<Vector3>();
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    void OnEnable()
    {
        LoadStaticResources();
        Spawn();
        Destroy(gameObject);
    }

    public void LoadStaticResources()
    {
        if (fractionAltairPrefab != null) return;

        //Items prefabs
        fractionAltairPrefab = Resources.Load(Constants.PATH_RESOURCES + "altair") as GameObject;
        boxPrefab = Resources.Load(Constants.PATH_RESOURCES + "Box") as GameObject;

        //Vegetation prefabs
        //Summer
        palmPrefab = Resources.Load(Constants.PATH_RESOURCES + "Palm") as GameObject;
        bushPrefab = Resources.Load(Constants.PATH_RESOURCES + "bush") as GameObject;

        //Winter
        WinterSmallBush = Resources.Load(Constants.PATH_RESOURCES + "WinterSmallBush") as GameObject;
        WinterStone1 = Resources.Load(Constants.PATH_RESOURCES + "WinterStone1") as GameObject;
        WinterStone2 = Resources.Load(Constants.PATH_RESOURCES + "WinterStone2") as GameObject;
        WinterStone3 = Resources.Load(Constants.PATH_RESOURCES + "WinterStone3") as GameObject;
        WinterThinBush = Resources.Load(Constants.PATH_RESOURCES + "WinterThinBush") as GameObject;
        WinterSprunce = Resources.Load(Constants.PATH_RESOURCES + "WinterSprunce") as GameObject;

        //Human Prefabs
        HumanEnemyOne = Resources.Load(Constants.PATH_RESOURCES + "Enemy1") as GameObject;
        HumanEnemyTwo = Resources.Load(Constants.PATH_RESOURCES + "Enemy2") as GameObject;
        JohnTheBuilder = Resources.Load(Constants.PATH_RESOURCES + "JohnTheBuilder") as GameObject;

        //Animal Prefabs
        lionPrefab = Resources.Load(Constants.PATH_RESOURCES + "LION") as GameObject;
        Wolf = Resources.Load(Constants.PATH_RESOURCES + "Wolf") as GameObject;
        Bear = Resources.Load(Constants.PATH_RESOURCES + "Bear") as GameObject;
    }

    public void Spawn()
    {
        if (!canSpawn) return;
        GetPossibleLocations();
        switch (type)
        {
            case SPAWN_TYPE.FRACTION_ALTAIR:
                SpawnFractionAltair();
                break;
            case SPAWN_TYPE.LIONS:
                SpawnObjectsOfType(lionPrefab, Mob.MaxMobsToSpawn - Mob.NumMobsSpawned);
                Mob.NumMobsSpawned = _spawnedObjects.Count;
                break;
            case SPAWN_TYPE.HUMAN_ENEMY:
                SpawnRandomEnemies(Mob.MaxMobsToSpawn - Mob.NumMobsSpawned);
                Mob.NumMobsSpawned = _spawnedObjects.Count;
                break;
            case SPAWN_TYPE.WOLF:
                SpawnObjectsOfType(Wolf, Mob.MaxMobsToSpawn - Mob.NumMobsSpawned);
                Mob.NumMobsSpawned = _spawnedObjects.Count;
                break;
            case SPAWN_TYPE.PALM:
                SpawnObjectsOfType(palmPrefab);
                break;
            case SPAWN_TYPE.BOX:
                break;
            case SPAWN_TYPE.BUSH:
                SpawnObjectsOfType(bushPrefab);
                break;
            case SPAWN_TYPE.WINTER_EVIROMENT:
                SpawnWinterElements();
                break;
        }
        _numSpawnedObjects = _spawnedObjects.Count;
        _meshRenderer.enabled = false;
        if(_numSpawnedObjects > 0)
        {
            canSpawn = false;
        }
    }

    void GetPossibleLocations()
    {
        //First of all let's find the 2 corners of the game object
        float scaleX = transform.localScale.x;
        float scaleZ = transform.localScale.z;
        Vector3 topLeftCorner = transform.position - new Vector3(scaleX / 2, 0, scaleZ / 2);
        Vector3 bottomRightCorner = transform.position + new Vector3(scaleX / 2, 0, scaleZ / 2);
        
        //We need to precise the heigth
        float heigth = GetPointHeight(transform.position);

        //Get density
        float density = SpawnDensityDict[type];

        for(float i = topLeftCorner.x; i < bottomRightCorner.x; i+=density)
        {
            for (float j = topLeftCorner.z; j < bottomRightCorner.z; j += density)
            {
                _possibleLocations.Add(new Vector3(i, heigth, j));
            }
        }

        for (int i = 0; i < _possibleLocations.Count; i++)
        {
            Vector3 temp = _possibleLocations[i];
            int randomIndex = Random.Range(i, _possibleLocations.Count);
            _possibleLocations[i] = _possibleLocations[randomIndex];
            _possibleLocations[randomIndex] = temp;
        }
    }

    void SpawnFractionAltair()
    {
        if (!ChooseIfAppear())
        {
            if(NumFractionAltairsSpawned == 0)
            {
                SpawnFractionAltair();
            }
            return;
        }
        //Choose 1 position for the altair
        Vector3 position = _possibleLocations[Constants.randomNumberGenerator.Next(_possibleLocations.Count)];
        //Correct heigth
        position += new Vector3(0, 0.38742f, 0);
        SpawnItem(fractionAltairPrefab, position, fractionAltairPrefab.transform.rotation);
        NumFractionAltairsSpawned++;
    }

    void SpawnRandomEnemies(int max = int.MaxValue)
    {
        int itemsSpawned = 0;
        foreach (Vector3 location in _possibleLocations)
        {
            if (ChooseIfAppear())
            {
                GameObject obj;
                float rnd = Random.RandomRange(0f, 1f);
                if(rnd > 0.5f)
                {
                    obj = HumanEnemyOne;
                }
                else
                {
                    obj = HumanEnemyTwo;
                }
                if (itemsSpawned == max) break;
                SpawnItem(obj, location, obj.transform.rotation);
                itemsSpawned++;
            }
        }
    }

    void SpawnWinterElements()
    {
        //Like this is easier to take one
        List<GameObject> WinterGameObjects = new List<GameObject>();
        WinterGameObjects.Add(WinterSmallBush);
        WinterGameObjects.Add(WinterStone1);
        WinterGameObjects.Add(WinterStone2);
        WinterGameObjects.Add(WinterStone3);
        WinterGameObjects.Add(WinterThinBush);
        WinterGameObjects.Add(WinterSprunce);  

        foreach (Vector3 location in _possibleLocations)
        {
            if (ChooseIfAppear())
            {
                GameObject randomElement = WinterGameObjects[Random.Range(0, WinterGameObjects.Count)];
                Debug.Log(randomElement);
                Debug.Log(location);
                SpawnItem(randomElement, location, randomElement.transform.rotation);
            }
        }
    }

    void SpawnObjectsOfType(GameObject obj, int max=int.MaxValue)
    {
        int itemsSpawned = 0;
        foreach (Vector3 location in _possibleLocations)
        {
            if (ChooseIfAppear())
            {
                if (itemsSpawned == max) break;
                try
                {
                    SpawnItem(obj, location, obj.transform.rotation);
                }
                catch(System.Exception ex)
                {
                }
                itemsSpawned++;
            }

        }
    }

    void SpawnBox()
    {

    }
   
    private float GetPointHeight(Vector3 point)
    {
        RaycastHit hit;
        if (Physics.Raycast(point, Vector3.down, out hit)) return hit.point.y;
        if (Physics.Raycast(point, Vector3.up, out hit)) return hit.point.y;
        return float.NegativeInfinity;
    }

    private bool ChooseIfAppear()
    {
        return Constants.randomNumberGenerator.NextDouble() < SpawnChanceToAppear[type];
    }

    private void SpawnItem(GameObject itemPrefab, Vector3 position, Quaternion rotation)
    {
        GameObject obj = GameObject.Instantiate(itemPrefab, position, rotation) as GameObject;
        _spawnedObjects.Add(obj);
    }

    public int GetNumSpawnsNeeded()
    {
        List<int> indexToRemove = new List<int>();
        for(int i = 0; i < _spawnedObjects.Count; i++)
        {
            GameObject go = _spawnedObjects[i];
            if(go == null && !ReferenceEquals(go, null)){
                indexToRemove.Add(i);
            }
        }
        foreach(int i in indexToRemove)
        {
            _spawnedObjects.RemoveAt(i);
        }

        if(indexToRemove.Count > 0)
        {
            canSpawn = true;
        }

        return indexToRemove.Count;
    }



}
