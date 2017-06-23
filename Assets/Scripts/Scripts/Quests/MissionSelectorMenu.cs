using UnityEngine;
using System.Collections.Generic;
using Quests;
using UnityEngine.UI;

/// <summary>
/// Used to allow the player to take a mission.
/// </summary>
public class MissionSelectorMenu : UIElement {

    private List<QuestManager.QuestWithReward> _ChoosenQuests;
    public GameObject FeaturedQuestUIRepresentationPrefab;
    public GameObject NormalQuestUIRepresentationPrefab;
    public Transform MissionPanelTransform;

    public void ShowAvaliableMissions()
    {
        LoadMissionsIntoUI();
        show();
    }

    public void DestroyLastQuests()
    {
        var objectsToDestroy = GameObject.FindGameObjectsWithTag("UIMission");
        for(int i = 0; i < objectsToDestroy.Length; i++)
        {
            Destroy(objectsToDestroy[i]);
        }
    }


    public void LoadMissionsIntoUI()
    {
        if (isVisible) return;
        DestroyLastQuests();
        _ChoosenQuests = QuestManager.instance.ChooseRandomQuests();
        foreach(var quest in _ChoosenQuests)
        {
            GameObject UiPrefab = quest.IsFeatured ? FeaturedQuestUIRepresentationPrefab : NormalQuestUIRepresentationPrefab;
            GameObject newQuestUi = Instantiate(UiPrefab) as GameObject;
            newQuestUi.GetComponent<UIMission>().Initialize(quest);
            newQuestUi.transform.SetParent(MissionPanelTransform);
        }

    }

}
