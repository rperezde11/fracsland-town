using UnityEngine;
using Items;
using UnityEngine.UI;
using Utils;

/// <summary>
/// Used to manage all the stats of the hero according in what items he have 
/// and display this details on a GUI.
/// </summary>
public class StatsManager : UIElement {

    public Text TxtAttackForce, TxtSpeed, TxtMaxHealth, LevelTxt, TopLeftLevelTxt;
    public Image experienceBar;

    private GameObject _levelUpEffect;
    public ToggleFade fadeMessage;

    public static StatsManager instance;

    private EquippableItem _weapon = null, _head = null;
    private Skin _equipedSkin = null;
    public EquippableItem Weapon { get { return _weapon; } }
    public EquippableItem Head { get { return _head; } }

    private GameObject _headSlot, _weaponSlot;
    private GameObject _equippedHat, _equippedWeapon;
    private Material _bodyMaterial;

    private float _healthBonus = 0f;
    private float _attackBonus = 0f;
    private float _speedBonus = 0;

    public float HealthBonus { get { return _healthBonus; } }
    public float AttackBonus { get { return _attackBonus; } }
    public float SpeedBonus { get { return _speedBonus; } }

    bool hasBeenEnabled = false;

    void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            hasBeenEnabled = true;
        }
    }


    public void Setup()
    {
        _headSlot = GameObject.Find("HeroHeatSlot");
        _weaponSlot = GameObject.Find("HeroWeaponSlot");
        _bodyMaterial = GameObject.Find("HeroMesh").gameObject.GetComponent<Renderer>().material;
        _levelUpEffect = GameObject.Find("LevelUpEffect");
        _levelUpEffect = GameObject.Find("Hero(Clone)/LevelUpEffect");
        HideLevelUpEffect();

        UpdateLevelingUI();

        //We need to add again the equipped objects "at visual level" because the game object is destroyed in each sce
        StatsManager.instance.ReloadEquippedItems();

    }

    public void EquipItem(EquippableItem item)
    {
        if (item == null) return;
        Destroy(_equippedWeapon);
        GameObject prefab = Resources.Load(Constants.PATH_RESOURCES + item.PrefabName) as GameObject;
        GameObject obj = GameObject.Instantiate(prefab);
        obj.transform.parent = _weaponSlot.transform;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
        _equippedWeapon = obj;
        _weapon = item;
        DataManager.instance.Warehouse.EquippedWeaponID = item.ID;
        RecalcBonus();
    }

    public void EquipHat(EquippableItem item)
    {
        Destroy(_equippedHat);
        GameObject prefab = Resources.Load(Constants.PATH_RESOURCES + item.PrefabName) as GameObject;
        GameObject obj = GameObject.Instantiate(prefab);
        obj.transform.parent = _headSlot.transform;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
        _equippedHat = obj;
        DataManager.instance.Warehouse.EquippedHeadID = item.ID;
        _head = item;
        RecalcBonus();
    }

    public void EquipSkin(Skin skin)
    {
        _bodyMaterial.mainTexture = skin.Texture;
        _equipedSkin = skin;
        DataManager.instance.Warehouse.EquippedSkinID = skin.ID;
        RecalcBonus();
    }

    /// <summary>
    /// We need to equip our last objects to the new hero
    /// </summary>
    /// <param name="level"></param>
    public void ReloadEquippedItems()
    {
        if(DataManager.instance.Warehouse.EquippedSkinID != -1)
        {
            //EquipSkin((Skin)Item.GetItemByID(DataManager.instance.Warehouse.EquippedSkinID));
        }
        if (DataManager.instance.Warehouse.EquippedWeaponID != -1)
        {
            //EquipItem((EquippableItem)Item.GetItemByID(DataManager.instance.Warehouse.EquippedWeaponID));
        }
        if (DataManager.instance.Warehouse.EquippedHeadID != -1)
        {
            //EquipHat((EquippableItem)Item.GetItemByID(DataManager.instance.Warehouse.EquippedHeadID));
        }
    }

    private void RecalcBonus()
    {
        _healthBonus = _speedBonus = _attackBonus = 0;
        if(_weapon != null)
        {
            _healthBonus = _weapon.HealthBonus;
            _speedBonus = _weapon.SpeedBonus;
            _attackBonus = _weapon.AttackBonus;
        }
        if(_head != null)
        {
            _healthBonus += _head.HealthBonus;
            _speedBonus += _head.SpeedBonus;
            _attackBonus +=  _head.AttackBonus;
        }
    }

    void Update()
    {
        UpdateGUI();
    }

    void UpdateGUI()
    {
        TxtAttackForce.text = AttackBonus.ToString();
        TxtSpeed.text = SpeedBonus.ToString();
        TxtMaxHealth.text = HealthBonus.ToString();
        LevelTxt.text = DataManager.instance.Level.ToString();
    }

    public void AddExperience(int experience, bool show=true)
    {
        DataManager.instance.Experience += experience;
        CalcLevel(true);
        UpdateLevelingUI();
        if(GUIController.instance && show)
            GUIController.instance.CreateFloatingText("+" + experience.ToString() + "Exp", LevelController.instance.Hero.transform, Color.blue);
        DataManager.instance.SyncExperience(experience);
    }

    public void SetExperience(int numExperience)
    {
        DataManager.instance.Experience = numExperience;
        DataManager.instance.ExperienceToLevelUP = Constants.LEVELING_LEVEL1_EXPERIENCE;
        DataManager.instance.Level = 1;
        CalcLevel();
        UpdateLevelingUI();
    }

    void OnLevelWasLoaded(int level)
    {
        if (!hasBeenEnabled) return;
        _levelUpEffect = GameObject.Find("LevelUpEffect");
        experienceBar = GameObject.Find("ExperienceBar").GetComponent<Image>();
    }

    private void LevelUP(bool witVisualFeedback)
    {
        DataManager.instance.Experience -= DataManager.instance.ExperienceToLevelUP;
        DataManager.instance.ExperienceToLevelUP = Mathf.RoundToInt(1.10f * DataManager.instance.ExperienceToLevelUP);
        ++DataManager.instance.Level;
        if (witVisualFeedback)
        {
            _levelUpEffect = GameObject.Find("LevelUpEffect");
            _levelUpEffect.GetComponent<ParticleSystem>().Play();
            _levelUpEffect.GetComponent<Light>().enabled = true;
            JukeBox.instance.LoadAndPlaySound("level_up", 1);
            ToggleFade.instance.FadeWithTextAndColor("LEVEL UP", Color.yellow, 2, HideLevelUpEffect); 
        }
    }

    private void HideLevelUpEffect()
    {
        _levelUpEffect = GameObject.Find("LevelUpEffect");
        _levelUpEffect.GetComponent<ParticleSystem>().Stop();
        _levelUpEffect.GetComponent<Light>().enabled = false;
    }

    public void UpdateLevelingUI()
    {
        TopLeftLevelTxt.text = DataManager.instance.Level.ToString();
        LevelTxt.text = DataManager.instance.Level.ToString();
        experienceBar.fillAmount = GetNormalizedExperienceToLevelUP();
    }

    private void CalcLevel(bool withVisualFeedback = false)
    {
        while (DataManager.instance.Experience >= DataManager.instance.ExperienceToLevelUP && DataManager.instance.Level <= Constants.LEVELING_MAX_LEVEL)
        {
            LevelUP(withVisualFeedback);
        }
    }

    public float GetNormalizedExperienceToLevelUP()
    {
        return DataManager.instance.Experience / (float)DataManager.instance.ExperienceToLevelUP;
    }
}
