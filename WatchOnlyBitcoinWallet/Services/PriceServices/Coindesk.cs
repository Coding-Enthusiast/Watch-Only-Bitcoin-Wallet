// WatchOnlyBitcoinWallet
// Copyright (c) 2016 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WatchOnlyBitcoinWallet.Models;

namespace WatchOnlyBitcoinWallet.Services.PriceServices
{
    public class Coindesk : ApiBase, IPriceApi
    {
        public async Task<Response<decimal>> UpdatePriceAsync()
        {
            Response<decimal> resp = new Response<decimal>();
            Response<JObject> apiResp = await SendApiRequestAsync("https://api.coindesk.com/v1/bpi/currentprice.json");
            if (!apiResp.IsSuccess)
            {
                resp.Error = apiResp.Error;
                return resp;
            }
            resp.Result = (decimal)apiResp.Result["bpi"]["USD"]["rate"];
            return resp;
        }



        public async Task<Response<List<PriceHistory>>> GetPriceHistoryAsync(DateTime start, DateTime end)
        {
            Response<List<PriceHistory>> resp = new Response<List<PriceHistory>>();
            Response<JObject> apiResp = await SendApiRequestAsync($"https://api.coindesk.com/v1/bpi/historical/close.json?start={start.ToString("yyyy-mm-dd")}&end={end.ToString("yyyy-mm-dd")}");
            if (!apiResp.IsSuccess)
            {
                resp.Error = apiResp.Error;
                return resp;
            }
            resp.Result = new List<PriceHistory>();
            for (DateTime i = start; i < end; i = i.AddDays(1))
            {
                try
                {
                    decimal price = (decimal)apiResp.Result["bpi"][i.ToString("yyyy-mm-dd")];
                    resp.Result.Add(new PriceHistory() { Time = i, Price = price });
                }
                catch (Exception) { }
            }
            return resp;
        }
    }
}
