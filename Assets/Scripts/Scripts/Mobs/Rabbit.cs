using UnityEngine;
using System.Collections;

/// <summary>
/// Rabbit class
/// </summary>
public class Rabbit : Animal
{

    private enum ANIMATIONS
    {
        TAKE = 0,
        DEATH = 1,
        ATTACK = 2,
        IDLE1 = 3,
        IDLE2 = 4,
        LiTTLE_MOVE = 5,
        RUN = 6,
    }

    public override void SelfSetup()
    {
        _deadAnimIdx = (int)ANIMATIONS.DEATH;
        _attackAnimation = (int)ANIMATIONS.ATTACK;
        _runAnimation = (int)ANIMATIONS.RUN;
        _walkAnimation = (int)ANIMATIONS.RUN;
        _attackDistance = 0.8f;
        }

    public override void Setup() { }
    public override void UpdateAnimal()
    {
        if (_actualUpdateLogic != null) _actualUpdateLogic();
    }

    public override void MakeFuriousSound()
    {
        JukeBox.instance.LoadAndPlaySound("mooo", 0.5f);
    }
}
