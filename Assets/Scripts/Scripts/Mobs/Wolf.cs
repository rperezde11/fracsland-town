using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// This is an enemy who is more or less like the HumanEnemy, he
/// tryies to find the player and kill him.
/// </summary>
public class Wolf : Mob
{

    public static int NumLionsSpawned = 0;
    public static int MaxLionsToSpawn = 40;

    private new enum MOB_ANIMATION
    {
        IDLE_LOOK_ARROUND = 0,
        IDLE = 1,
        HOWL = 2,
        WALK = 3,
        RUN = 4,
        BITE = 5,
        STAND_BYTE = 6,
        GET_HIT = 7,
        DEATH = 8,
        DEAD = 9
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
        JukeBox.instance.LoadAndPlaySound("wolf", 0.15f);
        PlayAnimation((int)MOB_ANIMATION.BITE);
    }

    public override void IdleEffect()
    {
        PlayAnimation((int)MOB_ANIMATION.IDLE);
    }

    public override void DeadEffect()
    {
        PlayAnimation((int)MOB_ANIMATION.DEAD);
        DropReward();
        Destroy(gameObject, 2f);
    }

    public override void StunEffect()
    {
        PlayAnimation((int)MOB_ANIMATION.GET_HIT);
    }

    public override void SetupParticularAttributes()
    {
        _patrolNodes = GameObject.FindGameObjectsWithTag("AnimalPatrolNode");
        ChooseRandomPatrolNode();
    }


}
