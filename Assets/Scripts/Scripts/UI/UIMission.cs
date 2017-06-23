using UnityEngine;
using System.Collections;
using Quests;
using UnityEngine.UI;

/// <summary>
/// Used to show a mission inside the mission selection
/// menu.
/// </summary>
public class UIMission : MonoBehaviour {

    public Text Title;
    public Text Description;
    public Text GoldAmmount;
    public Text DiamondsAmmout;
    public Button AcceptButton;
    private QuestManager.QuestWithReward _quest;

    public void Initialize(QuestManager.QuestWithReward quest)
    {
        _quest = quest;
        Title.text = quest.QuestInfo.Title;
        Description.text = quest.QuestInfo.Description;
        GoldAmmount.text = quest.NumGold.ToString();
        DiamondsAmmout.text = quest.NumDiamonds.ToString();
        UnityEngine.Events.UnityAction callback = () => {
            QuestManager.instance.AcceptQuest(_quest);
            GUIController.instance.MissionSelectorMenu.hide();
            JukeBox.instance.LoadAndPlaySound("letsmove", 0.7f);
        };
        AcceptButton.onClick.AddListener(callback);
    }
}
