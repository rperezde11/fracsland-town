using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Utils;
using System;
using Quests;

/// <summary>
/// Used to setup scenes.
/// </summary>
public class LevelController : MonoBehaviour {

    public static LevelController instance;

    private float nextSpawnCheck = 0f;

	public ArrayList fractionableObjects;
	public GameObject levelStartPoint, levelEndPoint;

    public delegate void ActualLevelUpdateLogic();
    private ActualLevelUpdateLogic _actualUpdate = null;

    public Hero Hero { get { return _hero; } }
    private Hero _hero;

	public float minHeightBeforeHeroDead = 8.0f;

    public int DestinationPortal = 0;
    public bool placeInPortalRequired = false;
    public Vector3 heroPortalDestination;

    private Dictionary<SpawnPoint.SPAWN_TYPE, List<SpawnPoint>> spawnPoints;

    void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            fractionableObjects = new ArrayList();
            DontDestroyOnLoad(gameObject);
            _actualUpdate = DefaultUpdate;
        }

    }

    /// <summary>
    /// Used to place hero infront of a portal when the level has been loaded
    /// </summary>
    /// <param name="destinationPortalID"></param>
    public void RequestPositioningInfrontOfPortal(int destinationPortalID, string destinationScene, Vector3 destination)
    {
        heroPortalDestination = destination;
        QuestManager.instance.SaveActualQuest();
        DestinationPortal = destinationPortalID;
        string ds = destinationScene;
        placeInPortalRequired = true;
        LoadingScreenManager.LoadScene(ds);
        _actualUpdate = SpawningHeroUpdate;
    }


    void Start()
    {
        //JukeBox.instance.Play(JukeBox.instance.weather[0], 0.3f);

        SpamHero();
        ToggleFade.instance.FadeWithTextAndColor(Constants.SCENE_NAME_TOWN, Color.grey, 2f);
    }
    
    /// <summary>
    /// Get all the spawn points and make them spawn
    /// </summary>
    public void SpawnItems()
    {
        foreach (GameObject spawnPoint in GameObject.FindGameObjectsWithTag("SpawnPoint"))
        {
            SpawnPoint sp = spawnPoint.GetComponent<SpawnPoint>();
            sp.Spawn();
            spawnPoints[sp.type].Add(sp);
        }
    }
    
    /// <summary>
    /// Used to clear the counters that tracks how much entities has been spawmed
    /// </summary>
    public void RestartSpawnCounters()
    {
        spawnPoints = new Dictionary<SpawnPoint.SPAWN_TYPE, List<SpawnPoint>>();
        foreach(SpawnPoint.SPAWN_TYPE type in Enum.GetValues(typeof(SpawnPoint.SPAWN_TYPE)))
        {
            spawnPoints.Add(type, new List<SpawnPoint>());
        }

        Mob.NumMobsSpawned = 0;
        SpawnPoint.NumFractionAltairsSpawned = 0;
        nextSpawnCheck = 0f;
    }	

	public void SpamHero()
    {
        if (!GetStartPoint())
        {
            throw new System.Exception("E: No start point found!!");
        }
        
        if (GameObject.FindGameObjectsWithTag("Player").Length > 1)
        {
            throw new System.Exception("E: There is more than one player!!");
        }

        GameObject h;

        h = GameObject.Find("Hero(Clone)");

        if (h == null)
        {
            GameObject heroPrefab;

            if ( !(heroPrefab = (GameObject)Resources.Load(Constants.PATH_RESOURCES + "Hero") as GameObject) )
            {
                throw new Exception("Hero prefab not found!!");
            }

            h = (GameObject)Instantiate(heroPrefab, levelStartPoint.transform.position, levelStartPoint.transform.rotation);

            GameObject heroTarget;

            if ( !(heroTarget = (GameObject)Resources.Load(Constants.PATH_RESOURCES + "HeroTarget") as GameObject) )
            {
                throw new Exception("Hero target prefab not found!!");
            }

            heroTarget = GameObject.Instantiate(heroTarget);

            _hero = h.GetComponent<Hero>();
            _hero.target = heroTarget.GetComponent<HeroTarget>();
            heroTarget.transform.position = _hero.transform.position;

            StatsManager.instance.Setup();
            _hero.SetHealth(Hero.CurrentHealth);
        }


    }

	private bool GetStartPoint()
    {
		levelStartPoint = GameObject.Find ("HeroSpamPoint");

		if (levelStartPoint != null)
        {
			return true;
		}

		return false;
	}

	public void resurrectHero(){
        this.Hero.Ressurect();
		this.Hero.transform.position = this.levelStartPoint.transform.position;
        this.Hero.setNewDestinationPosition(Hero.transform.position);
        this.Hero.SetHealth(Hero.CurrentHealth);
        Hero.movementManager.StopMovement ();
        Hero.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = true;
        Hero.isControlledByPlayer = true;
        Hero.SetHeroUpdate (Hero.defaultUpdate);
        if(QuestManager.instance.ActualQuest != null)
            QuestManager.instance.ActualQuest.Restart();
    }

	public void DrownPlayer(){
		if(Constants.DEVELOPMENT) Debug.Log ("Hero dead: Drowned");
		JukeBox.instance.Play (JukeBox.instance.LoadSound ("hero_dead"), 1);
		resurrectHero ();
	}

	public void goToMainMenu(){
		Application.LoadLevel (Constants.SCENE_NAME_SPLASH_SCENE);
	}

    void DefaultUpdate()
    {
        if (Hero.isDead())
        {
            resurrectHero();
        }
    }

    /// <summary>
    /// Used to ensure that the hero is correctly placed, this avoids problems with the navmesh
    /// </summary>
    void SpawningHeroUpdate()
    {
        if (_hero == null) return;
        if(Vector3.Distance(_hero.transform.position, heroPortalDestination) > 0.1f)
        {
            _hero.transform.position = heroPortalDestination;
            Destroy(LevelController.instance.Hero.GetComponent<UnityEngine.AI.NavMeshAgent>());
        }
        else
        {
            RestartSpawnCounters();
            UnityEngine.AI.NavMeshHit closestHit;
            if (UnityEngine.AI.NavMesh.SamplePosition(heroPortalDestination, out closestHit, 500, 1))
            {
                _hero.transform.position = closestHit.position;
                _hero.gameObject.AddComponent<UnityEngine.AI.NavMeshAgent>();
                _hero.movementManager.NavAgent = LevelController.instance.Hero.GetComponent<UnityEngine.AI.NavMeshAgent>();
                _hero.movementManager.SetNewDestination(heroPortalDestination);

            }
            _actualUpdate = DefaultUpdate;
        }
    }

	void Update () {
        if (_actualUpdate != null) _actualUpdate();
	}

}
