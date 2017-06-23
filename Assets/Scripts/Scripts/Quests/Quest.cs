using UnityEngine;
using Items;
using System.Collections.Generic;
using Utils;

/// <summary>
/// The base class of a quest
/// </summary>
public abstract class Quest : MonoBehaviour
{
    public struct RequiredItem
    {
        public Item item;
        public int quantity;
        public GameObject UIRepresentation;

        public RequiredItem(Item i, int q, GameObject rep)
        {
            item = i;
            quantity = q;
            UIRepresentation = rep;
        }
    }

    public ConfiguredQuest Configuration;
    public bool isPlayerSolvingTheQuest;
    protected Material notEnoughtMaterial, originalMaterial;
    protected List<RequiredItem> requiredItems;
	protected AudioClip successSound;
    public int ID = 0;
    protected bool _isDone = false;

    public string Description;
    public UIGoal.GOALTYPE QuestType;

    public abstract void Configure(ConfiguredQuest configuration);
    public abstract void Setup();
    public abstract void Interact();
    public abstract void Fail();
    public abstract void Success();
    public abstract void Restart();
    public abstract void Solve(Fraction fraction);
    
    protected bool _hasBeenSolved = true;

    void Awake()
    {
        requiredItems = new List<RequiredItem>();
    }

    void Start()
    {
        Setup();
    }

    void Update()
    {
        Interact();
    }


	public void SendTry(Fraction fraction, int levelID)
	{
		if (Constants.USE_FAKE_API)
			return;
		
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        parameters.Add("top", fraction.top.ToString());
        parameters.Add("down", fraction.down.ToString());
        parameters.Add("conf_id", levelID.ToString());
        
		//Send the post request
		DataManager.instance.Api.POST(Constants.getAPIURL(Constants.API_POST_TRY_SOLVE), parameters, API.AUTH_TYPE.HARD, null, null, null, null);
		//TODO: Make something here???

	}

    public void ShowQuestDetails()
    {
        GUIController.instance.ShowGoal(QuestType, Description, gameObject, this);
    }

    public void Save()
    {
        Utils.Utils.SaveQuestConfiguration(Configuration);
    }

    public void Load()
    {
        //HACK: At the moment this is not necessary but in a future it could be usefull to maintain quest state in a zone
        return;
        ConfiguredQuest data = Utils.Utils.LoadQuestConfiguration(Configuration);
        Configure(data);
    }
}

