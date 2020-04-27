using System.Collections.Generic;
using System;
using StockSDK;

namespace BillSDK
{
    public class BillLine
    {
        public Item Item { get; }
        public int Quantity { get; }
        public float SubTotal { get; }

        public BillLine(Item item, int quantity)
        {
            Item = item;
            Quantity = quantity;
            SubTotal = quantity * item.UnitPrice;
        }
    }
}
