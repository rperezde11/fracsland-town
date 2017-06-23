using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// An enemy who is waiting the hero to kill him
/// </summary>
public class Lion : Mob {

    private new enum  MOB_ANIMATION
    {
        IDLE = 0,
        WALK = 1,
        RUN = 2,
        JUMP = 3,
        JUMP_ATTACK = 4,
        BITE = 5,
        ROAR = 6,
        CLAWS_ATTACK_COMBO = 7,
        GET_HIT = 8,
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
        PlayAnimation((int)MOB_ANIMATION.CLAWS_ATTACK_COMBO);
        JukeBox.instance.LoadAndPlaySound("lion", 0.6f);
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
        PlayAnimation( (int) MOB_ANIMATION.GET_HIT);
    }

    public override void SetupParticularAttributes(){}
}
