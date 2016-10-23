using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using System.IO;
using System.Xml.Serialization;
using System.Net.Http;
using Newtonsoft.Json;

namespace WatchOnlyBitcoinWallet
{
    public class WalletData
    {
        //public static async Task GetBalance(BitcoinAddress BTC)
        //{
        //    using (var client = new HttpClient())
        //    {
        //        string url = "https://blockchain.info/address/" + BTC.Address + "?format=json&limit=0";
        //        try
        //        {
        //            BlockchainInfoAPI BitcoinInfo = JsonConvert.DeserializeObject<BlockchainInfoAPI>(await client.GetStringAsync(url));
        //            decimal balance = BitcoinInfo.final_balance * 0.00000001m;
        //            BTC.Balance = balance;
        //        }
        //        catch (Exception)
        //        {
        //            BTC.Balance = 0;
        //        }
        //    }
        //}
    }
}
