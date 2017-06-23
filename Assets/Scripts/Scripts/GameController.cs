using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using SimpleJSON;
using Utils;
using Items;
using Quests;
using System;

/// <summary>
/// Used to manage some important things on the game.
/// </summary>
public class GameController : MonoBehaviour {

    public enum CURRENCY_TYPE {GOLD, DIAMOND };
    
	public static GameController instance;

    public GameObject MouseOverObject;

    private bool _pendingTodestroy = false;
    private bool _hasConfiguredGame;

    private PlayerWarehouse _confiscatedItems;

    public GameObject[] ToDestroyGameObjectsWhenLeavingGame;

    /// <summary>
    /// Make Game controller don't be destroyed when changing scene
    /// </summary>
    void Awake()
    {
        DontDestroyOnLoad(this);

        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            _pendingTodestroy = true;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            _confiscatedItems = new PlayerWarehouse();
            _hasConfiguredGame = false;
        }
    }

    void Start()
    {
        GUIController.instance.UIMainQuest.ShowEnginePieces(PlayerPrefs.GetInt(Constants.STORAGE_NUM_QUESTS));
        QuestManager.instance.ChageActualQuestState(QuestManager.ActualQuestStates.UNNASIGNED);
        _hasConfiguredGame = true;
    }


	void Update () {

    }
   


    void OnLevelWasLoaded(int level)
    {
        //Dont do anithing if you are going to be destroyed on the next TICK
        if (_pendingTodestroy) return;

        if (QuestManager.IsPlayerInScene(Constants.SCENE_NAME_WIN) || QuestManager.IsPlayerInScene(Constants.SCENE_NAME_SPLASH_SCENE))
        {
            Destroy(gameObject);
        }

        LevelController.instance.SpamHero();

        // Make the Quest manager prepare the zone for the active quests
        QuestManager.instance.QuestPreparations(level);
        

    }

    public void changeLanguage(int lang)
    {
        LanguageManager.instance.ActualLang = lang;
        TextFactory.ChangeLanguage(lang);
    }

    public void DestroyInGameGameObjects()
    {
        for (int i = 0; i < ToDestroyGameObjectsWhenLeavingGame.Length; i++)
        {
            Destroy(ToDestroyGameObjectsWhenLeavingGame[i]);
        }
    }

	public void GoToWinScreen()
	{
        DestroyInGameGameObjects();
        LoadingScreenManager.LoadScene(Constants.SCENE_NAME_WIN);
	}

	public bool HasQuestsTODO()
	{
		if (DataManager.instance.ConfiguredQuests != null) {
			return DataManager.instance.ConfiguredQuests.Count > 0;
		}
		return false;

	}

    public void QuitGame()
    {
        Application.Quit();
    }

	public void GoToMainMenu()
	{
        DestroyInGameGameObjects();
        LoadingScreenManager.LoadScene(Constants.SCENE_NAME_SPLASH_SCENE);
	}


    //TODO: Add the ability to buy with more than one currency
    public bool BuyRequest(Item item, int quantity, CURRENCY_TYPE currency, bool syncWithServer, DataManager.Callback callback)
    {
        bool bougth = false;
        switch (currency)
        {
            case CURRENCY_TYPE.GOLD:
                if (DataManager.instance.Gold >= item.GoldAmmount * quantity)
                {
                    DataManager.instance.Gold -= item.GoldAmmount * quantity;
                    DataManager.instance.UpdateCurrencyRequest(currency, -item.GoldAmmount * quantity);
                    bougth = true;
                }
                break;
            case CURRENCY_TYPE.DIAMOND:
                if (DataManager.instance.Diamonds >= item.DiamondsAmmount * quantity)
                {
                    DataManager.instance.Diamonds -= item.DiamondsAmmount * quantity;
                    //Sync With the server
                    DataManager.instance.UpdateCurrencyRequest(currency, -item.DiamondsAmmount * quantity);
                    bougth = true;
                }
                break;
            default:
                Debug.Log("Unexpected currency");
                break;
        }
        if (bougth)
        {
            DataManager.instance.Warehouse.AddItem(item, quantity, syncWithServer, callback);
            JukeBox.instance.LoadAndPlaySound("shop", 0.6f);
            GUIController.instance.UpdateGoldAndDiammonds();
        }
        return bougth;
    }

    internal void GiveRandomMoney()
    {
        GameController.instance.AddMoney(Mathf.RoundToInt(Mathf.Pow(1.1f, DataManager.instance.Level)) + Mathf.Max(UnityEngine.Random.Range(0, DataManager.instance.Level), 3));
    }

    public bool HasEnoughtMoney(Item item, CURRENCY_TYPE currency)
    {
        switch (currency)
        {
            case CURRENCY_TYPE.GOLD:
                return DataManager.instance.Gold >= item.GoldAmmount;
            case CURRENCY_TYPE.DIAMOND:
                return DataManager.instance.Diamonds >= item.DiamondsAmmount;
        }
        return false;
    }

    public void AddMoney(int ammount)
    {
        DataManager.instance.UpdateCurrencyRequest(CURRENCY_TYPE.GOLD, ammount);
        DataManager.instance.Gold += ammount;
        GUIController.instance.CreateFloatingText("+" + ammount + " Gold", LevelController.instance.Hero.transform, Color.yellow);
        GUIController.instance.UpdateGoldAndDiammonds();
    }

    public void AddDiammods(int ammount)
    {
        DataManager.instance.UpdateCurrencyRequest(CURRENCY_TYPE.DIAMOND, ammount);
        DataManager.instance.Diamonds += ammount;
        GUIController.instance.CreateFloatingText("+" + ammount + " Diamond", LevelController.instance.Hero.transform, Color.blue);
        GUIController.instance.UpdateGoldAndDiammonds();
    }

    public void ConfiscateItems(Item item)
    {
        int quantity = QuestManager.instance.Inventory.GetItemQuantity(item);
        if (quantity > 0)
        {
            _confiscatedItems.AddItem(item, quantity, false, null);
            QuestManager.instance.Inventory.RemoveItem(item, quantity);
        }
    }

    public void ReturnConfiscatedItems(Item item)
    {
        if (_confiscatedItems.HasItem(item))
        {
            int quantity = _confiscatedItems.GetNumOfItem(item);
            _confiscatedItems.RemoveItemQuantity(item, quantity);
            QuestManager.instance.Inventory.addItem(item, quantity);
        }
    }

    public void GiveRandomFraction()
    {
        int numOptions = 3;
        int choosenIndex;
        Fraction.SimplifiedFraction solution = DataManager.instance.ActualQuest.solution;
        List<int> possibilities = new List<int>();
        for (int i = 0; i < solution.down; i++)
        {
            if (i == solution.top)
                continue;
            possibilities.Add(i);
        }

        var toGiveFractions = new List<Fraction>();
        if (solution.down < 3)
        {
            numOptions = solution.down - 1;
        }
        for (int i = 0; i < numOptions; i++)
        {
            choosenIndex = UnityEngine.Random.Range(0, possibilities.Count);
            toGiveFractions.Add(new Fraction(Constants.ID_FRACTION, "Fraction", "inventory_fraction", possibilities[choosenIndex], DataManager.instance.ActualQuest.solution.down));
            possibilities.RemoveAt(choosenIndex);
        }
        choosenIndex = UnityEngine.Random.Range(0, possibilities.Count);
        int multiplier = UnityEngine.Random.Range(1, Mathf.RoundToInt(DataManager.instance.Level / 5) + 1);
        multiplier = multiplier == 0 ? 1 : multiplier;
        QuestManager.instance.Inventory.addItem((new Fraction(Constants.ID_FRACTION, "Fraction", "inventory_fraction", possibilities[choosenIndex] * multiplier, DataManager.instance.ActualQuest.solution.down * multiplier)), 1);

    }
}
