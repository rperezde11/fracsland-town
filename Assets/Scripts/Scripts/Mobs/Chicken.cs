using UnityEngine;
using System.Collections;

/// <summary>
/// Chicken Class
/// </summary>
public class Chicken : Animal
{

    private enum ANIMATIONS
    {
        TAKE = 0,
        DEATH = 1,
        GET_HIT = 2,
        IDLE1 = 3,
        RUN = 4,
        RUN_ANGRY = 5,
        SIT_ON = 6,
        WALK = 7,
    }

    public override void SelfSetup()
    {
        _idleAnimation = (int)ANIMATIONS.IDLE1;
        _deadAnimIdx = (int)ANIMATIONS.DEATH;
        _attackAnimation = (int)ANIMATIONS.RUN_ANGRY;
        _runAnimation = (int)ANIMATIONS.RUN;
        _walkAnimation = (int)ANIMATIONS.WALK;
        _attackDistance = 0.8f;
    }

    public override void Setup() { }
    public override void UpdateAnimal()
    {
        if (_actualUpdateLogic != null) _actualUpdateLogic();
    }

    public override void MakeFuriousSound()
    {
        JukeBox.instance.LoadAndPlaySound("chicken", 0.5f);
    }
}
