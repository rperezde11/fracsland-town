using UnityEngine;
using UnityEngine.EventSystems;
using Quests;

/// <summary>
/// An slot where the player can have fractions, or items, in the future it will be cool
/// to implement the thrash in order to destroy game objects
/// </summary>
public class InventorySlot: MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public enum Slot { ITEM, Fraction, BOTH, TRASH };
    public int position = 0;
    public Slot slotTipe;

    public void OnDrop(PointerEventData eventData)
    {
        Draggable d = eventData.pointerDrag.GetComponent<Draggable>();

        int lastPosition = d.parentToREturnTo.GetComponent<InventorySlot>().position;

        //If we have one game object inside this position we need to swap it
        Debug.Log(transform.childCount);
        if (transform.childCount > 0)
        {
            Draggable dragable = transform.GetChild(0).GetComponent<Draggable>();
            dragable.parentToREturnTo = d.parentToREturnTo;
            dragable.transform.SetParent(d.parentToREturnTo);
        }

        QuestManager.instance.Inventory.swapPosition(lastPosition, position);
        d.parentToREturnTo = this.transform;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
    }

    public void OnPointerExit(PointerEventData eventData)
    {
    }
}
