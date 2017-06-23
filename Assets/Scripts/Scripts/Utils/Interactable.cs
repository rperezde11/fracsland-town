using UnityEngine;
using Utils;
using Quests.GameEvents;
using Quests;

/// <summary>
/// When this class is attached into a game object it becames interactable 
/// witch means that it will fire a game event when the player interacts with
/// it.
/// </summary>
public class Interactable : MonoBehaviour
{
    public string name;
    public GUIController.CURSOR_TYPE cursorType = GUIController.CURSOR_TYPE.CURSOR_DEFAULT;
    public QuestManager.GameEventType selectedGameEvent;
    QuestManager questManager;
    GameEvent gameEvent;
    private string _defaultName;//Used to restore the interactable name
    private Vector3 _lastHeroPosition;
    private bool _goingTotarget = false;
    private bool _eventDone = false;

    private bool _mouseOver = false;
    private bool _disableAfterConfigure = false;

    public float InteractionDistance = Constants.HERO_INTERACTION_MIN_DISTANCE;

    [SerializeField]
    public bool MouseOver { get { return _mouseOver; } }

	int _frameCounter = 0;

    // Use this for initialization
    void Start()
    {
        questManager = GameObject.Find("_SCRIPTS").GetComponent<QuestManager>();
        gameEvent = questManager.CreateGameEvent(selectedGameEvent, gameObject);
        _defaultName = name;
        //If it has been requested to disable after configure it is done here:
        this.enabled = !_disableAfterConfigure;
    }

    void Update()
    {
        if (_eventDone) return;
        if(_goingTotarget && Vector3.Distance(LevelController.instance.Hero.transform.position, _lastHeroPosition) < 0.01f 
            && Vector3.Distance(LevelController.instance.Hero.gameObject.transform.position, transform.position) < InteractionDistance)
        {
            _eventDone = true;
            if(gameEvent != null)
                gameEvent.OnFired();
        }
        _lastHeroPosition = LevelController.instance.Hero.transform.position; 
    }

    void OnMouseOver()
    {
        _mouseOver = true;
        GUIController.instance.showInspectedItem(name, gameObject);
        GUIController.instance.SetCursor(cursorType);
        if(!GUIController.instance.onGUIOpened && Vector3.Distance(this.transform.position, LevelController.instance.Hero.gameObject.transform.position) < InteractionDistance)
        {
            GUIController.instance.showInspectedItem(this.name, gameObject);
            LevelController.instance.Hero.interactingItem = this;
            _frameCounter = 0;
        }

        if(selectedGameEvent == QuestManager.GameEventType.SOLVE_EVENT || selectedGameEvent == QuestManager.GameEventType.FARM_EVENT)
        {
            GUIController.instance.showInspectedItem(this.name, gameObject);
            LevelController.instance.Hero.interactingItem = this;
        }
    }

    void OnMouseExit()
    {
        _mouseOver = false;
        GUIController.instance.SetCursor(GUIController.CURSOR_TYPE.CURSOR_DEFAULT);
        GUIController.instance.hideInspector();
        LevelController.instance.Hero.interactingItem = null;
    }

    void OnMouseDown()
    {

		if (!LevelController.instance.Hero.canFireEvents)
			return;
		
        if (!GUIController.instance.onGUIOpened && Vector3.Distance(this.transform.position, LevelController.instance.Hero.gameObject.transform.position) < InteractionDistance)
        {
            if(gameEvent != null)
			    gameEvent.OnFired();
        }
        else
        {
            LevelController.instance.Hero.setNewDestinationPosition(transform.position);
            _goingTotarget = true;
        }
    }

    public void Destroy(float time = 0)
    {
        Destroy(this.gameObject, time);
    }
    
    /// <summary>
    /// Sets the this interactable name to it's first name
    /// </summary>
    public void SetNameToDefautl()
    {
        name = _defaultName;
    }

    public void DisableAfterConfiguration()
    {
        _disableAfterConfigure = true;
    }
}