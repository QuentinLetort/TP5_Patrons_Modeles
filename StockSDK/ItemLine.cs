using System;

namespace StockSDK
{
    public class ItemLine
    {
        public Item Item { get; }
        public int Quantity { get; }
        public ItemLine(Item item, int quantity)
        {
            this.Item = item;
            this.Quantity = quantity;
        }
    }
}
