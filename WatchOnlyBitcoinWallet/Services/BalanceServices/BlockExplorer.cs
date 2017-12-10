using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace WatchOnlyBitcoinWallet.Services.BalanceServices
{
    public class BlockExplorer : BalanceApi
    {
        public override async Task<Response> UpdateBalancesAsync(List<Models.BitcoinAddress> addrList)
        {
            Response resp = new Response();

            using (HttpClient client = new HttpClient())
            {
                decimal satoshi = 0.00000001m;
                foreach (var addr in addrList)
                {
                    try
                    {
                        string result = await client.GetStringAsync("https://blockexplorer.com/api/addr/" + addr.Address + "/balance");
                        decimal bal = int.Parse(result) * satoshi;
                        addr.Difference = bal - addr.Balance;
                        addr.Balance = bal;
                    }
                    catch (Exception ex)
                    {
                        string errMsg = (ex.InnerException == null) ? ex.Message : ex.Message + " " + ex.InnerException;
                        resp.Errors.Add(errMsg);
                        break;
                    }
                }
            }

            return resp;
        }
    }
}
