using UnityEngine;
using System.Collections;

/// <summary>
/// An NPC who is constructing something in fracstown
/// </summary>
public class Builder : HumanNPC
{
    const float soundRate = 1f;
    float _nextSound;
    float vol;
    const float MAX_VOL = 0.2f;
    public override void SelfSetup()
    {

    }

    public override void Setup()
    {

    }

    public override void UpdateHuman()
    {
        PlayAnimation(NPC_ANIMATION.BUILD_2);

        if(Time.time > _nextSound)
        {
            if (LevelController.instance.Hero == null) return;
            float distance = Vector3.Distance(LevelController.instance.Hero.transform.position, transform.position);
            vol = MAX_VOL / distance;
            vol = vol > MAX_VOL ? MAX_VOL : vol;
            // JukeBox.instance.LoadAndPlaySound("Metal", vol); // TODO: Uncomment
            _nextSound = Time.time + soundRate;
        }
    }

    protected override void Die()
    {

    }
}
