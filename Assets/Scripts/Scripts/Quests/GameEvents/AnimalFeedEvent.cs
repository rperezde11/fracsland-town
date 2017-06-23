using UnityEngine;
using Quests;
using System.Collections.Generic;
using Items;

namespace Quests.GameEvents
{
    /// <summary>
    /// Fired when and animal is feeded by the player
    /// </summary>
    class AnimalFeedEvent : GameEvent
    {
        private AnimalQuest _animalQuest;

        public AnimalFeedEvent(GameObject sender, QuestManager questManager) : base(sender, questManager)
        {
            _animalQuest = sender.GetComponent<AnimalQuest>();
        }

        public override void OnFired()
        {
            Item necessaryVegetable = Vegetable.GetItemByVegetableType(_animalQuest.Plant);
            if (QuestManager.instance.Inventory.hasEnoughtOfItem(necessaryVegetable, 1))
            {
                QuestManager.instance.Inventory.RemoveItem(necessaryVegetable, 1);
                GameController.instance.AddMoney(Utils.Utils.GetStandartRewardForLevel(DataManager.instance.Level));
                if(_animalQuest != null)
                {
                    _animalQuest.Animal.ReturnToCage();
                }
                
            }

            // Fire all callbacks associated to this event
            Dictionary<int, QuestManager.EventCallback> callbacks = QuestManager.instance.GetSubscribersOfType(QuestManager.GameEventType.ANIMAL_FEED_EVENT);
            var args = new object[] { sender.name };

            foreach (KeyValuePair<int, QuestManager.EventCallback> callback in callbacks)
            {
                callback.Value(args);
            }
        }
    }
}
