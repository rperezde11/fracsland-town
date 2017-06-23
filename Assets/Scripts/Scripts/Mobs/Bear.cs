using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// This mob is not used right now but with this class
/// it won't be difficult to add it into the game as
/// a new enemy.
/// </summary>
public class Bear : Mob
{
    public static int NumLionsSpawned = 0;
    public static int MaxLionsToSpawn = 40;

    private new enum MOB_ANIMATION
    {
        TAKE = 0,
        STAND_BITE_ATTACK = 1,
        STAND_CLAWS_ATTACK_LEFT,
        STAND_CLAWS_ATTACK_RIGHT,
        STAND_DEATH = 4,
        STAND_GET_HI = 5,
        STAND_ROAR = 6,
        STAND_TO_4_LEGS = 7,
        STAND_BITE = 8,
        CLAWS_ATTACK_LEFT = 9,
        CLAWS_ATTACK_RIGHT = 10,
        DEATH = 11,
        GET_HIT = 12,
        ROAR = 13,
        STAND_UP = 14,
        GET_HIT_LEFT = 15,
        GET_HIT_RIGHT = 16,
        IDLE_STAND = 17,
        IDLE_4_LEGS = 18,
        PARALIZED = 19,        
        PUNCH_FRONTAL = 20,
        PUNCH_LEFT = 21,
        PUNCH_RIGHT = 22,
        RUN = 23,
        WALK = 24,
        BITE = 25
    };

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
    }

    void Start()
    {
        SetupCommonComponents();
    }

    public override void PatrolEffect()
    {
        PlayAnimation((int)MOB_ANIMATION.WALK);
    }

    public override void FollowEffect()
    {
        PlayAnimation((int)MOB_ANIMATION.RUN);
    }

    public override void AttackEffect()
    {
        PlayAnimation((int)MOB_ANIMATION.BITE);
    }

    public override void IdleEffect()
    {
        PlayAnimation((int)MOB_ANIMATION.IDLE_STAND);
    }

    public override void DeadEffect()
    {
        PlayAnimation((int)MOB_ANIMATION.DEATH);
        DropReward();
        Destroy(gameObject, 2f);
    }

    public override void StunEffect()
    {
        PlayAnimation((int)MOB_ANIMATION.GET_HIT);
    }

    public override void SetupParticularAttributes()
    {
        _health = 300;
    }
}
