using UnityEngine;
using System.Collections.Generic;

namespace Quests.GameEvents
{
    /// <summary>
    /// Fired when the player wants to place a vegetable in a farm parcel
    /// </summary>
    class PlantEvent : GameEvent
    {
        private FarmParcel _container;
        private Vegetable.PLANT_TYPE _vegetableType;

        public PlantEvent(GameObject sender, QuestManager questManager) : base(sender, questManager)
        {
            _container = sender.GetComponent<FarmParcel>();
            _vegetableType = _container.SelectedPlant.Type;
        }

        public override void OnFired()
        {
            Dictionary<int, QuestManager.EventCallback> callbacks = QuestManager.instance.GetSubscribersOfType(QuestManager.GameEventType.PLANT_EVENT);
            var args = new object[] { _vegetableType, _container };

            if (callbacks == null || callbacks.Count == 0) return;
            foreach (KeyValuePair<int, QuestManager.EventCallback> callback in callbacks)
            {
                //Fire all the callbacks
                try
                {
                    callback.Value(args);
                } catch (System.Exception ex){}
                
            }
        }
    }
}
