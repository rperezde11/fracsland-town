using System.Collections.Generic;
using Utils;

namespace Items
{
    /// <summary>
    /// Manages the permanent data of the player
    /// </summary>
    public class PlayerWarehouse
    {
        //Contains the id of the item and the quantity of a certain item
        private Dictionary<int, int> _warehouse;

        private int _equippedHeadID;
        public int EquippedHeadID { get { return _equippedHeadID; } set { _equippedHeadID = value; } }

        private int _equippedWeaponID;
        public int EquippedWeaponID { get { return _equippedWeaponID; } set { _equippedWeaponID = value; } }

        private int _equippedSkinID;
        public int EquippedSkinID { get { return _equippedSkinID; } set { _equippedSkinID = value; } }

        public PlayerWarehouse()
        {
            _warehouse = new Dictionary<int, int>();
        }

        public void AddItem(Item item, int quantity, bool syncInServer, DataManager.Callback callback)
        {
            AddItem(item.ID, quantity, syncInServer, callback);
        }

        public void AddItem(int itemID, int quantity, bool syncInServer, DataManager.Callback callback)
        {
            if (HasItem(itemID))
            {
                _warehouse[itemID] += quantity;
            }
            else
            {
                _warehouse.Add(itemID, quantity);
            }
            if (syncInServer)
                DataManager.instance.UpdateItem(itemID, quantity, callback);
        }

        public bool HasItem(Item item)
        {
            return HasItem(item.ID);
        }

        public bool HasItem(int itemID)
        {
            return _warehouse.ContainsKey(itemID);
        }

        public bool HasEnoughtOfItem(Item item, int quantity)
        {
            if (!HasItem(item)) return false;
            if(_warehouse[item.ID] >= quantity)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void RemoveItemQuantity(Item item, int quantity)
        {
            if(HasEnoughtOfItem(item, quantity))
            {
                _warehouse[item.ID] -= quantity;
            }
        }

        public int GetNumOfItem(Item item)
        {
            if(!HasItem(item))return 0;
            else
            {
                return _warehouse[item.ID];
            }
        }
    }
}
