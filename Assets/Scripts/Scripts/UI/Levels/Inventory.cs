using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Items;
using Utils;
using System;

/// <summary>
/// Used to manage the visible part of the objects that the player has, allowing him to resort them...
/// </summary>
public class Inventory : UIElement
{
    [SerializeField]
    private int[] inventoryPositions;

    const int MAX_POTIONS = 99;

    public Text NumPotion;

    private GameObject[] _inventorySlots;
    public NotificationManager notificationManager;

    public Dictionary<int, InventoryItem> items;

    private GameObject _inventoryItemPrefab;

    [SerializeField]
    private bool _isFull = false;
    public bool isFull { get { return _isFull; } }

    private int _numItems = 0;
    public int numItems { get { return _numItems; } }

    public AudioClip itemPickup;

    int counter = 0;

    private InventoryItem _potion;

    private List<InventoryItem> _fractions;

    void Awake()
    {

        _fractions = new List<InventoryItem>();
        items = new Dictionary<int, InventoryItem>();
        _potion = new InventoryItem(Potion.HealthPotion, 10, -1);
        UpdateNumPotionsUI();

        //Get all the slots of the inventory
        var tmpSlots = GameObject.FindGameObjectsWithTag("InventorySlot");
        _inventorySlots = new GameObject[tmpSlots.Length];

        //We need to sort the slots in order to make them coincide with their position
        foreach(GameObject slot in tmpSlots)
        {
            _inventorySlots[slot.GetComponent<InventorySlot>().position] = slot;  
        }

        inventoryPositions = new int[_inventorySlots.Length];

        for (int i = 0; i < inventoryPositions.Length; i++)
        {
            inventoryPositions[i] = -1;
        }
        notificationManager = GameObject.Find("UI").GetComponent<NotificationManager>();

        _inventoryItemPrefab = (GameObject)Resources.Load(Constants.PATH_RESOURCES + "UI/InventoryItem", typeof(GameObject));
       
    }

    public void updateInventory(Dictionary<int, InventoryItem> items)
    {
        //Destroy all the inventory item representation before beguinning
        GameObject[] itemsToDestroy = GameObject.FindGameObjectsWithTag("InventoryItem");
        foreach (GameObject item in itemsToDestroy)
        {
            Destroy(item);
        } 

        foreach(InventoryItem item in items.Values)
        {
            Transform itemInSlot = _inventorySlots[item.position].transform.Find("InventoryItem(Clone)");
            GameObject newInventoryItem;

            newInventoryItem = GameObject.Instantiate(_inventoryItemPrefab);
            newInventoryItem.transform.SetParent(_inventorySlots[item.position].transform);
            newInventoryItem.transform.position = new Vector3(0, 0, 1);            

            if(item.item.ID == Constants.ID_FRACTION)
            {
                newInventoryItem.transform.Find("Quantity").GetComponent<Text>().text = "";
                newInventoryItem.transform.Find("Top").GetComponent<Text>().text = ((Fraction)item.item).top.ToString();
                newInventoryItem.transform.Find("Down").GetComponent<Text>().text = ((Fraction)item.item).down.ToString();
            }
            else
            {
                newInventoryItem.transform.Find("Quantity").GetComponent<Text>().text = item.quantity.ToString();
                newInventoryItem.transform.Find("Top").GetComponent<Text>().text = "";
                newInventoryItem.transform.Find("Down").GetComponent<Text>().text = "";
            }
            newInventoryItem.GetComponent<RawImage>().texture = item.item.InventoryRepresentation.texture;
            Draggable d = newInventoryItem.GetComponent<Draggable>();
            d.parentToREturnTo = itemInSlot;
            d.item = item.item;   
        }

    }

    public void addItem(Item item, int quantity)
    {
        JukeBox.instance.Play(itemPickup, 0.6f);
        if (item.ID == Constants.ID_FRACTION)
        {
            Fraction frac = (Fraction)item;
            if (!items.ContainsKey(frac.fracIdentifier))
            {
                int itemPosition = getFreePosition(frac.fracIdentifier);
                if (itemPosition != -1)
                {
                    InventoryItem newItem = new InventoryItem(item, quantity, itemPosition);
                    items.Add(frac.fracIdentifier, newItem);
                    _fractions.Add(newItem);
                }
                else
                {
                    fullInventoryMessage();
                    return;
                }
            }
            notificationManager.addNotification(item.InventoryRepresentation, "New Fraction Avaliable", "Congratulations! It seems that you have a new superpower!!", 5f, frac.top, frac.down);
        }
        else
        {
            if (items.ContainsKey(item.ID))
            {
                items[item.ID].quantity += quantity;

            }
            else
            {
                int itemPosition = getFreePosition(item.ID);
                if (itemPosition != -1)
                {
                    InventoryItem newItem = new InventoryItem(item, quantity, itemPosition);
                    items.Add(item.ID, newItem);
                }
                else
                {
                    fullInventoryMessage();
                    return;
                }

            }

            GUIController.instance.CreateFloatingText("+" + quantity.ToString() + " " + item.Name, LevelController.instance.Hero.transform, Color.cyan);
        }

        //If the inventory is enabled we need to actualize it
        if (this.enabled)
        {
            updateInventory(items);
        }
    }

    public void UsePotion()
    {
        if(_potion.quantity > 0)
        {
            ((Potion)_potion.item).Use();
            --_potion.quantity;
            UpdateNumPotionsUI();
        }
    }

    public void AddPotions(int numPotions)
    {
        GUIController.instance.CreateFloatingText("+" + numPotions.ToString() + " Potion", transform, Color.magenta);
        _potion.quantity = _potion.quantity + numPotions > MAX_POTIONS ? MAX_POTIONS : _potion.quantity + numPotions;
        _potion.quantity += numPotions;
        UpdateNumPotionsUI();
    }

    public void UpdateNumPotionsUI()
    {
        NumPotion.text = _potion.quantity.ToString();
    }

    private void fullInventoryMessage()
    {
        notificationManager.addNotification(NotificationManager.NotificationType.ALERT, "Full Inventory", "Your inventory is full :(", 5f);
    }

    private int getFreePosition(int itemID)
    {
        bool found = false;
        int i = 0;
        while (!_isFull && !found && i < inventoryPositions.Length)
        {
            if (inventoryPositions[i] == -1)
            {
                inventoryPositions[i] = itemID;
                _numItems++;
                if(_numItems >= inventoryPositions.Length)
                {
                    _isFull = true;
                }
                return i;
            }
            i++;
        }
        return -1;
    }

    public void inventoryBtnAction()
    {
        if (isVisible)
        {
            hide();
        }
        else
        {
            show();
        }
    }

    /// <summary>
    /// Changes the position of gameobjects on the inventory by it's indexes
    /// </summary>
    /// <param name="lastPosition"></param>
    /// <param name="newPostion"></param>
    public void swapPosition(int lastPosition, int newPosition)
    {
        int temp = inventoryPositions[lastPosition];
        inventoryPositions[lastPosition] = inventoryPositions[newPosition];
        inventoryPositions[newPosition] = temp;
        if(inventoryPositions[newPosition] != -1) items[inventoryPositions[newPosition]].position = newPosition;
        //If there is a game object there i need to move it to another part
    }

    public Item getItemAt(int position)
    {
        if (position >= inventoryPositions.Length) return null;
        try
        {
            if (inventoryPositions[position] != -1)
            {
                return items[inventoryPositions[position]].item;
            }
        }catch(IndexOutOfRangeException ex)
        {
            return null;
        }

        return null;
    }

    public void RemoveItem(Item item, int quantity)
    {
        if(items[item.ID].quantity > quantity)
        {
            items[item.ID].quantity -= quantity;
        }
        else
        {
            items.Remove(item.ID);
            RemoveItemFromPosition(item);
        }
        updateInventory(items);
    }

    private void RemoveItemFromPosition(Item item)
    {
        bool found = false;
        int i = 0;
        while(!found && i < inventoryPositions.Length)
        {
            if(inventoryPositions[i] == item.ID)
            {
                inventoryPositions[i] = -1;
                found = true;
            }
            i++;
        }
    }

    public bool hasEnoughtOfItem(Item requiredItem, int quantity)
    {
        try
        {
            bool enought = items[requiredItem.ID].quantity >= quantity;
            return enought;
        }
        catch(Exception ex)
        {
            return false;
        }
    }

    public int GetItemQuantity(Item item)
    {
        if (items.ContainsKey(item.ID))
        {
            return items[item.ID].quantity;
        }
        return 0;
    }

	public bool hasItem(int itemID){
		try
		{
			return items[itemID].quantity > 0;;
		}
		catch(Exception ex)
		{
			return false;
		}
	}

    /// <summary>
    /// Removes all the fractions of the inventory
    /// </summary>
    public void ClearAllFractions()
    {
        items.Clear();
        for(int i = 0; i < inventoryPositions.Length; i++)
        {
            inventoryPositions[i] = -1;
        }
        updateInventory(items);
    }
}


