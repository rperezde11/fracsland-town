namespace Items
{
    /// <summary>
    /// Used to know how much on an item we have in the inventory
    /// </summary>
    public class InventoryItem
    {
        public Item item;
        public int quantity;
        public int position;

        public InventoryItem(Item item, int quantity, int position)
        {
            this.item = item;
            this.quantity = quantity;
            this.position = position;
        }

        public string getQuantity()
        {
            return "x" + quantity.ToString() + " ";
        }
    }
}