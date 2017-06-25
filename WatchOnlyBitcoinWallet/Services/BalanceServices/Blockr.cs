using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WatchOnlyBitcoinWallet.Models;

namespace WatchOnlyBitcoinWallet.Services.BalanceServices
{
    public class Blockr : BalanceApi
    {
        public override async Task<Response> UpdateBalancesAsync(List<Models.BitcoinAddress> addrList)
        {
            Response resp = new Response();

            string aggregatedAddr = addrList.Select(x => x.Address).Aggregate((a, b) => a + "," + b);

            string url = "http://btc.blockr.io/api/v1/address/balance/" + aggregatedAddr;
            Response<JObject> apiResp = await SendApiRequestAsync(url);
            if (apiResp.Errors.Any())
            {
                resp.Errors.AddRange(apiResp.Errors);
                return resp;
            }
            if (apiResp.Result["status"].ToString() != "success")
            {
                resp.Errors.Add(apiResp.Result["message"].ToString());
                return resp;
            }

            if (addrList.Count > 1)
            {
                foreach (var item in apiResp.Result["data"])
                {
                    decimal bal = (decimal)item["balance"];
                    BitcoinAddress ba = addrList.Find(x => x.Address == item["address"].ToString());
                    ba.Difference = bal - ba.Balance;
                    ba.Balance = bal;
                }
            }
            else
            {
                decimal bal = (decimal)apiResp.Result["data"]["balance"];
                addrList[0].Difference = bal - addrList[0].Balance;
                addrList[0].Balance = bal;
            }

            return resp;
        }
    }
}
