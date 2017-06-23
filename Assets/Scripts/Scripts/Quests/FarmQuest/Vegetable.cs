using UnityEngine;
using System.Collections.Generic;
using Utils;
using Items;
using Quests;

/// <summary>
/// Defines how the plant will grow 
/// </summary>
public struct GrowingBehaivourData
{
    public Vector3 InitialPosition;
    public Vector3 EndPosition;
    public Vector3 InitialScale;
    public Vector3 FinalScale;
    public float GrowingTime;

    public GrowingBehaivourData(float time, Vector3 iPos, Vector3 ePosition, Vector3 iScale, Vector3 fScale)
    {
        this.InitialPosition = iPos;
        this.GrowingTime = time * Constants.TO_MIN;
        this.EndPosition = ePosition;
        this.InitialScale = iScale;
        this.FinalScale = fScale;
    }
}


/// <summary>
/// Handles all the growing logic of a vegetable
/// </summary>
public class Vegetable : MonoBehaviour {

    public enum GROWING_STATE { LIMBO, GROWING, DONE }
    public enum PLANT_TYPE { STRAW, PUMPKIN, RED_MUSHROOM, BLUE_MUSHROOM, CARROTS }
    public delegate void ActualUpdate();

    private GROWING_STATE _actualGrowingState = GROWING_STATE.LIMBO;
    public GROWING_STATE State { get { return _actualGrowingState; } }
    ActualUpdate _actualUpdate;

    public PLANT_TYPE Type;
    [SerializeField]
    GrowingBehaivourData _growingBehaivour;

    private float _growingTimeControlVar = 0f;
    private bool _canGrow = true;

    public static Dictionary<PLANT_TYPE, GrowingBehaivourData> GrowingBehaivours = new Dictionary<PLANT_TYPE, GrowingBehaivourData>() {
        { PLANT_TYPE.STRAW, new GrowingBehaivourData( 1f, new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0.5f, 0.5f, 0.5f))},
        { PLANT_TYPE.PUMPKIN, new GrowingBehaivourData( 1f, new Vector3(0.179f, -0.045f, 0.0223f), new Vector3(0.179f, -0.045f, -0.177f), new Vector3(0, 0, 0), new Vector3(0.39f, 0.39f, 0.16f))},
        { PLANT_TYPE.RED_MUSHROOM, new GrowingBehaivourData( 1f, new Vector3(0, 0, 0.0072f), new Vector3(0, 0, 0), new Vector3(0.01f, 0.01f, 0.01f), new Vector3(1, 1, 1))},
        { PLANT_TYPE.BLUE_MUSHROOM, new GrowingBehaivourData( 1f, new Vector3(0, 0, 0.0086f), new Vector3(0, 0, -0.321f), new Vector3(0.1f, 0.1f, 0.1f), new Vector3(2.751797f, 2.751797f, 1.527535f))},
        { PLANT_TYPE.CARROTS, new GrowingBehaivourData( 1f, new Vector3(0, 0, -1.71f), new Vector3(0, 0, -1.94f), new Vector3(1, 1, 1), new Vector3(2f, 1f, 2f))}
    };

    private static Dictionary<PLANT_TYPE, Item> _itemByType = new Dictionary<PLANT_TYPE, Item>() {
        { PLANT_TYPE.STRAW, Item.Straw},
        { PLANT_TYPE.PUMPKIN, Item.Pumpkin},
        { PLANT_TYPE.RED_MUSHROOM, Item.RedMushrooms},
        { PLANT_TYPE.BLUE_MUSHROOM, Item.BlueMushroom},
        { PLANT_TYPE.CARROTS, Item.Carrot}
    };

    public void SetGrowingState(GROWING_STATE state)
    {
        _growingBehaivour = GrowingBehaivours[Type];
        _actualGrowingState = state;
        switch (state)
        {
            case GROWING_STATE.LIMBO:
                _actualUpdate = null;              
                gameObject.active = false;
                break;
            case GROWING_STATE.GROWING:
                SetGrowingParameters(_growingBehaivour.InitialScale, _growingBehaivour.InitialPosition);
                _growingTimeControlVar = 0;
                _actualUpdate = GrowingUpdate;
                _actualGrowingState = GROWING_STATE.GROWING;
                break;
            case GROWING_STATE.DONE:
                _actualUpdate = null;
                break;
            default:
                break;       
        }
    }
	
	void Update () {
        if(_actualUpdate != null) _actualUpdate();
	}

    void GrowingUpdate()
    {
        //If the plant has been frozen we need to pause it's growing behaivour
        if (!_canGrow) return;
        transform.localPosition = Vector3.Lerp(_growingBehaivour.InitialPosition, _growingBehaivour.EndPosition, _growingTimeControlVar);
        transform.localScale = Vector3.Lerp(_growingBehaivour.InitialScale, _growingBehaivour.FinalScale, _growingTimeControlVar);
   

        if(transform.localScale == _growingBehaivour.FinalScale)
        {
            _actualGrowingState = GROWING_STATE.DONE;
            _actualUpdate = null;
        }

        _growingTimeControlVar += Time.deltaTime / _growingBehaivour.GrowingTime;

    }

    public void SetGrowingParameters(Vector3 actualScale, Vector3 actualPosition)
    {
        transform.localScale = actualScale;
        transform.localPosition = actualPosition;
    }

    public string GetRemainingTime()
    {
        if (!_canGrow) return "Solve Me!";

        if (_actualGrowingState == GROWING_STATE.LIMBO)
        {
            return "";
        }

        float timer = _growingBehaivour.GrowingTime - _growingTimeControlVar * _growingBehaivour.GrowingTime;
        int minutes = Mathf.FloorToInt(timer / 60F);
        int seconds = Mathf.FloorToInt(timer - minutes * 60);
        if(minutes < 0 || seconds < 0)
        {
            return "Take Me! :)";
        }
        
        return transform.name + ": " + string.Format("{0:0}:{1:00}", minutes, seconds);
    }

    /// <summary>
    /// Disables the ability of this plant to grow
    /// </summary>
    public void FreezeGrowing()
    {
        Debug.Log("I have been frozen!!");
        _canGrow = false;
    }

    /// <summary>
    /// Resets the ability to grow of this plant
    /// </summary>
    public void UnFreezeGrowing()
    {
        _canGrow = true;
    }

    //The plant will grow
    public void Grow()
    {
        _growingTimeControlVar = 1;
        UnFreezeGrowing();
    }

    public void Collect()
    {
        Item item = null;
        switch (Type)
        {
            case PLANT_TYPE.STRAW:
                item = Item.Straw;
                break;
            case PLANT_TYPE.RED_MUSHROOM:
                item = Item.RedMushrooms;
                break;
            case PLANT_TYPE.PUMPKIN:
                item = Item.Pumpkin;
                break;
            case PLANT_TYPE.CARROTS:
                item = Item.Carrot;
                break;
            case PLANT_TYPE.BLUE_MUSHROOM:
                item = Item.BlueMushroom;
                break;
        }
        QuestManager.instance.Inventory.addItem(item, 1);
        _canGrow = true;
       
    }

    public static Item GetItemByVegetableType(PLANT_TYPE t)
    {
        return _itemByType[t];
    }
}
