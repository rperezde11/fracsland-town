using UnityEngine;
using System.Collections.Generic;
using Quests;

/// <summary>
/// A place where the player can plant some vegetables
/// </summary>
public class FarmParcel : MonoBehaviour {

    public GameObject Straw, Pumpkin, RedMushRoom, BlueMushroom, Carrots;
    public enum PLANT_STATE { EMPTY, GROWING, DONE }
    private PLANT_STATE _actualPlantState = PLANT_STATE.EMPTY;
    public PLANT_STATE PlantState { get { return _actualPlantState; } }

    private bool _isPartOfQuest = false;
    public bool IsPartOfQuest { get { return _isPartOfQuest; } }

    public Sprite EmptySprite;
    private Interactable _interactable;
    private SphereCollider _collider;
    private ParticleSystem _particles;
    public int ID;

    //We need a way to return to the original sprite
    private Sprite _initialSprite;

    GameObject _selectedPlant;
    public Vegetable SelectedPlant { get{ return _selectedPlant.GetComponent<Vegetable>(); } }

    public Dictionary<Vegetable.PLANT_TYPE, int> _missionPlants;

    void Awake()
    {
        _missionPlants = new Dictionary<Vegetable.PLANT_TYPE, int>();
        _interactable = GetComponent<Interactable>();
        _collider = GetComponent<SphereCollider>();
        _particles = GetComponent<ParticleSystem>();
        _selectedPlant = Carrots;
        _initialSprite = GetComponent<SpriteRenderer>().sprite;
    }

    public void PlantStraw()
    {
        PlantSpecie(Vegetable.PLANT_TYPE.STRAW);
    }

    public void PlantRedMushroom()
    {
        PlantSpecie(Vegetable.PLANT_TYPE.RED_MUSHROOM);
    }

    public void PlantBlueMushroom()
    {
        PlantSpecie(Vegetable.PLANT_TYPE.BLUE_MUSHROOM);
    }

    public void PlantCarrots()
    {
        PlantSpecie(Vegetable.PLANT_TYPE.CARROTS);
    }


    public void PlantPumkin()
    {
        PlantSpecie(Vegetable.PLANT_TYPE.PUMPKIN);
    }

    private void PlantSpecie(Vegetable.PLANT_TYPE type)
    {
        if (_actualPlantState != PLANT_STATE.EMPTY) return;

        _selectedPlant = null;

        switch(type)
        {
            case Vegetable.PLANT_TYPE.STRAW:
                _selectedPlant = Straw;
                break;
            case Vegetable.PLANT_TYPE.PUMPKIN:
                _selectedPlant = Pumpkin;
                break;
            case Vegetable.PLANT_TYPE.RED_MUSHROOM:
                _selectedPlant = RedMushRoom;
                break;
            case Vegetable.PLANT_TYPE.BLUE_MUSHROOM:
                _selectedPlant = BlueMushroom;
                break;
            case Vegetable.PLANT_TYPE.CARROTS:
                _selectedPlant = Carrots;
                break;
            default:
                break;
        }
        _actualPlantState = PLANT_STATE.GROWING;
        _selectedPlant.SetActive(true);
        _selectedPlant.GetComponent<Vegetable>().SetGrowingState(Vegetable.GROWING_STATE.GROWING);

        //Create and fire the event
        QuestManager.instance.CreateGameEvent(QuestManager.GameEventType.PLANT_EVENT, gameObject).OnFired();
    }

    void Update()
    {
        if(_selectedPlant.GetComponent<Vegetable>().State == Vegetable.GROWING_STATE.DONE)
        {
            _particles.enableEmission = true;
            _actualPlantState = PLANT_STATE.DONE;
        }
        else
        {
            _particles.enableEmission = false;
        }

        Interactable interac = GetComponent<Interactable>();
        interac.name = _selectedPlant.GetComponent<Vegetable>().GetRemainingTime();
        if (interac.name == "") interac.SetNameToDefautl();
    }


    /// <summary>
    /// The next quantity of vegetabes of thw specie will need a fraction to grow
    /// </summary>
    /// <param name="specie"></param>
    /// <param name="quantity"></param>
    public void AddVegetablesForMission(Vegetable.PLANT_TYPE specie, int quantity)
    {
        if (_missionPlants.ContainsKey(specie))
        {
            _missionPlants.Add(specie, quantity);
        }
        else
        {
            _missionPlants[specie] += quantity;
        }
    }

    public void RemoveVegetablesForMission(Vegetable.PLANT_TYPE specie, int quantity)
    {
        if (!_missionPlants.ContainsKey(specie))
        {
            Debug.LogError("Plant no found!");
            return;
        }

        _missionPlants[specie] -= quantity;
        if(_missionPlants[specie] <= 0)
        {
            _missionPlants.Remove(specie);
        }
    }

    public void GrowPlant()
    {
        _selectedPlant.GetComponent<Vegetable>().Grow();
    }

    public void ReturnToInitialState()
    {
        GetComponent<SpriteRenderer>().sprite = _initialSprite;
        _selectedPlant.GetComponent<Vegetable>().SetGrowingState(Vegetable.GROWING_STATE.LIMBO);
        _actualPlantState = PLANT_STATE.EMPTY;
        _interactable.SetNameToDefautl();
    }

    public void CollectPlant(bool force=false)
    {
        Vegetable plant = _selectedPlant.GetComponent<Vegetable>();
        if (plant.State != Vegetable.GROWING_STATE.DONE && force == false) return;
        plant.Collect();
        ReturnToInitialState();      
    }

    public void SetAsPartOfQuest()
    {
        _isPartOfQuest = true;
        
    }

    public void UnsetAsPartOfQuest()
    {
        _isPartOfQuest = false;
    }

    public void DestroyPlant()
    {
        if (_actualPlantState != PLANT_STATE.EMPTY)
        {
            Vegetable plant = _selectedPlant.GetComponent<Vegetable>();
            plant.SetGrowingState(Vegetable.GROWING_STATE.LIMBO);
        }
    }

    /// <summary>
    /// Used to destroy all the vegetables that are already planted to force
    /// the student to use the quest
    /// </summary>
    /// <param name="type"></param>
    public static void DestroyAllPlantsOfType(Vegetable.PLANT_TYPE type)
    {
        GameObject[] parcels = GameObject.FindGameObjectsWithTag("FarmParcel");
        foreach(GameObject p in parcels)
        {
            FarmParcel parcel = p.GetComponent<FarmParcel>();
            if(parcel.SelectedPlant.Type == type)
            {
                parcel.DestroyPlant();
            }
        }
    }
}
