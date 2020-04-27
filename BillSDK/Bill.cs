using System.Linq;
using System;
using UserSDK;
using System.Collections.Generic;
using StockSDK;
using RPC;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace BillSDK
{
    public class Bill
    {
        public User User { get; }
        public List<BillLine> ListLines { get; }
        public float SubTotal { get; set; }
        public float TotalWithTaxes { get; set; }
        public Bill(User user, List<BillLine> listLines)
        {
            this.User = user;
            this.ListLines = listLines;
            ComputeSubtotal();
            ComputeTotalWithTaxes();
        }
        private void ComputeSubtotal()
        {
            float result = 0;
            foreach (BillLine line in ListLines)
            {
                result += line.SubTotal;
            }
            SubTotal = result;
        }
        private void ComputeTotalWithTaxes()
        {
            float tps = 0.05f * SubTotal;
            float tvq = 0.09975f * SubTotal;
            TotalWithTaxes = SubTotal + tps + tvq;
        }
        public override string ToString()
        { 
            return $"Subtotal: {SubTotal} , Total with taxes: {TotalWithTaxes}";
        }
        public static Bill CreateBill(User user, List<ItemLine> lines)
        {
            RPCClient rpcClient = new RPCClient();
            JObject message = new JObject(){
                {"products", (JArray)JToken.FromObject(lines)},
                {"user", (JObject)JToken.FromObject(user)}
            }; 
            string response = rpcClient.Call(message.ToString(), "bill_queue");
            rpcClient.Close();
            return JsonConvert.DeserializeObject<Bill>(response); 
        }
    }
}
