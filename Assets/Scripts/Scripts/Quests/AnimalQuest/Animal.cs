using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// The base class used in Rabbit, Cow and Chicken.
/// </summary>
public abstract class Animal : LivingBeing {

    protected Animation _animation;
    protected List<string> _animations;

    public abstract void SelfSetup();
    public abstract void Setup();
    public abstract void UpdateAnimal();
    public abstract void MakeFuriousSound();

    protected MovementManager _movementManager;

    protected int i = 0;
    protected int _deadAnimIdx = 2;
    protected int _walkAnimation = 10;
    protected int _idleAnimation = 4;
    protected int _attackAnimation = 9;
    protected int _runAnimation = 10;

    protected Vector3 _startPosition;
    protected ActualLogicUpdate _actualLogicUpdateFunction = null;
    protected float _attackDistance = 1.6f;

    protected float _distanceToStop = 6; //Make animals keep a distance of 2 metters with our hero

    void Awake()
    {
        _animation = GetComponent<Animation>();
        _animations = new List<string>();
        _movementManager = GetComponent<MovementManager>();
        foreach (AnimationState state in _animation)
        {
            _animations.Add(state.name);
        }

        _startPosition = transform.position;
        SelfSetup();

    }

    void Start()
    {
        Setup();
        PlayAnimation(_idleAnimation);
    }

    public void FollowHero()
    {
        _actualUpdateLogic = FollowingHero;
    }

    public void FollowingHero()
    {
        //Follow our hero
        if(Vector3.Distance(LevelController.instance.Hero.transform.position, transform.position) > _distanceToStop)
        {
            _movementManager.SetNewDestination(LevelController.instance.Hero.transform.position);
            PlayAnimation(_walkAnimation);
        }
        else
        {
            _movementManager.StopMovement();
            PlayAnimation(_idleAnimation);
        }
    }

    public void ReturnToCage()
    {
        _actualUpdateLogic = ReturningToCage;
    }

    public void ReturningToCage()
    {
        if (Vector3.Distance(_startPosition, transform.position) >= 0.3)
        {
            _movementManager.SetNewDestination(_startPosition);
            PlayAnimation(_walkAnimation);
        }
        else
        {
            _movementManager.StopMovement();
            PlayAnimation(_idleAnimation);
            _actualUpdateLogic = null;
        }
    }

    public void AttackPlayer()
    {
        MakeFuriousSound();
        ChangeUpdateMethod(AttackingPlayer);
    }

    public void AttackingPlayer()
    {
        //Follow our hero
        if (Vector3.Distance(LevelController.instance.Hero.transform.position, transform.position) > _attackDistance)
        {
            _movementManager.SetNewDestination(LevelController.instance.Hero.transform.position);
            PlayAnimation(_runAnimation);
        }
        else
        {
            _movementManager.StopMovement();
            transform.LookAt(LevelController.instance.Hero.transform.position);
            PlayAnimation(_attackAnimation);
            ChangeUpdateMethod(null);
            LevelController.instance.Hero.GetHurt();
            ChangeUpdateMethodIn(1, FollowingHero);
        }
    }

    void Update()
    {
        if(_actualUpdateLogic != null ) _actualUpdateLogic();
    }

    public void PlayAnimation(int animation)
    {
        _animation.Play(_animations[animation], PlayMode.StopAll);
    }

    protected override void Die()
    {
        PlayAnimation(_deadAnimIdx);
    }
}
