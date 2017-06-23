using UnityEngine;
using System.Collections;
using Utils;
using System;
using Quests;

/// <summary>
/// This human it's not used rigth now but it's a perfect example 
/// of an enity who follows the player, explaining him something about
/// a level. (can be really usefull in tutorials)
/// </summary>
public class MrSmith : HumanNPC {
	GameObject altair;
	GameObject bridge;


	public override void  SelfSetup(){
		
	}

	public override void Setup(){
		PlayAnimation (NPC_ANIMATION.IDLE_1);
        _actualUpdateLogic = WelcomePlayer;
		altair = GameObject.Find ("altair");
		bridge = GameObject.Find ("Bridge");
	}

	public override void UpdateHuman(){
		if(_actualUpdateLogic != null) _actualUpdateLogic();
	}

	public void WelcomePlayer()
	{
        if (LevelController.instance.Hero == null) return;
		if (Vector3.Distance (LevelController.instance.Hero.transform.position, transform.position) < 2f) {
			PlayAnimation (NPC_ANIMATION.TALK);
			transform.LookAt (LevelController.instance.Hero.transform.position);
			JukeBox.instance.LoadAndPlaySound ("hello_man", 1);
			ConversationBubble.instance.ShowProgressiveText ("Hello, I'm Mr Smith and I'm lost, let me join you...", gameObject, .05f);
            _actualUpdateLogic = null;

			ScheduledTaskManager.SetTimeout (() => {
				ConversationBubble.instance.HideDialog();
                _actualUpdateLogic = FollowAndGuidePlayer;
			}, 6000);
		}
	}

	public void FollowAndGuidePlayer(){

		//Follow Player
		if (Vector3.Distance (LevelController.instance.Hero.transform.position, transform.position) < 1.5f) {
			PlayAnimation (NPC_ANIMATION.IDLE_1);
			_movementManager.StopMovement ();
		} else {
			PlayAnimation (NPC_ANIMATION.RUN);
			_movementManager.SetNewDestination (LevelController.instance.Hero.transform.position);
		}
		transform.LookAt (LevelController.instance.Hero.transform.position);
	}

    protected override void Die()
    {
        throw new NotImplementedException();
    }
}
