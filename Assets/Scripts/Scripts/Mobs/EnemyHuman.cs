using UnityEngine;

/// <summary>
/// An enemy who is looking for the player moving inside a network of nodes
/// and when he finds him he tries to kill him.
/// </summary>
public class EnemyHuman : Mob
{

    private new enum MOB_ANIMATION
    {
        RUN = 0,
        WALK_FRONT = 1,
        WALK_BACK = 2,
        WALK_RIGHT = 3,
        WALK_LEFT = 4,
        IDLE_1 = 5,
        IDLE_2 = 6,
        ATTACK_OR_MINE = 7,
        ATTACK_OR_MINE2 = 8,
        LUMBERING = 9,
        SAWING = 10,
        BUILD_1 = 11,
        BUILD_2 = 12,
        DIGGING = 13,
        USE = 14,
        GATHER = 15,
        TALK = 16,
        HIT1 = 17,
        HIT2 = 18,
        DEATH = 19,
        JUMP = 20
    }

    public override void PatrolEffect()
    {
        PlayAnimation((int)MOB_ANIMATION.WALK_FRONT);
    }

    public override void FollowEffect()
    {
        PlayAnimation((int)MOB_ANIMATION.RUN);
    }

    public override void AttackEffect()
    {
        PlayAnimation((int)MOB_ANIMATION.ATTACK_OR_MINE);
    }

    public override void IdleEffect()
    {
        PlayAnimation((int)MOB_ANIMATION.IDLE_1);
    }

    public override void DeadEffect()
    {
        PlayAnimation((int)MOB_ANIMATION.DEATH);
        DropReward();
        Destroy(gameObject, 2f);
    }

    public override void StunEffect()
    {
        PlayAnimation((int)MOB_ANIMATION.HIT2);
    }

    public override void SetupParticularAttributes()
    {
        _patrolNodes = GameObject.FindGameObjectsWithTag("HumanPatrolNode");
        ChooseRandomPatrolNode();
    }

    public void FallToTheGround()
    {
        ChangeUpdateMethod(null);
        PlayAnimation((int)MOB_ANIMATION.DEATH);
        ChangeUpdateMethodIn(4, _lastUpdateLogic);
        _movementManager.SetNewDestination(new Vector3(transform.position.x + UnityEngine.Random.RandomRange(-1f, 1f), transform.position.y, transform.position.z + UnityEngine.Random.RandomRange(-1f, 1f)), true);

    }
}
