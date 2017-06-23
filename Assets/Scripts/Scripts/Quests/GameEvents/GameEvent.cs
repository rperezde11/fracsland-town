using System;
using UnityEngine;
using Quests;

namespace Quests.GameEvents
{
    /// <summary>
    /// The base class of every event that is important in the game
    /// </summary>
	public abstract class GameEvent
	{
        protected GameObject sender;
        protected string debugInfo;
        protected QuestManager questManager;
		protected bool isDone;

		public GameEvent (GameObject sender, QuestManager questManager)
		{
			this.sender = sender;
			this.questManager = questManager;
			this.isDone = false;
		}

        public abstract void OnFired();
	}
}

