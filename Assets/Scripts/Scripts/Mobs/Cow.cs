using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Cow class
/// </summary>
public class Cow : Animal {

    private enum ANIMATIONS
    {
        TAKE = 0,
        DEATH = 1,
        GET_HIT = 2,
        IDLE1 = 3,
        IDLE2 = 4,
        IDLE3 = 5,
        PAWL = 6,
        PAWLR = 7,
        RAM_ATTACK = 8,
        RUN = 9,
        WALK = 10,
    }

    public override void SelfSetup()
    {
        _deadAnimIdx = (int)ANIMATIONS.DEATH;
        _attackAnimation = (int)ANIMATIONS.RAM_ATTACK;
        _runAnimation = (int)ANIMATIONS.RUN;
        _walkAnimation = (int)ANIMATIONS.WALK;    
    }

    public override void Setup() {}
    public override void UpdateAnimal() {
        if(_actualUpdateLogic != null) _actualUpdateLogic();
    }

    public override void MakeFuriousSound()
    {
        JukeBox.instance.LoadAndPlaySound("mooo", 0.5f);
    }
}
