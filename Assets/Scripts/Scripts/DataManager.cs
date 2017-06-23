using UnityEngine;
using System.Collections.Generic;
using Items;
using Quests;
using Utils;
using SimpleJSON;
using UnityEngine.SceneManagement;

/// <summary>
/// This class handles all the important that of the game
/// keeping it synchronized with the server side by using the API.
/// </summary>
public class DataManager : MonoBehaviour {

    public static DataManager instance;

    private string _nickName;
    public string NickName { get { return _nickName; } set { _nickName = value; } }

    private int _level;
    public int Level { get { return _level; } set { _level = value; } }

    private int _experience;
    public int Experience { get { return _experience; } set { _experience = value; } }

    private int _experienceToLevelUp;
    public int ExperienceToLevelUP { get { return _experienceToLevelUp; } set { _experienceToLevelUp = value; } }

    private int _gold;
    public int Gold { get { return _gold; } set { _gold = value; } }

    private int _diamonds;
    public int Diamonds { get { return _diamonds; } set { _diamonds = value; } }

    private PlayerWarehouse _playeraWarehouse;
    public PlayerWarehouse Warehouse { get { return _playeraWarehouse; } }

    private API _api;
    public API Api { get { return _api; } }

    private List<ConfiguredQuest> _configuredQuests;
    public List<ConfiguredQuest> ConfiguredQuests { get { return _configuredQuests; } }

    private ConfiguredQuest _actualQuest;
    public ConfiguredQuest ActualQuest { get { return _actualQuest; } }

    public delegate void Callback();

    void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            Item.CreateAndInitializeItemsDictionary();
            _playeraWarehouse = new PlayerWarehouse();
            _api = GetComponent<API>();
        }
    }

    void Start()
    {
        if (Constants.USE_FAKE_API)
        {
            _configuredQuests = new List<ConfiguredQuest>();
            _actualQuest = new ConfiguredQuest(3, 7, 12, 1, Constants.FRACTION_TYPE_PIE);
            _configuredQuests.Add(new ConfiguredQuest(5, 9, 12, 1, Constants.FRACTION_TYPE_RECTANGULAR));
            _configuredQuests.Add(new ConfiguredQuest(8, 11, 12, 1, Constants.FRACTION_TYPE_PIE));

            //Store the number of initial quests
            PlayerPrefs.SetInt(Constants.STORAGE_NUM_QUESTS, _configuredQuests.Count + 1);
            if (LevelController.instance != null)
            {
                LevelController.instance.RestartSpawnCounters();

                _playeraWarehouse = new PlayerWarehouse();
            }
        }

        //TODO: Initialzie player warehouse by using the API:
    }

    public void RequestGetInventory()
    {
        string url = Constants.getAPIURL(Constants.API_GET_INVENTORY);

        _api.sendGETRequest(url, API.AUTH_TYPE.HARD, SyncInventory);
    }

    private void SyncInventory(WWW www)
    {
        var json = JSON.Parse(www.text);
        var serverItems = json["inventory"].AsArray;
        foreach(JSONNode inventoryItem in serverItems)
        {
            int item_id = inventoryItem["item_id"].AsInt;
            int quantity = inventoryItem["quantity"].AsInt;
            _playeraWarehouse.AddItem(item_id, quantity, false, null);
        }
        var equippedItems = json["equipped_items"].AsArray;
        foreach(JSONNode equippedItem in equippedItems)
        {
            int itemID = equippedItem["item_id"].AsInt;
            int place = equippedItem["place"].AsInt;
            switch (place)
            {
                case Constants.PLAYER_EQUIPPED_POSITION_HEAD:
                    _playeraWarehouse.EquippedHeadID = itemID;
                    break;
                case Constants.PLAYER_EQUIPPED_POSITION_SKIN:
                    _playeraWarehouse.EquippedSkinID = itemID;
                    break;
                case Constants.PLAYER_EQUIPPED_POSITION_WEAPON:
                    _playeraWarehouse.EquippedWeaponID = itemID;
                    break;
            }
        }
        SceneManager.LoadScene(Constants.SCENE_NAME_SPLASH_SCENE);
    }



    public void ParseGameConfiguration(WWW www)
    {
        _configuredQuests = new List<ConfiguredQuest>();
        var json = JSON.Parse(www.text);
        var configuredQuests = json["quests_config"].AsArray;
        foreach (JSONNode configuredQuest in configuredQuests)
        {
            string solution = configuredQuest["solution"].ToString();
            solution = solution.Replace("\"", "");
            int numerator = int.Parse(solution.Split('/')[0].Trim()); ;
            int denominator = int.Parse(solution.Split('/')[1].Trim());
            int workingFraction = configuredQuest["working_fraction"].AsInt;
            int preferedFractionRepresentation = configuredQuest["frac_rep"].AsInt;
            int questConfID = configuredQuest["conf_id"].AsInt;


            ConfiguredQuest quest = new ConfiguredQuest(numerator, denominator, workingFraction, questConfID, preferedFractionRepresentation);
            _configuredQuests.Add(quest);
        }

        //Store the number of initial quests
        PlayerPrefs.SetInt(Constants.STORAGE_NUM_QUESTS, _configuredQuests.Count);
        //Store when the game has began
        PlayerPrefs.SetFloat(Constants.STORAGE_BEGUIN_TIME, Time.time);
        //Reset number of fails
        PlayerPrefs.SetInt(Constants.STORAGE_NUM_FAILS, 0);

        if (Constants.DEVELOPMENT)
        {
            Debug.Log("This are our configured quests:");

            foreach (ConfiguredQuest q in _configuredQuests)
            {
                Debug.Log(q);
            }
        }
        ChangeActualQuest();
        LoadingScreenManager.LoadScene(Constants.SCENE_NAME_TOWN);

    }

    public void ChangeActualQuest()
    {
        if (_configuredQuests.Count > 0)
        {
            _actualQuest = _configuredQuests[0];
            _configuredQuests.RemoveAt(0);
        }
        else
        {
            _actualQuest = null;
        }
    }

    public void UpdateItem(int item_id, int quantity, Callback callback)
    {
        if (Constants.USE_FAKE_API)
            return;

        Dictionary<string, string> parameters = new Dictionary<string, string>();
        parameters.Add("item_id", item_id.ToString());
        parameters.Add("quantity", quantity.ToString());

        //Send the post request
        DataManager.instance.Api.POST(Constants.getAPIURL(Constants.API_UPDATE_ITEM), parameters, API.AUTH_TYPE.HARD, null, null, null, null);

        if (callback != null) callback();
    }

    public void SyncEquippedItem(int itemID, int posion)
    {
        if (Constants.USE_FAKE_API)
            return;

        Dictionary<string, string> parameters = new Dictionary<string, string>();
        parameters.Add("item_id", itemID.ToString());
        parameters.Add("position", posion.ToString());

        //Send the post request
        DataManager.instance.Api.POST(Constants.getAPIURL(Constants.API_EQUIP_ITEM), parameters, API.AUTH_TYPE.HARD, null, null, null, null);
    }

    public void SyncExperience(int ammout)
    {
        if (Constants.USE_FAKE_API)
            return;

        Dictionary<string, string> parameters = new Dictionary<string, string>();
        parameters.Add("experience", ammout.ToString());

        //Send the post request
        DataManager.instance.Api.POST(Constants.getAPIURL(Constants.API_ADD_EXPERIENCE), parameters, API.AUTH_TYPE.HARD, null, null, null, null);
    }


    public void UpdateCurrencyRequest(GameController.CURRENCY_TYPE currency, int ammount)
    {
        if(Constants.USE_FAKE_API)
            return;

        Dictionary<string, string> parameters = new Dictionary<string, string>();
        parameters.Add("quantity", ammount.ToString());

        string url = "";

        //Send the post request
        switch (currency)
        {
            case GameController.CURRENCY_TYPE.GOLD:
                url = Constants.getAPIURL(Constants.API_UPDATE_GOLD);
                break;
            case GameController.CURRENCY_TYPE.DIAMOND:
                url = Constants.getAPIURL(Constants.API_UPDATE_DIAMONDS);
                break;
        }

        DataManager.instance.Api.POST(url, parameters, API.AUTH_TYPE.HARD, null, null, null, null);
    }

}
