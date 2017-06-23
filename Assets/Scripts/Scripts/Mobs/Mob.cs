using UnityEngine;
using System.Collections.Generic;
using Items;
using Utils;
using Quests;

/// <summary>
/// This is the base class of an enemy
/// </summary>
[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
[RequireComponent(typeof(MovementManager))]
public abstract class Mob : LivingBeing
{
    public static int NumMobsSpawned = 0;
    public static int MaxMobsToSpawn = 20;

    protected GameObject[] _patrolNodes;

    public enum MOB_ANIMATION { }
    protected enum ACTUAL_STATE { PATROL, FOLLOW, ATTACK, IDLE, DEAD }
    protected MovementManager _movementManager;
    protected ActualLogicUpdate _patrolLogic, _followLogic, _attackLogic, _idleLogic, _deadLogic;
    protected int _attackForce;
    protected float _speed, _attackSpeed, _distanceVision, _distanceToAttack, _nextAllowedAttackTime;
    protected ACTUAL_STATE _actualState;

    protected List<string> _animations;
    protected Animation _animation;

    public abstract void PatrolEffect();
    public abstract void FollowEffect();
    public abstract void AttackEffect();
    public abstract void IdleEffect();
    public abstract void DeadEffect();
    public abstract void StunEffect();
    public abstract void SetupParticularAttributes();

    protected bool _hasDroppedGold = false;

    protected GameObject _patrolTarget = null;

    protected bool _isAttackEnabled = true;
    string _mobname;

    void Awake()
    {
        _health = 100;
        _maxHealth = 100f;
        _actualLifeState = LIFE_STATE.ALIVE;
        _attackForce = 10;
        _attackSpeed = 1;
        _distanceVision = 2f;
        _speed = 2f;
        _actualUpdateLogic = PatrolLogic;
        _distanceToAttack = 1f;
        _mobname = gameObject.name.Replace("(Clone)", "");

    }

    void Start()
    {
        SetupCommonComponents();
        SetupParticularAttributes();

    }

    protected void SetupCommonComponents()
    {
        _animation = GetComponent<Animation>();
        _animations = new List<string>();
        _movementManager = GetComponent<MovementManager>();
        foreach (AnimationState state in _animation)
        {
            _animations.Add(state.name);
        }

        if(HealthBarPosition != null)
        {
            _healthBar = GUIController.instance.CreateHealthBar(this);
        }
    }

    protected void DropReward()
    {
        if (_hasDroppedGold) return;
        GameController.instance.GiveRandomMoney();
        StatsManager.instance.AddExperience(30, false);
        _hasDroppedGold = true;

        if (Random.Range(0f, 1f) > 0.7)
        {
            QuestManager.instance.Inventory.AddPotions(Random.Range(1, 2));
        }

        if (Random.Range(0f, 1f) > 0.9) 
        {
            GameController.instance.GiveRandomFraction();
        }
         
    }

    protected virtual void PatrolLogic()
    {
        _movementManager.RestoreMovement();
        if (isInVisionRange(LevelController.instance.Hero.gameObject)) _actualUpdateLogic = FollowLogic;

        if (_patrolNodes == null || _patrolNodes.Length == 0)
        {
            IdleEffect();
        }
        else
        {
            if (Vector3.Distance(_patrolTarget.transform.position, transform.position) < 1)
            {
                ChooseRandomPatrolNode();
            }

            PatrolEffect();
        }
    }

    public void Stun(float stunnedTime)
    {
        ActualLogicUpdate up = _actualUpdateLogic;
        ChangeUpdateMethodIn(stunnedTime, up, true);
        StunEffect();
    }

    protected virtual void FollowLogic()
    {

        //If the hero isn't in the vision range just patrol
        if (!isInVisionRange(LevelController.instance.Hero.gameObject)) {
            _actualUpdateLogic = PatrolLogic;
            _actualState = ACTUAL_STATE.FOLLOW;
        }

        //If hero isn't in the attack range just 
        if (!isInAttackRange(LevelController.instance.Hero.gameObject)){
            _movementManager.SetNewDestination(LevelController.instance.Hero.gameObject.transform.position);
            FollowEffect();
        }
        else
        {
            _actualUpdateLogic = AttackLogic;
            _actualState = ACTUAL_STATE.ATTACK;
        }
        
    }
        
    protected virtual void AttackLogic()
    {
        if (isInAttackRange(LevelController.instance.Hero.gameObject))
        {
            if (!_isAttackEnabled)
            {
                IdleEffect();
                return;
            }
            
            if (_nextAllowedAttackTime <= Time.time)
            {
                LevelController.instance.Hero.TakeDammage(_attackForce);
                _nextAllowedAttackTime = Time.time + _attackSpeed;
                AttackEffect();      
            }

        }
        else
        {
            _actualUpdateLogic = FollowLogic;
            _actualState = ACTUAL_STATE.FOLLOW;
        }
    }

    void Update()
    {
        if (isDead())
        {
            try{
                Destroy(_healthBar.gameObject);
            }
            catch (MissingReferenceException){}
            
            _actualUpdateLogic = DeadEffect;
        }
        else
        {
            if (this.GetNormalizedHealth() < 1)
            {
                _healthBar.show();
            }
        }

        _actualUpdateLogic();
    }

    protected virtual void IdleLogic()
    {
        IdleEffect();
    }

    protected virtual void DeadLogic()
    {
        DeadEffect();
    }

    protected bool isInAttackRange(GameObject target)
    {
        return Vector3.Distance(target.transform.position, transform.position) <= _distanceToAttack;
    }

    protected bool isInVisionRange(GameObject target)
    {
        return Vector3.Distance(target.transform.position, transform.position) <= _distanceVision;
    }

    protected override void Die()
    {
        _actualLifeState = LIFE_STATE.DEAD;
    }

    protected void PlayAnimation(int animation)
    {
        _animation.Play(_animations[animation], PlayMode.StopAll);
    }


    void OnMouseOver()
    {

        if (GUIController.instance.onGUIOpened) return;

        GUIController.instance.SetCursor(GUIController.CURSOR_TYPE.CUSOR_ATTACK);
        GUIController.instance.showInspectedItem(_mobname, gameObject);
        

        if (Vector3.Distance(this.transform.position, LevelController.instance.Hero.gameObject.transform.position) < 1.5f)
        {
            if (Input.GetMouseButtonDown(0))
            {
                LevelController.instance.Hero.FastAttack(this);
            }
            else if (Input.GetMouseButtonDown(1))
            {
                LevelController.instance.Hero.HeavyAttack(this);
            }

        }
        else
        {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                LevelController.instance.Hero.setNewDestinationPosition(transform.position);
            }
        }
    }

    void OnMouseExit()
    {
        GUIController.instance.SetCursor(GUIController.CURSOR_TYPE.CURSOR_DEFAULT);
        GUIController.instance.hideInspector();
        LevelController.instance.Hero.interactingItem = null;
    }

    protected void ChooseRandomPatrolNode()
    {
        if(_patrolNodes.Length > 0)
        {
            _patrolTarget = _patrolNodes[UnityEngine.Random.Range(0, _patrolNodes.Length)];
            _movementManager.SetNewDestination(_patrolTarget.transform.position);
        }
    }


    public void EnableAttack()
    {
        _isAttackEnabled = true;
    }

    public void DisableAttack()
    {
        _isAttackEnabled = false;
    }

}
