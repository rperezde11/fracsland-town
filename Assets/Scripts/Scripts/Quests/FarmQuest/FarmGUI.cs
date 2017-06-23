using UnityEngine.UI;

/// <summary>
/// Used to manage the Farm User interface allowing player to
/// plant a vegetables in farmparcels.
/// </summary>
public class FarmGUI : UIElement {

    public static FarmGUI instance = null;
    private FarmParcel _actualPlantContainer;

    public Button StrawBtn, PumpkingBtn, RedMushBtn, BlueMushBtn, CarrotBtn;
    public Image StrawImg, PumpkingImg, RedMushImg, BlueMushImg, CarrotImg;
    public Text StrawTxt, PumpkingTxt, RedMushTxt, BlueMushTxt, CarrotTxt;

    void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    } 


	void Start () {
        hide();
	}
	
    public void ShowGUI(FarmParcel container)
    {
        _actualPlantContainer = container;
        show();
    }

    public void PlantStraw()
    {
        _actualPlantContainer.PlantStraw();
        hide();
    }

    public void PlantRedMushroom()
    {
        _actualPlantContainer.PlantRedMushroom();
        hide();
    }

    public void PlantBlueMushroom()
    {
        _actualPlantContainer.PlantBlueMushroom();
        hide();
    }

    public void PlantCarrots()
    {
        _actualPlantContainer.PlantCarrots();
        hide();
    }


    public void PlantPumkin()
    {
        _actualPlantContainer.PlantPumkin();
        hide();
    }

    public void EnableAllButtons()
    {
        BlueMushBtn.gameObject.active = true;
        BlueMushImg.enabled = true;
        BlueMushTxt.enabled = true;

        CarrotBtn.gameObject.active = true;
        CarrotImg.enabled = true;
        CarrotTxt.enabled = true;

        PumpkingBtn.gameObject.active = true;
        PumpkingImg.enabled = true;
        PumpkingTxt.enabled = true;

        RedMushBtn.gameObject.active = true;
        RedMushImg.enabled = true;
        RedMushTxt.enabled = true;

        StrawBtn.gameObject.active = true;
        StrawImg.enabled = true;
        StrawTxt.enabled = true;
    }

    public void DisableButton(Vegetable.PLANT_TYPE type)
    {
        switch (type)
        {
            case Vegetable.PLANT_TYPE.BLUE_MUSHROOM:
                BlueMushBtn.gameObject.active = false;
                BlueMushImg.enabled = false;
                BlueMushTxt.enabled = false;
                break;
            case Vegetable.PLANT_TYPE.CARROTS:
                CarrotBtn.gameObject.active = false;
                CarrotImg.enabled = false;
                CarrotTxt.enabled = false;
                break;
            case Vegetable.PLANT_TYPE.PUMPKIN:
                PumpkingBtn.gameObject.active = false;
                PumpkingImg.enabled = false;
                PumpkingTxt.enabled = false;
                break;
            case Vegetable.PLANT_TYPE.RED_MUSHROOM:
                RedMushBtn.gameObject.active = false;
                RedMushImg.enabled = false;
                RedMushTxt.enabled = false;
                break;
            case Vegetable.PLANT_TYPE.STRAW:
                StrawBtn.gameObject.active = false;
                StrawImg.enabled = false;
                StrawTxt.enabled = false;
                break;
        }
    }

}
