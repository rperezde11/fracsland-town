using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// USed to fire an action when the player hits the buttons of the shop (to buy or equip an item)
/// </summary>
public class ShopButton : MonoBehaviour {

    public Shop.SHOP_CATEGORY category;

    public void AddListners()
    {
        Button but = GetComponent<Button>();
        UnityEngine.Events.UnityAction action = () => { Shop.instance.RenderCategory(category); };
        but.onClick.AddListener(action);
    }

}
