using System;
using UserSDK;
using System.Collections.Generic;
using StockSDK;

namespace BillSDK
{
    public class Bill
    {
        private User User { get; }
        private List<BillLine> ListLines { get; }
        private float SubTotal { get; }
        private float TotalWithTaxes { get; }
        public Bill(User user, List<BillLine> listLines, float subTotal, float totalWithTaxes)
        {
            this.User = user;
            this.ListLines = listLines;
            this.SubTotal = subTotal;
            this.TotalWithTaxes = totalWithTaxes;
        }
        public static Bill CreateBill(User user, List<ItemLine> lines)
        {
            return null;
        }


    }
}
