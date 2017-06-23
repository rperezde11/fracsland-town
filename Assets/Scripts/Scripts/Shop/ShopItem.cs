using UnityEngine;
using UnityEngine.UI;
using Items;
using Utils;

/// <summary>
/// Used to display the details of a item in the shop.
/// </summary>
public class ShopItem : MonoBehaviour {

    public static Color BUTTON_NOT_ENOUGTH_MONEY_BACKGROUND = Color.red;
    public static Color BUTTON_HAS_ENOUGTH_MONEY_BACKGROUND = new Color32(22, 176, 49, 255);

    public static Color ALREADY_HAS_ITEM_BACKGROUND = new Color32(155, 227, 176, 100);
    public static Color NOT_ENOUGTH_MONEY_BACKGROUND = new Color32(255, 0, 0, 100);
    public static Color HAS_ENOUGTH_MONEY_BACKGROUND = new Color32(22, 176, 49, 100);

    public Shop.SHOP_CATEGORY _category;
    public int id;
    public Image texture;
    public Text Tittle;
    public Text description;
    public Text gold;
    public Text diamonds;
    public Button buyGold, buyDiamonds, equip;
    public bool owned;
    public Item item;
    public Image backgroundImage;
    public bool isSetUp = false;

    public void SetParameters(Item item, Shop.SHOP_CATEGORY cat)
    {
        _category = cat;
        texture.sprite = item.InventoryRepresentation;
        Tittle.text = item.Name;
        gold.text = item.GoldAmmount.ToString();
        diamonds.text = item.DiamondsAmmount.ToString();
        owned = false;
        this.item = item;

        owned = DataManager.instance.Warehouse.HasItem(item);

        if (!owned)
        {
            EquippableItem ei = null;
            UnityEngine.Events.UnityAction actionGold = null;
            UnityEngine.Events.UnityAction actionDiammonds = null;

            switch (_category)
            {
                case Shop.SHOP_CATEGORY.WEAPONS:
                    actionGold = () => {
                        if (Shop.instance.BuyItem(item, GameController.CURRENCY_TYPE.GOLD, true, () => { DataManager.instance.SyncEquippedItem(item.ID, Constants.PLAYER_EQUIPPED_POSITION_WEAPON); }))
                        {
                            StatsManager.instance.EquipItem((EquippableItem)item);
                            owned = true;
                            OwnedPresentation();
                        }
                    };
                    actionDiammonds = () => {
                        if (Shop.instance.BuyItem(item, GameController.CURRENCY_TYPE.DIAMOND, true, () => { DataManager.instance.SyncEquippedItem(item.ID, Constants.PLAYER_EQUIPPED_POSITION_WEAPON); }))
                        {
                            StatsManager.instance.EquipItem((EquippableItem)item);
                            owned = true;
                            OwnedPresentation();
                        }
                    };
                    ei = (EquippableItem)item;
                    description.text = "Attack Bonus: +" + ei.AttackBonus.ToString() + "\nSpeed Bonus: +" + ei.SpeedBonus.ToString() + "\nHealth Bonus: +" + ei.HealthBonus.ToString();
                    break;
                case Shop.SHOP_CATEGORY.HEAD:
                    actionGold = () => {
                        if (Shop.instance.BuyItem(item, GameController.CURRENCY_TYPE.GOLD, true, () => { DataManager.instance.SyncEquippedItem(item.ID, Constants.PLAYER_EQUIPPED_POSITION_HEAD); }))
                        {
                            StatsManager.instance.EquipHat((EquippableItem)item);
                            owned = true;
                            OwnedPresentation();
                        }
                    };
                    actionDiammonds = () => {
                        if (Shop.instance.BuyItem(item, GameController.CURRENCY_TYPE.DIAMOND, true, () => { DataManager.instance.SyncEquippedItem(item.ID, Constants.PLAYER_EQUIPPED_POSITION_HEAD); }))
                        {
                            StatsManager.instance.EquipHat((EquippableItem)item);
                            owned = true;
                            OwnedPresentation();
                        }
                    };
                    ei = (EquippableItem)item;
                    description.text = "Attack Bonus: +" + ei.AttackBonus.ToString() + "\nSpeed Bonus: +" + ei.SpeedBonus.ToString() + "\nHealth Bonus: +" + ei.HealthBonus.ToString();
                    break;
                case Shop.SHOP_CATEGORY.BODY:
                    actionGold = () => {
                        if (Shop.instance.BuyItem(item, GameController.CURRENCY_TYPE.GOLD, true, () => { DataManager.instance.SyncEquippedItem(item.ID, Constants.PLAYER_EQUIPPED_POSITION_SKIN); }))
                        {
                            StatsManager.instance.EquipSkin((Skin)item);
                            owned = true;
                            OwnedPresentation();
                        }
                    };
                    actionDiammonds = () => {
                        if (Shop.instance.BuyItem(item, GameController.CURRENCY_TYPE.DIAMOND, true, () => { DataManager.instance.SyncEquippedItem(item.ID, Constants.PLAYER_EQUIPPED_POSITION_SKIN); }))
                        {
                            StatsManager.instance.EquipSkin((Skin)item);
                            owned = true;
                            OwnedPresentation();
                        }
                    };
                    break;
                default:
                    break;
            }
            buyGold.onClick.AddListener(actionGold);
            buyDiamonds.onClick.AddListener(actionDiammonds);
        }
        else
        {
            OwnedPresentation();
        }
        isSetUp = true;
    }

    void Update()
    {
        if (!isSetUp) return;
        if (!owned)
        {
            bool hasMoney = false;
            if (GameController.instance.HasEnoughtMoney(item, GameController.CURRENCY_TYPE.GOLD))
            {
                buyGold.GetComponent<Image>().color = BUTTON_HAS_ENOUGTH_MONEY_BACKGROUND;
                hasMoney = true;
            }
            else
            {
                buyGold.GetComponent<Image>().color = BUTTON_NOT_ENOUGTH_MONEY_BACKGROUND;
            }

            if (GameController.instance.HasEnoughtMoney(item, GameController.CURRENCY_TYPE.DIAMOND))
            {
                buyDiamonds.GetComponent<Image>().color = BUTTON_HAS_ENOUGTH_MONEY_BACKGROUND;
                hasMoney = true;
            }
            else
            {
                buyDiamonds.GetComponent<Image>().color = BUTTON_NOT_ENOUGTH_MONEY_BACKGROUND;
            }
            if (hasMoney)
            {
                backgroundImage.color = HAS_ENOUGTH_MONEY_BACKGROUND;
            }
            else
            {
                backgroundImage.color = NOT_ENOUGTH_MONEY_BACKGROUND;
            }
        }
    }

    void OwnedPresentation()
    {
        backgroundImage.color = ALREADY_HAS_ITEM_BACKGROUND;
        buyDiamonds.GetComponent<UIElement>().hide();
        buyGold.GetComponent<UIElement>().hide();
        equip.GetComponent<UIElement>().show();

        UnityEngine.Events.UnityAction action = null;
        switch (_category)
        {
            case Shop.SHOP_CATEGORY.WEAPONS:
                action = () => {
                    StatsManager.instance.EquipItem((EquippableItem)item);
                    DataManager.instance.SyncEquippedItem(item.ID, Constants.PLAYER_EQUIPPED_POSITION_WEAPON);
                };
                break;
            case Shop.SHOP_CATEGORY.HEAD:
                action = () => {
                    StatsManager.instance.EquipHat((EquippableItem)item);
                    DataManager.instance.SyncEquippedItem(item.ID, Constants.PLAYER_EQUIPPED_POSITION_HEAD);
                };
                break;
            case Shop.SHOP_CATEGORY.BODY:
                action = () => {
                    StatsManager.instance.EquipSkin((Skin)item);
                    DataManager.instance.SyncEquippedItem(item.ID, Constants.PLAYER_EQUIPPED_POSITION_SKIN);
                };
                break;
            default:
                break;
        }
        equip.onClick.RemoveAllListeners();
        equip.onClick.AddListener(action);
    }
}
