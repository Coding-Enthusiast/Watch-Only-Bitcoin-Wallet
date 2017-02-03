using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchOnlyBitcoinWallet.Models;

namespace WatchOnlyBitcoinWallet.Services
{
    public class BlockchainInfoService : ApiCall
    {
        public static async Task<bool> GetBalace(List<BitcoinAddress> bitAddrList)
        {
            bool IsBalanceChanged = false;
            foreach (var bitAddr in bitAddrList)
            {
                string url = "https://blockchain.info/address/" + bitAddr.Address + "?format=json&limit=0";
                JObject jResult = await GetApiResponse(url);
                decimal satoshi = 0.00000001m;
                decimal bal = (int)jResult["final_balance"] * satoshi;
                if (bitAddr.Balance != bal)
                {
                    bitAddr.Balance = bal;
                    IsBalanceChanged = true;
                }
            }
            return IsBalanceChanged;
        }
    }
}
