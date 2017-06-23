using UnityEngine;
using System;
using Items;
using Utils;

namespace Quests.GameEvents
{
    /// <summary>
    /// FIred when the player clicks over an altair giving him some fractions
    /// </summary>
	class FractionAltairEvent : GameEvent
	{

		public FractionAltairEvent(GameObject sender, QuestManager questManager) : base(sender, questManager)
		{

		}

		public override void OnFired()
		{
			FractionContainer container = sender.transform.GetComponent<FractionContainer> ();
			container.GiveFractions ();
		}
	}
}
