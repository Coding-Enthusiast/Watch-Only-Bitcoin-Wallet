// WatchOnlyBitcoinWallet
// Copyright (c) 2016 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Threading.Tasks;

namespace WatchOnlyBitcoinWallet.Services.PriceServices
{
    public class CoinGecko : ApiBase, IPriceApi
    {
        public async Task<Response<decimal>> UpdatePriceAsync()
        {
            Response<decimal> resp = new();
            Response<JObject> apiResp = await SendApiRequestAsync("https://api.coingecko.com/api/v3/exchange_rates");
            if (!apiResp.IsSuccess)
            {
                resp.Error = apiResp.Error;
                return resp;
            }
            Debug.Assert(apiResp.Result is not null);

            JToken? x = apiResp.Result["rates"]?["usd"]?["value"];
            if (x is null)
            {
                resp.Error = "JSON in API response doesn't include the params " +
                             "(API may have changed, please report this as a bug).";
                return resp;
            }

            resp.Result = (decimal)x; 
            return resp;
        }
    }
}
