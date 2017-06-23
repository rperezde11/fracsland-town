using Items;
using System;
using Utils;
using Quests;

/// <summary>
/// A quest where you have to feed an animal
/// </summary>
public class AnimalQuest : Quest
{
    public enum ANIMAL_TYPE { COW, CHICKEN, RABBIT }

    public ANIMAL_TYPE AnimalType;
    public Vegetable.PLANT_TYPE Plant;
    public MinimapMarker selfMarker;

    public MinimapMarker farmMarker;
    private Interactable _interactable;

    private int _actualEventCallbackID;
    private ConfiguredQuest _questConf;

    private FarmParcel _parcel;

    private bool _hasCollectedVegetable = false;

    private Animal _animal;
    public Animal Animal { get { return _animal; } }

    private bool _hasBeenEnabled;

    /// <summary>
    /// Choose one random animal
    /// </summary>
    /// <param name="configuration"></param>
    public override void Configure(ConfiguredQuest configuration)
    {
        _hasBeenEnabled = true;
        _questConf = configuration;
        farmMarker.EnableMarker();
        GUIController.instance.ShowGoal(UIGoal.GOALTYPE.VIOLENT, Description, farmMarker.gameObject, this);

        SetAnimal();
        _actualEventCallbackID = QuestManager.instance.SubscribeIntoEvent(QuestManager.GameEventType.PLANT_EVENT, PlantPlanted);
        _interactable = GetComponent<Interactable>();
        _interactable.DisableAfterConfiguration();
        _animal.FollowHero();
        _hasBeenSolved = false;
    }

    private void SetAnimal()
    {
        switch (AnimalType)
        {
            case ANIMAL_TYPE.CHICKEN:
                _animal = GetComponent<Chicken>();
                break;
            case ANIMAL_TYPE.COW:
                _animal = GetComponent<Cow>();
                break;
            case ANIMAL_TYPE.RABBIT:
                _animal = GetComponent<Rabbit>();
                break;
        }
    }

    public override void Fail()
    {
        try
        {
            _animal.AttackPlayer();
        }
        catch (Exception)
        {
            SetAnimal();
            _animal.AttackPlayer();
        }
        
    }

    public override void Interact(){
        if (!_hasBeenEnabled)
        {
            selfMarker.DisableMarker();
            _interactable.enabled = true;
            farmMarker.DisableMarker();
            this.enabled = false;
        }

        if (_hasBeenSolved) return;
        GameController.instance.ConfiscateItems(Vegetable.GetItemByVegetableType(Plant));
    }

    public override void Restart()
    {
        throw new NotImplementedException();
    }

    public override void Setup()
    {
        _interactable = GetComponent<Interactable>();
    }

    public override void Solve(Fraction fraction)
    {
        if (_hasCollectedVegetable) return;
        if (!LevelController.instance.Hero.interactingItem.GetComponent<FarmParcel>().IsPartOfQuest) return;
        QuestManager.instance.UnsubscribeEnvent(_actualEventCallbackID, QuestManager.GameEventType.PLANT_EVENT);
        _parcel = LevelController.instance.Hero.interactingItem.GetComponent<FarmParcel>();
        if (fraction == DataManager.instance.ActualQuest.solution)
        {
            Success();
            QuestManager.instance.Inventory.RemoveItem(fraction, 1);
        } 
        else
        {
            Fail();
        }
        SendTry(fraction, DataManager.instance.ActualQuest.questConfigurationID);
    }

    /// <summary>
    /// Plant planted event
    /// </summary>
    /// <param name="args"></param>
    public void PlantPlanted(object[] args)
    {
        Vegetable.PLANT_TYPE type = (Vegetable.PLANT_TYPE)args[0];
        FarmParcel parcel = (FarmParcel)args[1];

        if(type == Plant)
        {
            GUIController.instance.ClearQuestGoals(this);
            FarmParcel.DestroyAllPlantsOfType(type);
            GUIController.instance.ShowGoal(UIGoal.GOALTYPE.VIOLENT, "Make the "+ Vegetable.GetItemByVegetableType(type).Name +" grow" , gameObject, this);
            FarmGUI.instance.DisableButton(type);
            parcel.SelectedPlant.FreezeGrowing();
            GameController.instance.ConfiscateItems(Vegetable.GetItemByVegetableType(type));
            parcel.SetAsPartOfQuest();
            //Generate the texture for the solution
            parcel.GetComponent<PieFraction>().GenerateTexture();
        }
    }


    public override void Success()
    {
        JukeBox.instance.LoadAndPlaySound("objective_done", 1);
        _hasBeenSolved = true;
        _parcel.CollectPlant(true);
        GUIController.instance.ClearQuestGoals(this);
        QuestManager.instance.SubscribeIntoEvent(QuestManager.GameEventType.ANIMAL_FEED_EVENT, PostSuccess);
        GUIController.instance.ShowGoal(UIGoal.GOALTYPE.VIOLENT, "Give me something to eat ;)", gameObject, this);
        selfMarker.EnableMarker();
        _hasCollectedVegetable = true;
        _parcel.UnsetAsPartOfQuest();
        _interactable.enabled = true;
        FarmGUI.instance.EnableAllButtons();
        GameController.instance.ReturnConfiscatedItems(Vegetable.GetItemByVegetableType(Plant));
    }

    /// <summary>
    /// Used if the player whants to give some food to some animal
    /// </summary>
    /// <param name="args"></param>
    public void PostSuccess(object[] args)
    {
        string animalName = (string)args[0];
        if (animalName == _animal.gameObject.name)
        {
            GUIController.instance.ClearQuestGoals(this);
            selfMarker.DisableMarker();
            QuestManager.instance.ChageActualQuestState(QuestManager.ActualQuestStates.PENDING_TO_REWARD);
            _isDone = true;
        }
            
    }
}
