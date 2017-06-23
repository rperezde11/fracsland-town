using System;
using System.Collections.Generic;
using Quests.GameEvents;
using UnityEngine;
using Utils;
using UnityEngine.SceneManagement;

namespace Quests
{
    /// <summary>
    /// Used to register and fire game events and track what is the quest state
    /// </summary>
	public class QuestManager : MonoBehaviour
	{

        public struct QuestWithReward
        {
            public QuestInformation QuestInfo;
            public int NumDiamonds;
            public int NumGold;
            public bool IsFeatured;
            public QuestWithReward(QuestInformation info, int numDiamonds, int numGold, bool isFeatured)
            {
                this.QuestInfo = info;
                this.NumDiamonds = numDiamonds;
                this.NumGold = numGold;
                this.IsFeatured = isFeatured;
            }
        }

        public static QuestManager instance;

        private Quest _actualQuest;
        public Quest ActualQuest { get { return _actualQuest; } }

        public enum GameEventType { NO_EVENT, PALM_EVENT, FRACTION_ALTAIR, SOLVE_EVENT, FARM_EVENT, PLANT_EVENT, ANIMAL_FEED_EVENT, OPEN_QUEST_DIALOG, OPEN_SHOP_EVENT }
        public enum ActualQuestStates { UNNASIGNED, DOING_QUEST, PENDING_TO_REWARD, ESCAPING_FROM_FRACSLAND }
        
        public delegate void EventCallback(params object[] arguments);
        public int ids = 0;  // This ids are used to recognize a subscriber in order to unsubscribe it
        private Dictionary<GameEventType, Dictionary<int, EventCallback>> _subscribers;

        private ActualQuestStates _actualQuestState = ActualQuestStates.UNNASIGNED;
        public ActualQuestStates ActualQuestState { get { return _actualQuestState; } }

        private QuestWithReward _actualQuestWithReward;
        public QuestWithReward ActualQuestWithReward { get { return _actualQuestWithReward; } }

        public Inventory Inventory;
        public Chat Chat;

        void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }
            else
            {     
                instance = this;
                _subscribers = new Dictionary<GameEventType, Dictionary<int, EventCallback>>();

                //Init all the info about the quests (this is only for client, no server involved here)
                QuestDetailArchive.Init();
            }

        }

        public GameEvent CreateGameEvent(GameEventType selectedGameEvent, GameObject sender, float minDistanceInteraction = Constants.HERO_INTERACTION_MIN_DISTANCE)
        {
            switch (selectedGameEvent)
            {
                case GameEventType.PALM_EVENT:
                    return new PalmEvent(sender, this);
                case GameEventType.FRACTION_ALTAIR:
                    return new FractionAltairEvent(sender, this);
                case GameEventType.FARM_EVENT:
                    return new FarmEvent(sender, this);
                case GameEventType.PLANT_EVENT:
                    return new PlantEvent(sender, this);
                case GameEventType.ANIMAL_FEED_EVENT:
                    return new AnimalFeedEvent(sender, this);
                case GameEventType.OPEN_QUEST_DIALOG:
                    return new QuestNPCClickEvent(sender, this);
                case GameEventType.OPEN_SHOP_EVENT:
                    return new ShopNPCINteracionEvent(sender, this);
            }
            return null;
        }

        public void Update()
        {

        }

        public void GuidePlayerToTheQuestManagerNPC(bool isRewarding = false)
        {
            if (IsPlayerInScene(Constants.SCENE_NAME_TOWN))
            {
                GUIController.instance.ClearAllGoals();
                string message = isRewarding ? "Go with Lord Barus to receive your reward!" : "Go with Lord Barus to see if you can help him in somethig...";
                GUIController.instance.ShowGoal(UIGoal.GOALTYPE.VIOLENT, message, GameObject.Find("LordBarus"), null);
            }
            else
            {
                string message = isRewarding ? "Go with Lord Barus to receive your reward!" : "Go to the town to get some missions";
                GUIController.instance.ClearAllGoals();
                GUIController.instance.ShowGoal(UIGoal.GOALTYPE.VIOLENT, message, PortalManager.GetPortalBySceneName(Constants.SCENE_NAME_TOWN), null);
            }
        }

        public void GuidePlauerDuringTheQuest()
        {
            if (!IsPlayerInScene(ActualQuestWithReward.QuestInfo.Scene))
            {
                GUIController.instance.ClearQuestGoals(_actualQuest);
                GameObject destinationPortal = PortalManager.GetPortalBySceneName(_actualQuestWithReward.QuestInfo.Scene);
                if(destinationPortal == null)
                {
                    destinationPortal = PortalManager.GetPortalBySceneName(Constants.SCENE_NAME_TOWN);
                }
                GUIController.instance.ShowGoal(UIGoal.GOALTYPE.VIOLENT, "Go to the quest zone!", destinationPortal, _actualQuest);
            }
        }

        public static bool IsPlayerInScene(string sceneName)
        {
            return SceneManager.GetActiveScene().name == sceneName;
        }

        public List<QuestWithReward> ChooseRandomQuests()
        {
            var questsOfEachZone = QuestDetailArchive.ChooseOneQuestOfEachZone();
            var questsWithRewardToReturn = new List<QuestWithReward>();

            ConfiguredQuest actualQuestConfiguration = DataManager.instance.ActualQuest;

            foreach (QuestInformation qI in questsOfEachZone)
            {
                bool isFeatured = actualQuestConfiguration.PreferedFractionRepresentation == qI.FractionType;
                questsWithRewardToReturn.Add(new QuestWithReward(qI, CalcNumDiamods(isFeatured), CalcNumGold(isFeatured), isFeatured));       
            }
            return questsWithRewardToReturn;
        }

        public int CalcNumDiamods(bool isFeatured)
        {
            if (isFeatured)
            {
                return 3;
            }
            else
            {
                return 1;
            }
        }

        public int CalcNumGold(bool isFeatured)
        {
            if (isFeatured)
            {
                return 300;
            }
            else
            {
                return 100;
            }
        }

        public void ShowQuestDetails()
        {
            _actualQuest.ShowQuestDetails();
        }

        public void SaveActualQuest()
        {
            if(_actualQuest != null)
             _actualQuest.Save();
        }

        public void LoadActiveQuest()
        {
            _actualQuest.Load();
        }

        /// <summary>
        /// Subcribe a callback to game envent
        /// </summary>
        /// <param name="type"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public int SubscribeIntoEvent(GameEventType type, EventCallback callback)
        {
            int id = ids++;
            if(_subscribers.ContainsKey(type))
            {
                _subscribers[type].Add(id, callback);
            }
            else
            {
                _subscribers.Add(type, new Dictionary<int, EventCallback>());
                _subscribers[type].Add(id, callback);
            }
            return id;
        }

        /// <summary>
        /// Used to unsubscribe a callback
        /// </summary>
        /// <param name="eventID"></param>
        /// <param name="type"></param>
        public void UnsubscribeEnvent(int eventID, GameEventType type)
        {
            try
            {
                _subscribers[type].Remove(eventID);       
            }catch(KeyNotFoundException exception)
            {
                Debug.LogError("Event not found!");
            }
        }

        /// <summary>
        /// Gets all the subscribers of some game event
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Dictionary<int, EventCallback> GetSubscribersOfType(GameEventType type)
        {
            if(_subscribers.ContainsKey(type))
            {
                return _subscribers[type];
            }
            return null;
        }


        /// <summary>
        /// Give a reward when players goes to the NPC and clicks over him
        /// </summary>
        public void GiveRewardToPlayer()
        {
            if (_actualQuestState != ActualQuestStates.PENDING_TO_REWARD)
            {
                Debug.LogError("Trying to reward a player when he is not intendet to, please set the state to PENDING_TO_REWARD before calling this method.");
                return;
            }
            
            GameController.instance.AddDiammods(_actualQuestWithReward.NumDiamonds);
            GameController.instance.AddMoney(_actualQuestWithReward.NumGold);
            StatsManager.instance.AddExperience(Mathf.RoundToInt(DataManager.instance.ExperienceToLevelUP / UnityEngine.Random.Range(2f, 4f)));
            GUIController.instance.UIMainQuest.AddAddedPiece();
            _actualQuestState = ActualQuestStates.UNNASIGNED;
        }

        public void ChageActualQuestState(ActualQuestStates newState)
        {
            _actualQuestState = newState;
            switch (_actualQuestState)
            {
                case ActualQuestStates.DOING_QUEST:
                    GUIController.instance.ClearAllGoals();
                    break;
                case ActualQuestStates.PENDING_TO_REWARD:
                    GUIController.instance.ClearQuestGoals(_actualQuest);
                    GUIController.instance.ShowGoal(UIGoal.GOALTYPE.VIOLENT, "Go with Lord Barus to get your reward and a boat engine piece!", PortalManager.GetPortalBySceneName(Constants.SCENE_NAME_TOWN), null);
                    break;
                case ActualQuestStates.UNNASIGNED:
                    GUIController.instance.ClearQuestGoals(_actualQuest);
                    GUIController.instance.ShowGoal(UIGoal.GOALTYPE.VIOLENT, "Go with Lord Barus to see if you can help him in somethig...", gameObject, null);
                    break;
            }
            Debug.Log(newState);
            IndicatePlayerWhatToDo();
        }


        private void IndicatePlayerWhatToDo()
        {
            switch (_actualQuestState)
            {
                case ActualQuestStates.UNNASIGNED:
                    GuidePlayerToTheQuestManagerNPC();
                    break;
                case ActualQuestStates.DOING_QUEST:
                    GuidePlauerDuringTheQuest();
                    break;
                case ActualQuestStates.PENDING_TO_REWARD:
                    GuidePlayerToTheQuestManagerNPC(true);
                    break;
                case ActualQuestStates.ESCAPING_FROM_FRACSLAND:
                    break;
            }
        }

        public void AcceptQuest(QuestWithReward quest)
        {
            _actualQuestWithReward = quest;
            ChageActualQuestState(ActualQuestStates.DOING_QUEST);
        }

        public void QuestPreparations(int level)
        {
            IndicatePlayerWhatToDo();
            if (_actualQuestState == ActualQuestStates.PENDING_TO_REWARD) return;
            if(SceneManager.GetActiveScene().name == _actualQuestWithReward.QuestInfo.Scene)
            {
                GUIController.instance.ClearAllGoals();
                Quest quest = GetQuestByID(_actualQuestWithReward.QuestInfo.Id);
                quest.Configure(DataManager.instance.ActualQuest);
                _actualQuest = quest;
            }

        }

        public Quest GetQuestByID(int id)
        {
            var questGos = GameObject.FindGameObjectsWithTag("Quest");
            foreach (GameObject qGo in questGos)
            {
                Quest quest = qGo.GetComponent<Quest>();
                if (quest.ID == id)
                {
                    return quest;
                }
            }
            return null;
        }
    }
}

