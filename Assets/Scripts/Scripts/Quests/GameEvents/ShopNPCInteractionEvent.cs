using UnityEngine;

namespace Quests.GameEvents
{
    /// <summary>
    /// Fired when the player wants to show the GUI of the shop.
    /// </summary>
    class ShopNPCINteracionEvent : GameEvent
    {

        public ShopNPCINteracionEvent(GameObject sender, QuestManager questManager) : base(sender, questManager)
        {

        }

        public override void OnFired()
        {
            LevelController.instance.Hero.stopMoving();
            if (Shop.instance.isVisible) return;
            sender.GetComponent<Vendor>().Greetings();
            GUIController.instance.OpenShopUI();
        }
    }
}
