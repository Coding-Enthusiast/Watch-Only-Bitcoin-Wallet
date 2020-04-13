using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using WatchOnlyBitcoinWallet.Models;

namespace WatchOnlyBitcoinWallet.Services
{
    public enum PriceServiceNames
    {
        Bitfinex,
        Btce,
        Coindesk
    }
    public enum BalanceServiceNames
    {
        BlockCypher,
        Blockonomics
    }

    public abstract class Api
    {
        protected async Task<Response<JObject>> SendApiRequestAsync(string url)
        {
            Response<JObject> resp = new Response<JObject>();
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    ServicePointManager.SecurityProtocol =
                        SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls |
                        SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                    string result = await client.GetStringAsync(url);
                    resp.Result = JObject.Parse(result);
                }
                catch (Exception ex)
                {
                    string errMsg = (ex.InnerException == null) ? ex.Message : ex.Message + " " + ex.InnerException;
                    resp.Errors.Add(errMsg);
                }
            }
            return resp;
        }
    }

    public abstract class PriceApi : Api
    {
        public abstract Task<Response<decimal>> UpdatePriceAsync();
    }

    public abstract class BalanceApi : Api
    {
        public abstract Task<Response> UpdateBalancesAsync(List<BitcoinAddress> addrList);

        public abstract Task<Response> UpdateTransactionListAsync(List<BitcoinAddress> addrList);

        internal decimal Satoshi = 0.00000001m;
    }
}
