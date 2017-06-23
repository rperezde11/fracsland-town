using UnityEngine;
using Utils;
using System.Collections.Generic;

namespace Items
{
    /// <summary>
    /// This is the base class of all our items in te game
    /// </summary>
    public class Item {

        public static Item Wood = new Item(Constants.ID_WOOD, "Wood", "inventory_wood");

        public static Item Carrot = new Item(Constants.ID_CARROT, "Carrot", "Carrot");

        public static Item Pumpkin = new Item(Constants.ID_PUMPKIN, "Pumpkin", "pumpkin");
        public static Item BlueMushroom = new Item(Constants.ID_BLUE_MUSHROOM, "BlueMushroom", "BlueMushroom");
        public static Item RedMushrooms = new Item(Constants.ID_RED_MUSHROOM, "RedMushrooms", "RedMushrooms");
        public static Item Straw = new Item(Constants.ID_STRAW, "Straw", "straw");

        private int _id;
        public int ID { get { return _id; } }

        private string _name;
        public string Name { get { return _name; } }

        private Sprite _inventoryRepresentation;
        public Sprite InventoryRepresentation { get { return _inventoryRepresentation; } }

        private int _goldAmmount;
        public int GoldAmmount { get { return _goldAmmount; } }

        private int _diamondsAmmount;
        public int DiamondsAmmount { get { return _diamondsAmmount; } }

        protected bool _isEquipable = false;
        public bool IsEquipable { get { return _isEquipable; } }

        //USed to get items by id
        public static Dictionary<int, Item> Items;

        public Item(int id, string name, string image)
        {
            _id = id;
            _name = name;
            _inventoryRepresentation = (Sprite) Resources.Load("Media/Images/"+image, typeof(Sprite));
        }

        public Item(int id, string name, bool isConsumible, string image, int goldAmmount, int diamondsAmmount)
        {
            _id = id;
            _name = name;
            _inventoryRepresentation = (Sprite)Resources.Load("Media/Images/" + image, typeof(Sprite));
            _goldAmmount = goldAmmount;
            _diamondsAmmount = diamondsAmmount;
        }

        /// <summary>
        /// This is used to know what items we have in the game just in case we need to loop over them
        /// or find one with ID.
        /// </summary>
        /// <param name="item"></param>
        public static void RegisterItem(Item item)
        {
            Items.Add(item.ID, item);
        }

        public static Item GetItemByID(int itemID)
        {
            return Items[itemID];
        }

        /// <summary>
        /// Track all the items that we have created in fracsland and make them accessible from a dictionary
        /// </summary>
        public static void CreateAndInitializeItemsDictionary()
        {
            Items = new Dictionary<int, Item>();

            Item.RegisterItem(Item.Wood);

            Item.RegisterItem(EquippableItem.axe1);
            Item.RegisterItem(EquippableItem.axe2);
            Item.RegisterItem(EquippableItem.axe3);
            Item.RegisterItem(EquippableItem.axe4);
            Item.RegisterItem(EquippableItem.axe5);
            Item.RegisterItem(EquippableItem.axe6);

            Item.RegisterItem(EquippableItem.hat1);
            Item.RegisterItem(EquippableItem.hat2);
            Item.RegisterItem(EquippableItem.hat3);
            Item.RegisterItem(EquippableItem.hat4);
            Item.RegisterItem(EquippableItem.hat5);
            Item.RegisterItem(EquippableItem.hat6);
            Item.RegisterItem(EquippableItem.hat7);

            Item.RegisterItem(Skin.skin1);
            Item.RegisterItem(Skin.skin2);
            Item.RegisterItem(Skin.skin3);
            Item.RegisterItem(Skin.skin4);
            Item.RegisterItem(Skin.skin5);
            Item.RegisterItem(Skin.skin6);
            Item.RegisterItem(Skin.skin7);
            Item.RegisterItem(Skin.skin8);
            Item.RegisterItem(Skin.skin9);
            Item.RegisterItem(Skin.skin10);
            Item.RegisterItem(Skin.skin11);
            Item.RegisterItem(Skin.skin12);
            Item.RegisterItem(Skin.skin13);

            Item.RegisterItem(Item.Carrot);
            Item.RegisterItem(Item.Pumpkin);
            Item.RegisterItem(Item.BlueMushroom);
            Item.RegisterItem(Item.RedMushrooms);
            Item.RegisterItem(Item.Straw);

            Item.RegisterItem(Potion.HealthPotion);
        }
    }
}
