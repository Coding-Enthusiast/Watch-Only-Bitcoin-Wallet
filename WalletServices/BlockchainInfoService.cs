using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Models;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;

namespace WalletServices
{
    public class BlockchainInfoService : ApiCall
    {
        public static async void GetBalace(ObservableCollection<BitcoinAddress> bitAddrList)
        {
            foreach (var bitAddr in bitAddrList)
            {
                string url = "https://blockchain.info/address/" + bitAddr.Address + "?format=json&limit=0";
                JObject jResult = await GetApiResponse(url);
                decimal satoshi = 0.00000001m;
                bitAddr.Balance = (int)jResult["final_balance"] * satoshi;
            }
        }
    }
}
