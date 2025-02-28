// WatchOnlyBitcoinWallet
// Copyright (c) 2016 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Threading.Tasks;

namespace WatchOnlyBitcoinWallet.Services.PriceServices
{
    public class Bitfinex : ApiBase, IPriceApi
    {
        public async Task<Response<decimal>> UpdatePriceAsync()
        {
            Response<decimal> resp = new();
            Response<JObject> apiResp = await SendApiRequestAsync("https://api.bitfinex.com/v1/pubticker/btcusd");
            if (!apiResp.IsSuccess)
            {
                resp.Error = apiResp.Error;
                return resp;
            }
            Debug.Assert(apiResp.Result is not null);

            decimal? t = (decimal?)apiResp.Result?["last_price"];
            if (t is null)
            {
                resp.Error = "API JSON response doesn't include \"last_price\" field (this is a bug).";
                return resp;
            }
            resp.Result = t.Value;
            resp.IsSuccess = true;
            return resp;
        }
    }
}
