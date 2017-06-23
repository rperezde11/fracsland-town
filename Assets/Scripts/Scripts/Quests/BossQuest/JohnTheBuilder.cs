using UnityEngine;

/// <summary>
/// The boss of the boss quest it's like an enemy human but inmortal.
/// </summary>
public class JohnTheBuilder : EnemyHuman {

    public override void SetupParticularAttributes()
    {
        base.SetupParticularAttributes();
        _maxHealth = 300;
        _health = _maxHealth;
        _attackForce = 30;
        _attackSpeed = 3;
        _isAttackEnabled = false;
    }

    public void DowngradeForce()
    {
        _attackForce = 1;
        GameObject.Find("CaptainsShield").SetActive(false);
        _isAttackEnabled = true;
        ChangeUpdateMethodIn(1, FollowLogic);
    }

    /// <summary>
    /// If the player fails to answer captain jonhn's fractions he will get a little punch
    /// </summary>
    public void AttackAndStunPlayer()
    {
        LevelController.instance.Hero.TakeDammage(_attackForce);
        AttackEffect();
        ChangeUpdateMethod(null);
        ChangeUpdateMethodIn(1, FollowLogic);
        LevelController.instance.Hero.GetHurt();
    }
}
