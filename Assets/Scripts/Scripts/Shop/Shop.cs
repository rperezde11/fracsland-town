using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Items;

/// <summary>
/// Used to allow the player to buy items.
/// </summary>
public class Shop : UIElement {

    public enum SHOP_CATEGORY { WEAPONS, HEAD, BODY, FRACTION, MISC }
    public static Shop instance = null;
    public GameObject shopItemPrefab;
    public Text numGold;
    public Text numDiamonds;
    public GameObject shopContents;
    private Dictionary<SHOP_CATEGORY, List<Item>> shopItems;
    private List<GameObject> _inListItems = null;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            shopItems = new Dictionary<SHOP_CATEGORY, List<Item>>();
            shopItems[SHOP_CATEGORY.WEAPONS] = new List<Item>() {
                EquippableItem.axe1,
                EquippableItem.axe2,
                EquippableItem.axe3,
                EquippableItem.axe4,
                EquippableItem.axe5,
                EquippableItem.axe6,
            };
            shopItems[SHOP_CATEGORY.HEAD] = new List<Item>() {
                EquippableItem.hat1,
                EquippableItem.hat2,
                EquippableItem.hat3,
                EquippableItem.hat4,
                EquippableItem.hat5,
                EquippableItem.hat6,
                EquippableItem.hat7,
            };
            shopItems[SHOP_CATEGORY.BODY] = new List<Item>() {
                Skin.skin1,
                Skin.skin2,
                Skin.skin3,
                Skin.skin4,
                Skin.skin5,
                Skin.skin6,
                Skin.skin7,
                Skin.skin8,
                Skin.skin9,
                Skin.skin10,
                Skin.skin11,
                Skin.skin12,
                Skin.skin13,
            };
            shopItems[SHOP_CATEGORY.MISC] = new List<Item>() { };
            foreach(GameObject but in GameObject.FindGameObjectsWithTag("ShopCategory"))
            {
                but.GetComponent<ShopButton>().AddListners();
            }
            _inListItems = new List<GameObject>();
        }
    }
	
    void Start()
    {
        RenderCategory(SHOP_CATEGORY.WEAPONS);
    }

    public void Update()
    {
        numDiamonds.text = DataManager.instance.Diamonds.ToString();
        numGold.text = DataManager.instance.Gold.ToString();
    }
    
    public void ClearCategoryList()
    {
        if (_inListItems.Count == 0) return;
        foreach(GameObject item in _inListItems)
        {
            Destroy(item);
        }
        _inListItems.Clear();
    }

    public void RenderCategory(SHOP_CATEGORY actualCategory)
    {
        ClearCategoryList();
        int itemCount = shopItems[actualCategory].Count;

        RectTransform rowRectTransform = shopItemPrefab.GetComponent<RectTransform>();
        RectTransform containerRectTransform = shopContents.GetComponent<RectTransform>();
        float height = rowRectTransform.rect.height;

        containerRectTransform.sizeDelta = new Vector2(containerRectTransform.offsetMax.x, itemCount * rowRectTransform.GetComponent<LayoutElement>().minHeight + shopContents.gameObject.GetComponent<VerticalLayoutGroup>().spacing * itemCount);

        for (int i = 0; i < itemCount; i++)
        {
            GameObject newItem = Instantiate(shopItemPrefab) as GameObject;
            ShopItem si = newItem.GetComponent<ShopItem>();
            si.SetParameters(shopItems[actualCategory][i], actualCategory);
            newItem.transform.SetParent(shopContents.gameObject.transform);
            _inListItems.Add(newItem);
        }
    }

    public bool BuyItem(Item item, GameController.CURRENCY_TYPE currency, bool syncWithServer, DataManager.Callback callback)
    {
        return GameController.instance.BuyRequest(item, 1, currency, syncWithServer, callback);
    }

}
