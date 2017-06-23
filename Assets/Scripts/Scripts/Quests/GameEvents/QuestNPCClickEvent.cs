using UnityEngine;

namespace Quests.GameEvents
{
    /// <summary>
    /// Fired when the player wants to open the mission dialog in order to 
    /// select a new mission to do.
    /// </summary>
    class QuestNPCClickEvent : GameEvent
    {
        
        public QuestNPCClickEvent(GameObject sender, QuestManager questManager) : base(sender, questManager)
        {

        }

        public override void OnFired()
        {
            var actualQuestState = QuestManager.instance.ActualQuestState;
            
            if(actualQuestState == QuestManager.ActualQuestStates.PENDING_TO_REWARD)
            {
                QuestManager.instance.GiveRewardToPlayer();
                JukeBox.instance.LoadAndPlaySound("alright", 0.5f);
                QuestManager.instance.Inventory.ClearAllFractions();
                if (GameController.instance.HasQuestsTODO())
                {
                    DataManager.instance.ChangeActualQuest();
                    QuestManager.instance.ChageActualQuestState(QuestManager.ActualQuestStates.UNNASIGNED);
                }   
                else
                    GameController.instance.GoToWinScreen();
            }
            else if(actualQuestState == QuestManager.ActualQuestStates.UNNASIGNED)
            {
                GUIController.instance.MissionSelectorMenu.ShowAvaliableMissions();
                LevelController.instance.Hero.stopMoving();
            }
            
        }
    }
}
