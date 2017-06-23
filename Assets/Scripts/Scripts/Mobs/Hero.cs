using UnityEngine;
using UnityEngine.VR;
using UnityEngine.EventSystems;
using Utils;
using Items;
using Quests;
using UnityEngine.SceneManagement;
using System;

/// <summary>
/// This class handles all the movement, actions and skills of our hero.
/// </summary>
public class Hero : HumanNPC {

    const float HERO_BASE_HEALTH = 100f;
    const float HERO_BASE_ATTACK = 10f;
    const float HERO_Heavy_ATTACK_BONUS = 20f;
    const float HERO_BASE_SPEED = 2f;

	public EventSystem eventSystem;
	private float heroYCoord;

	//Constants for the hero
	private const int HERO_SPEED = 2;
    private const float FAST_ATTACK_RATE = 0.5f;
    private const float HEAVY_ATTACK_RATE = 1f;
    
    private HeroTarget _target = null;
    public HeroTarget target { set { _target = value; } }

    private Interactable _interactingItem;
    public Interactable interactingItem {get { return _interactingItem; } set { _interactingItem = value; } }

    public bool hasReachedTarget = false;

    public float NextAllowedAttackTime { get { return nextAtackTime; } }
    private float nextAtackTime = 0f;

    public bool isControlledByPlayer;
	public MovementManager movementManager;

	public bool canFireEvents = true;

    GameObject _minimapPlayer;

    public static float CurrentHealth = 0f;

	public void defaultUpdate()
	{
        if (!isControlledByPlayer && movementManager.isMoving)
			return;

		if (isControlledByPlayer) updateInput();
		moveToTarget();		
	}

	public void updateInput()
    {
        if (UnityEngine.VR.VRSettings.enabled)
        {
            JoystickInput(); 
        }
        else
        {
            MouseInput();
        }
    }

    private void MouseInput()
    {
        if (GUIController.instance.onGUIOpened) return;
        //if touching the screen (finger or mouse)
        if (((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || (Input.GetMouseButton(0))) && !GUIController.instance.onGUIOpened)
        {
            //If not touching the GUI
            if (!eventSystem.IsPointerOverGameObject())
            {
                RaycastHit hit;
                Ray ray;
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                
                #if UNITY_EDITOR
                    ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                #elif (UNITY_ANDROID || UNITY_IPHONE || UNITY_WP8)
				    ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                #endif

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.name.Equals("Terrain"))
                    {
                        setNewDestinationOfHero(hit);
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            _target.MarkDestination();
        }
    }

    private void JoystickInput()
    {
        Vector3 movement = Vector3.zero;
        float movementIntensity = 0;

        Vector3 direction = Camera.main.transform.parent.localRotation * Vector3.forward;

        movementIntensity = Input.GetAxis("Vertical");
        movement += movementIntensity * direction * Time.deltaTime;

        float rotationValue;

        if ((rotationValue = Input.GetAxis("JoystickRightX")) > 0)
        {
            gameObject.transform.Rotate(Vector3.up, 25 * rotationValue * Time.deltaTime, Space.Self);
        }

        if ((rotationValue = Input.GetAxis("JoystickRightX")) < 0)
        {
            gameObject.transform.Rotate(Vector3.up, (25 * rotationValue * Time.deltaTime), Space.Self);
        }

        // 25 to speed up and try to make more straight lines
        setNewDestinationOfHero(25 * movement);
    }

    public void Lumber()
    {
        if (!CanAttack()) return;
        stopMoving();
        PlayAnimation(NPC_ANIMATION.LUMBERING);
        nextAtackTime = Time.time + 2f;
        ChangeUpdateMethodIn(2f, defaultUpdate, true);
    }

    public void FastAttack(Mob enemy)
    {
        if (!CanAttack()) return;
        stopMoving();
        PlayAnimation(NPC_ANIMATION.ATTACK_OR_MINE);

        transform.LookAt(enemy.transform);
        nextAtackTime = Time.time + FAST_ATTACK_RATE;
        enemy.TakeDammage(HERO_BASE_ATTACK + StatsManager.instance.AttackBonus);
        ChangeUpdateMethodIn(FAST_ATTACK_RATE, defaultUpdate, true);
    }

    public void HeavyAttack(Mob enemy)
    {
        if (!CanAttack()) return;
        stopMoving();
        PlayAnimation(NPC_ANIMATION.ATTACK_OR_MINE2);

        transform.LookAt(enemy.transform);
        nextAtackTime = Time.time + HEAVY_ATTACK_RATE;
        enemy.TakeDammage(HERO_BASE_ATTACK + StatsManager.instance.AttackBonus + HERO_Heavy_ATTACK_BONUS);
        ChangeUpdateMethodIn(HEAVY_ATTACK_RATE, defaultUpdate, true);
        enemy.Stun(HEAVY_ATTACK_RATE * 2f);
    }

    public bool CanAttack()
    {
        return nextAtackTime < Time.time;
    }

    private void setNewDestinationOfHero(RaycastHit hit){
        setNewDestinationPosition(hit.point);
	}

    private void setNewDestinationOfHero(Vector3 direction)
    {
        setNewDestinationPosition(transform.position + direction);
    }

    public new void TakeDammage(float dmg)
    {
        base.TakeDammage(dmg);
        Hero.CurrentHealth = _health;
        Debug.Log("Damage Saved:" + Hero.CurrentHealth);
    }

    public void SetHealth(float health)
    {
        if(health > 0)
        {
            _health = health;
            Debug.Log("Health Setup:" + _health);
        }
        else
        {
            Hero.CurrentHealth = 100;
        }
    }

    /// <summary>
    /// Hero need to be moved by other scripts
    /// </summary>
    /// <param name="dest"></param>
    public void setNewDestinationPosition(Vector3 dest)
    {
        if (_target == null)
        {
            _target = GameObject.Find("HeroTarget(Clone)").GetComponent<HeroTarget>();
        }
        else
        {
            hasReachedTarget = false;
            _target.transform.position = dest;

            if (movementManager.SetNewDestination(dest, VRSettings.enabled && VRDevice.isPresent))
            {
                PlayAnimation(NPC_ANIMATION.RUN);
            }
        }
    }

	private void moveToTarget(){

		Vector3 heroCurrentPosition;
        heroCurrentPosition = transform.position;

        if (Vector3.Distance (heroCurrentPosition, _target.transform.position) < 0.2f) {
            hasReachedTarget = true;
            _target.StopBlinking();
            PlayAnimation(NPC_ANIMATION.IDLE_1);       
        }
	}

    public void stopMoving()
    {
        _movementManager.StopMovement();
        PlayAnimation(NPC_ANIMATION.IDLE_1);
    }


    public int getSelectedItemID()
    {
        if(StatsManager.instance.Weapon == null)
        {
            return -1;
        }
        return StatsManager.instance.Weapon.ID;
    }

    public void useFraction(Fraction fraction)
    {

        if(interactingItem.selectedGameEvent == Quests.QuestManager.GameEventType.SOLVE_EVENT ||
            interactingItem.selectedGameEvent == Quests.QuestManager.GameEventType.FARM_EVENT)
        {
            QuestManager.instance.ActualQuest.Solve(fraction);
        }
        else
        {
            Debug.Log("No need to fire quest event");
        }
    }

    public void equipItem(Item item)
    {
        if(item != null && item.IsEquipable)
            StatsManager.instance.EquipItem((EquippableItem) item);
    }

	public void SetHeroUpdate(ActualLogicUpdate updateCallback){
        _actualUpdateLogic = updateCallback;
	}

    protected override void Die()
    {
        throw new NotImplementedException();
    }

    public override void SelfSetup()
    {
        isControlledByPlayer = true;
        heroYCoord = gameObject.transform.position.y;
        PlayAnimation(NPC_ANIMATION.IDLE_1);
        movementManager = GetComponent<MovementManager>();  
    }

    public override void Setup()
    {
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        MainCameraController.instance.AttachCameraToGameObject(this.gameObject);
        movementManager.SetNewDestination(_target.transform.position);
        movementManager.speed = 2f; 
        _lastUpdateLogic = defaultUpdate;
        _actualUpdateLogic = defaultUpdate;
        _health = CurrentHealth;
        _maxHealth = 100;

        string sceneName = SceneManager.GetActiveScene().name;
        _minimapPlayer = transform.Find("minimapplayer").gameObject;
        switch (sceneName)
        {
            case Constants.SCENE_NAME_FOREST:
                _minimapPlayer.transform.localScale = new Vector3(100f, 100f);
                break;
            default:
                 _minimapPlayer.transform.localScale = new Vector3(15f, 15f);
                break;
        }
    }

    public override void UpdateHuman()
    {
        if (_actualUpdateLogic != null) _actualUpdateLogic();
    }

    public void GetHurt()
    {
        ChangeUpdateMethod(null);
        PlayAnimation(NPC_ANIMATION.DEATH);
        ChangeUpdateMethodIn(4, defaultUpdate);
        _movementManager.SetNewDestination(new Vector3(transform.position.x + UnityEngine.Random.RandomRange(-1f, 1f), transform.position.y, transform.position.z + UnityEngine.Random.RandomRange(-1f, 1f)), true);
    }

    public void DieDrowned()
    {
        Bridge bridge = GameObject.Find("Bridge").GetComponent<Bridge>();
        ChangeUpdateMethod(null);
        movementManager.SetNewDestination(bridge.drownNodes[0].transform.position, true);
        ChangeUpdateMethod(DieDrownedUpdate);
        transform.LookAt(bridge.drownNodes[0].transform.position);
        PlayAnimation(NPC_ANIMATION.RUN);
    }

    public void DieDrownedUpdate()
    {
        if (!movementManager.isMoving)
        {
            Bridge bridge = GameObject.Find("Bridge").GetComponent<Bridge>();
            if(Vector3.Distance(transform.position, bridge.drownNodes[0].transform.position) < 0.1f)
            {
                movementManager.SetNewDestination(bridge.drownNodes[1].transform.position, true);
                PlayAnimation(NPC_ANIMATION.DEATH);
            }
            if (Vector3.Distance(transform.position, bridge.drownNodes[1].transform.position) < 0.1f)
            {
                LevelController.instance.DrownPlayer();
                ChangeUpdateMethod(defaultUpdate);
                PlayAnimation(NPC_ANIMATION.IDLE_1);
            }
        }   
    }
}