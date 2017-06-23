using UnityEngine;
using Items;

namespace Quests.GameEvents
{
    /// <summary>
    /// Fired when the player plants a vegetable
    /// </summary>
    class FarmEvent : GameEvent
    {
        FarmParcel container;

        public FarmEvent(GameObject sender, QuestManager questManager) : base(sender, questManager)
        {
            container = sender.GetComponent<FarmParcel>();

        }

        public override void OnFired()
        {
            if(container.PlantState == FarmParcel.PLANT_STATE.EMPTY)
            {
                FarmGUI.instance.ShowGUI(sender.GetComponent<FarmParcel>());
            }

            else if(container.PlantState == FarmParcel.PLANT_STATE.DONE)
            {
                container.CollectPlant();
                GameController.instance.GiveRandomFraction();
                GameController.instance.GiveRandomMoney();
            }
        }
    }
}
