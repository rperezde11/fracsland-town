using UnityEngine;

namespace Quests.GameEvents
{
    /// <summary>
    /// Fired when the player requests to cut a palm
    /// </summary>
    class PalmEvent : GameEvent
    {

        public PalmEvent(GameObject sender, QuestManager questManager) : base(sender, questManager)
        {

        }

        public override void OnFired()
        {
            if (isDone) return;
            if (LevelController.instance.Hero.getSelectedItemID() > 0)
            {
                LevelController.instance.Hero.stopMoving();
				LevelController.instance.Hero.canFireEvents = false;
                LevelController.instance.Hero.transform.LookAt(sender.transform.Find("CutPoint").transform.position);
                LevelController.instance.Hero.setNewDestinationPosition(sender.transform.position);
                LevelController.instance.Hero.Lumber();
                sender.GetComponent<Palm>().CutPalm();
				isDone = true;
            }
            else
            {
                NotificationManager.instance.addNotification(NotificationManager.NotificationType.MESSAGE, "Atxe required", "You need to equip your atxe (buy one in the village)!", 3f);
            }
        }
    }
}
