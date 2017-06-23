using System.Collections.Generic;
using Utils;


namespace Quests
{
    /// <summary>
    /// Used to store all the details about a quest in order to give this info the the players
    /// </summary>
    public struct QuestInformation
    {
        public int Id;
        public string Title;
        public string Description;
        public int FractionType;
        public string Scene;

        public QuestInformation(int id, string title, string description, int fractionType, string scene)
        {
            this.Id = id;
            this.Title = title;
            this.Description = description;
            this.FractionType = fractionType;
            this.Scene = scene;
        }
    }

    /// <summary>
    /// Stores all the details about quests giving to the programmer the option of getting this details
    /// by accessing into a dictioary, this class is uset too to initialize all the quests deitals
    /// </summary>
    public static class QuestDetailArchive
    {
        public static List<string> GameScenesWithQuestsNames = new List<string>() { Constants.SCENE_NAME_BEACH, Constants.SCENE_NAME_FARM, Constants.SCENE_NAME_FOREST };

        public static Dictionary<string, List<QuestInformation>> QuestInfoOfZones = new Dictionary<string, List<QuestInformation>>();

        public static void Init()
        {
            //Init the dictionary
            for(int i = 0; i < GameScenesWithQuestsNames.Count; i++)
                QuestInfoOfZones.Add(GameScenesWithQuestsNames[i], new List<QuestInformation>());
            
            //Breach
            QuestInfoOfZones[Constants.SCENE_NAME_BEACH].Add(new QuestInformation(Constants.MISSION_ID_BRIDGE, "Destroyed Bridge!", "Ohh no! Our bridge has been destroyed by a tsunami, we need to reconstruct it, please hep us! Be carefull with lions!", Constants.FRACTION_TYPE_RECTANGULAR, Constants.SCENE_NAME_BEACH));

            //Farm
            QuestInfoOfZones[Constants.SCENE_NAME_FARM].Add(new QuestInformation(Constants.MISSION_ID_FARM_RABBIT, "Starving Rabit", "Mr Rabbit is hungry! Please go and plant some carrots in order to feed him, he will be happy!!", Constants.FRACTION_TYPE_PIE, Constants.SCENE_NAME_FARM));
            QuestInfoOfZones[Constants.SCENE_NAME_FARM].Add(new QuestInformation(Constants.MISSION_ID_FARM_COW, "Royal Cow",  "Lady Cow wants something to it, could you please go to our farm and plant some straw, she would love it.", Constants.FRACTION_TYPE_PIE, Constants.SCENE_NAME_FARM));
            QuestInfoOfZones[Constants.SCENE_NAME_FARM].Add(new QuestInformation(Constants.MISSION_ID_FARM_CHICKEN, "Greedy Chicken!", "Dr Chicken has seen Lady Cow eating straw and he whants to eat some too, please go to our farm to get some, be carefull he is a bit nervous.", Constants.FRACTION_TYPE_PIE, Constants.SCENE_NAME_FARM));

            //Forest
            QuestInfoOfZones[Constants.SCENE_NAME_FOREST].Add(new QuestInformation(Constants.MISSION_ID_FOREST_CAPTAIN, "Dear Captain John...", "Captain John has returned! He and his gang are terrifying the frozen woods villagers please someone has to stop him!", Constants.FRACTION_TYPE_PIE, Constants.SCENE_NAME_FOREST));
        }

        public static List<QuestInformation> ChooseOneQuestOfEachZone()
        {
            List<QuestInformation> questsToReturn = new List<QuestInformation>();
            for(int i = 0; i < GameScenesWithQuestsNames.Count; i++)
            {
                var questsOfZone = QuestInfoOfZones[GameScenesWithQuestsNames[i]];
                int randomIndex = UnityEngine.Random.Range(0, questsOfZone.Count);
                questsToReturn.Add(questsOfZone[randomIndex]);
            }
            return questsToReturn;
        }

    }
}
