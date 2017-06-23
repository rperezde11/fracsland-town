using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Vendor Class
/// </summary>
public class Vendor : HumanNPC {

    public override void SelfSetup()
    {

    }

    public override void Setup()
    {

    }

    public override void UpdateHuman()
    {
        if (_actualUpdateLogic != null)
            _actualUpdateLogic();
    }

    protected override void Die()
    {

    }

    public void Waiting()
    {
        PlayAnimation(NPC_ANIMATION.IDLE_1);
    }

    public void Greetings()
    {
        JukeBox.instance.LoadAndPlaySound("hello_man", 1);
        ConversationBubble.instance.ShowProgressiveText("Hello, take a look, I have very good stuff...", gameObject, .05f);
        PlayAnimation(NPC_ANIMATION.TALK);
        ChangeUpdateMethod(null);
        ChangeUpdateMethodIn(2f, Waiting);
    }
}
