using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Quests;
using Utils;
using UnityEngine.SceneManagement;

/// <summary>
/// Handle all the behaivour of the GUI inside the game
/// </summary>
public class GUIController : MonoBehaviour {

    public GameObject chatPanel, menuPanel, inspector;
	public GameObject itemSelectedRect1, itemSelectedRect2, itemSelectedRect3, itemSelectedRect4, itemSelectedRect5, itemSelectedRect6;
    private int selectedItem = -1, selectedFraction = -1;
    public static GUIController instance;
    public Texture2D attackCursor, collectCursor, defaultCursor, questCursor, NPCCursor;

    public enum CURSOR_TYPE { CUSOR_ATTACK, CURSOR_COLLECT, CURSOR_DEFAULT, CURSOR_QUEST, CURSOR_NPC}
    private CursorMode _cursorMode = CursorMode.ForceSoftware;
    private Vector2 _cursorHotSpot = Vector2.zero;
    public Image HeroHealthIndicator;

    public FloatingText FloatingTextPrefab;
    public HealthBar HealthBarPrefab;

    private bool _onGUIOpened;
    public bool onGUIOpened { get { return _onGUIOpened; } }

    public GameObject GoalPanel, UIGoalPrefab;
    public MainQuestUI UIMainQuest;

    public MissionSelectorMenu MissionSelectorMenu;

    public List<GameObject> _showingGoals;

    public Text ZoneName;

    public Text VisibleGoldTxt, VisibleDiamondsTxt;
    public UIElement ZoneGoalsPannel;
    public Text ToggleGoalsText, TxtInspectedItem;

    public Text PlayerNickName;

    public void SetCursor(CURSOR_TYPE cursorType)
    {
        Texture2D cursorTexture = null;
        switch (cursorType)
        {
            case CURSOR_TYPE.CUSOR_ATTACK:
                cursorTexture = attackCursor;
                break;
            case CURSOR_TYPE.CURSOR_COLLECT:
                cursorTexture = collectCursor;
                break;
            case CURSOR_TYPE.CURSOR_DEFAULT:
                cursorTexture = defaultCursor;
                break;
            case CURSOR_TYPE.CURSOR_QUEST:
                cursorTexture = questCursor;
                break;
            case CURSOR_TYPE.CURSOR_NPC:
                cursorTexture = NPCCursor;
                break;
            default:
                cursorTexture = defaultCursor;
                Debug.Log("CursorError: This cursor is not defined");
                break;
        }
        Cursor.SetCursor(cursorTexture, _cursorHotSpot, _cursorMode);
    }

	void Awake ()
    {

        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            selectItem(1, false);
            _onGUIOpened = false;
            SetCursor(CURSOR_TYPE.CURSOR_DEFAULT);
            GameObject.DontDestroyOnLoad(gameObject);
            _showingGoals = new List<GameObject>();
        }
    }

    void Start()
    {
        hideAllMenus();

        //DataManager.instance.Api.createLoadingDialog(); // TODO: Uncomment

        UpdateGoldAndDiammonds();
        PlayerNickName.text = DataManager.instance.NickName;
    }

    void Update()
    {
        UpdateHeroHealthIndicator();
        handlePlayerInput();
    }

    private void UpdateHeroHealthIndicator()
    {
        HeroHealthIndicator.fillAmount = LevelController.instance.Hero.GetNormalizedHealth();
    }

    public void UsePotion()
    {
        QuestManager.instance.Inventory.UsePotion();
    }

    public void hideAllMenus()
    {
        QuestManager.instance.Chat.hide();
        QuestManager.instance.Inventory.hide();
        menuPanel.GetComponent<UIElement>().hide();
        Shop.instance.hide();
        StatsManager.instance.hide();
        _onGUIOpened = false;
    }

    public void OpenStatsManager()
    {
        StatsManager.instance.toggle();
        _onGUIOpened = StatsManager.instance.isVisible;
    }

    public void OpenShopUI()
    {
        Shop.instance.RenderCategory(Shop.SHOP_CATEGORY.WEAPONS);
        Shop.instance.toggle();
        StatsManager.instance.toggle();
        QuestManager.instance.Inventory.hide();
        _onGUIOpened = Shop.instance.isVisible;
    }

    private void handlePlayerInput()
    {

        if (Input.GetKeyDown(KeyCode.Q))
        {
            UsePotion();
        }

        if (Input.GetKeyDown(KeyCode.K) && !QuestManager.instance.Chat.isVisible)
        {
            StatsManager.instance.toggle();
            _onGUIOpened = !_onGUIOpened;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!onGUIOpened)
            {
                menuBtnAction();
            }
            else
            {
                hideAllMenus();
            }
            
        }

        if(Input.GetKeyDown(KeyCode.Tab))
        {
            chatBtnAction();
        }

        if(Input.GetKeyDown(KeyCode.I) && !QuestManager.instance.Chat.isVisible)
        {
            inventoryBtnAction();
            Shop.instance.hide();
            StatsManager.instance.hide();
        }
 
		if (Input.GetKeyDown(KeyCode.Alpha1) && !QuestManager.instance.Chat.isVisible) selectedItem = 1;
        if (Input.GetKeyDown(KeyCode.Alpha2) && !QuestManager.instance.Chat.isVisible) selectedItem = 2;
        if (Input.GetKeyDown(KeyCode.Alpha3) && !QuestManager.instance.Chat.isVisible) selectedItem = 3;
        if (Input.GetKeyDown(KeyCode.Alpha4) && !QuestManager.instance.Chat.isVisible) selectedItem = 4;
        if (Input.GetKeyDown(KeyCode.Alpha5) && !QuestManager.instance.Chat.isVisible) selectedItem = 5;
        if (Input.GetKeyDown(KeyCode.Alpha6) && !QuestManager.instance.Chat.isVisible) selectedItem = 6;

        if(selectedItem != -1)
	    	selectItem(selectedItem);
    }

    private void selectItem(int item, bool equip = true)
    {
        disableAllItemsSelection();
        switch (item)
        {
            case 1:
                itemSelectedRect1.GetComponent<UIToggleable>().show();
                break;
            case 2:
                itemSelectedRect2.GetComponent<UIToggleable>().show();
                break;
            case 3:
                itemSelectedRect3.GetComponent<UIToggleable>().show();
                break;
			case 4:
				itemSelectedRect4.GetComponent<UIToggleable>().show();
				break;
			case 5:
				itemSelectedRect5.GetComponent<UIToggleable>().show();
				break;
			case 6:
				itemSelectedRect6.GetComponent<UIToggleable>().show();
				break;
            default:
                break;
        }
        if (equip)
        {
            LevelController.instance.Hero.equipItem(QuestManager.instance.Inventory.getItemAt(item - 1));
        }
    }

    private void disableAllItemsSelection()
    {

        itemSelectedRect1.GetComponent<UIToggleable>().hide();
        itemSelectedRect2.GetComponent<UIToggleable>().hide();
        itemSelectedRect3.GetComponent<UIToggleable>().hide();
		itemSelectedRect4.GetComponent<UIToggleable>().hide();
		itemSelectedRect5.GetComponent<UIToggleable>().hide();
		itemSelectedRect6.GetComponent<UIToggleable>().hide();
    }
		
    public void chatBtnAction()
    {

        if (QuestManager.instance.Chat.isVisible)
        {
            QuestManager.instance.Chat.hide();
        }
        else
        {
            QuestManager.instance.Chat.show();
            GameObject.Find("ChatInput").GetComponent<InputField>().Select();
        }
        _onGUIOpened = QuestManager.instance.Chat.isVisible;
    }

    public void menuBtnAction()
    {
        QuestManager.instance.Chat.hide();
        UIElement element = menuPanel.GetComponent<UIElement>();
        if (element.isVisible){
            element.hide();
        }
        else
        {
            element.show();
        }
        _onGUIOpened = element.isVisible;
        
    }


    public void showInspectedItem(string title, GameObject inspected)
    {
        TxtInspectedItem.text = title;
        Inspector.instance.ShowInspector(inspected);
    }

    public void hideInspector()
    {
        Inspector.instance.hide();
    }

    
    public void inventoryBtnAction()
    {
        QuestManager.instance.Inventory.inventoryBtnAction();
        _onGUIOpened = QuestManager.instance.Inventory.isVisible;
    }

    public void changeLanguage(int lang)
    {
        GameController.instance.changeLanguage(lang);
    }

    public void CreateFloatingText(string text, Transform location, Color color)
    {
        FloatingText instance = Instantiate(FloatingTextPrefab);
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(location.position);


        instance.transform.SetParent(gameObject.transform, false);
        instance.transform.position = new Vector2(screenPosition.x + Random.Range(-.1f, .1f), screenPosition.y + Random.Range(-.1f, .1f));
        instance.SetText(text);
        instance.damageText.color = color;
    }

    public HealthBar CreateHealthBar(LivingBeing traget)
    {
        HealthBar instance = Instantiate(HealthBarPrefab);
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(traget.transform.position);


        instance.transform.SetParent(gameObject.transform, true);
        instance.transform.position = new Vector2(screenPosition.x, screenPosition.y);

        instance.Initialize(traget);

        return instance;
    }

    public void ShowGoal(UIGoal.GOALTYPE type, string title, GameObject target, Quest quest)
    {
        GameObject goal = Instantiate(UIGoalPrefab);
        goal.transform.SetParent(GoalPanel.transform, true);

        goal.GetComponent<UIGoal>().Setup(type, title, target, quest);
        IndicatorArrow.instance.SetTarget(target);
        _showingGoals.Add(goal);
    }

    public void ClearAllGoals()
    {
        List<GameObject> goalsToRemove = new List<GameObject>();
        foreach (GameObject goal in _showingGoals)
        {
            goalsToRemove.Add(goal);
        }
        DestroyGoals(goalsToRemove);
    }

    public void ClearQuestGoals(Quest quest)
    {
        List<GameObject> goalsToRemove = new List<GameObject>();
        foreach (GameObject goal in _showingGoals)
        {
            if(goal.GetComponent<UIGoal>().RelatedQuest == quest)
            {
                goalsToRemove.Add(goal);
            } 
        }
        DestroyGoals(goalsToRemove);
    }

    public void DestroyGoals(List<GameObject> goalsToDestroy)
    {
        foreach(GameObject goal in goalsToDestroy)
        {
            _showingGoals.Remove(goal);
            Destroy(goal);
        }
    }

    public void ShowZoneName()
    {
        string name = "";
        string sceneName = SceneManager.GetActiveScene().name;
        switch (sceneName)
        {
            case Constants.SCENE_NAME_BEACH:
                name = Constants.PLACE_BEACH;
                break;
            case Constants.SCENE_NAME_FARM:
                name = Constants.PLACE_FARM;
                FarmGUI.instance.EnableAllButtons();
                break;
            case Constants.SCENE_NAME_FOREST:
                name = Constants.PLACE_FOREST;
                break;
            case Constants.SCENE_NAME_TOWN:
                name = Constants.PLACE_TOWN;
                break;
            default:
                name = "Where are you??";
                break;
        }
        ZoneName.text = name;
        ToggleFade.instance.FadeWithTextAndColor(name, Color.gray, 2);
    }

    public void UpdateGoldAndDiammonds()
    {
        VisibleDiamondsTxt.text = DataManager.instance.Diamonds.ToString();
        VisibleGoldTxt.text = DataManager.instance.Gold.ToString();
    }

    public void ToggleUIGoals()
    {
        ZoneGoalsPannel.toggle();
        ToggleGoalsText.text = ZoneGoalsPannel.isVisible ? "Hide Goals" : "Show Goals";
    }
}
