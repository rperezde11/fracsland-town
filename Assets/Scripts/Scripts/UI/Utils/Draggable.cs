using UnityEngine;
using UnityEngine.EventSystems;
using Items;
using Utils;

/// <summary>
/// This class allows us to drag and drop fractions over the inventory or
/// over an interactable who accepts fractions.
/// </summary>
public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

    public Transform parentToREturnTo = null;
    public Item item { get { return _item; } set { _item = value; }}
    private Item _item;

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        parentToREturnTo = this.transform.parent;
        this.transform.SetParent(this.transform.parent.parent.parent);

        GetComponent<CanvasGroup>().blocksRaycasts = false;

        InventorySlot[] zonesToDrop = GameObject.FindObjectsOfType<InventorySlot>();
        //TODO: Make som king of visual feedback with this guys
    }

    public void OnDrag(PointerEventData eventData)
    {
        this.transform.position = eventData.position;
        LevelController.instance.Hero.stopMoving();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(_item.ID == Constants.ID_FRACTION && LevelController.instance.Hero.interactingItem != null)
        {
            LevelController.instance.Hero.useFraction((Fraction)_item);
        }

        this.transform.SetParent(parentToREturnTo);
        GetComponent<CanvasGroup>().blocksRaycasts = true;

        InventorySlot[] zonesToDrop = GameObject.FindObjectsOfType<InventorySlot>();
        //TODO: Make som king of visual feedback with this guys
        //Raycasts all what is under this object.
    }
}
