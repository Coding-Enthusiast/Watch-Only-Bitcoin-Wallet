// WatchOnlyBitcoinWallet
// Copyright (c) 2016 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using Autarkysoft.Bitcoin;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using WatchOnlyBitcoinWallet.Models;
using WatchOnlyBitcoinWallet.Services.PriceServices;

namespace WatchOnlyBitcoinWallet.Services.BalanceServices
{
    public class MempoolSpace : ApiBase, IPriceApi, IBalanceApi
    {
        private const string BaseUrl = "https://mempool.space/api/";

        public async Task<Response<decimal>> UpdatePriceAsync()
        {
            Response<decimal> resp = new();
            Response<JObject> apiResp = await SendApiRequestAsync($"{BaseUrl}v1/prices");
            if (!apiResp.IsSuccess)
            {
                resp.Error = apiResp.Error;
                return resp;
            }
            Debug.Assert(apiResp.Result is not null);

            decimal? t = (decimal?)apiResp.Result?["USD"];
            if (t is null)
            {
                resp.Error = BuildError("USD", "mempool.space");
                return resp;
            }
            resp.Result = t.Value;
            resp.IsSuccess = true;
            return resp;
        }


        public async Task<Response<List<PriceHistory>>> GetPriceHistoryAsync(DateTime start, DateTime end)
        {
            Response<List<PriceHistory>> resp = new();
            Response<JObject> apiResp = await SendApiRequestAsync($"{BaseUrl}v1/historical-price?currency=USD");
            if (!apiResp.IsSuccess)
            {
                resp.Error = apiResp.Error;
                return resp;
            }
            if (apiResp.Result is null)
            {
                resp.Error = "Result is not set (this is a bug!)";
                return resp;
            }

            JToken? priceArray = apiResp.Result["prices"];
            if (priceArray is null)
            {
                resp.Error = "API response does not include \"prices\" token.";
                return resp;
            }

            resp.Result = new List<PriceHistory>();
            foreach (JToken item in priceArray)
            {
                try
                {
                    long? time = (long?)item["time"];
                    decimal? price = (decimal?)item["USD"];
                    if (!time.HasValue || !price.HasValue)
                    {
                        resp.Error = "API response does not include \"time\" and/or \"USD\" token.";
                        return resp;
                    }

                    PriceHistory temp = new(time.Value, price.Value);
                    resp.Result.Add(temp);
                }
                catch (Exception ex)
                {
                    resp.Error = $"An exception was thrown: {ex.Message}";
                    return resp;
                }
            }
            
            return resp;
        }


        public async Task<Response> UpdateBalancesAsync(List<BitcoinAddress> addrList)
        {
            Response<decimal> resp = new();
            foreach (var addr in addrList)
            {
                Response<JObject> apiResp = await SendApiRequestAsync($"{BaseUrl}address/{addr.Address}");
                if (!apiResp.IsSuccess)
                {
                    resp.Error = apiResp.Error;
                    return resp;
                }
                Debug.Assert(apiResp.Result is not null);

                ulong? total = (ulong?)apiResp.Result?["chain_stats"]?["funded_txo_sum"];
                ulong? spent = (ulong?)apiResp.Result?["chain_stats"]?["spent_txo_sum"];
                if (total is null)
                {
                    resp.Error = BuildError("[chain_stats][funded_txo_sum]", "mempool.space");
                    return resp;
                }
                if (spent is null)
                {
                    resp.Error = BuildError("[chain_stats][spent_txo_sum]", "mempool.space");
                    return resp;
                }

                decimal newBalance = (total.Value - spent.Value) * Constants.Satoshi;
                Debug.Assert(newBalance >= 0);

                addr.Difference = newBalance - addr.Balance;
                addr.Balance = newBalance;
            }

            resp.IsSuccess = true;
            return resp;
        }


        public async Task<Response> UpdateTransactionListAsync(List<BitcoinAddress> addrList)
        {
            Response resp = new();
            foreach (var addr in addrList)
            {
                string url = $"{BaseUrl}address/{addr.Address}/txs/chain";
                Response<JObject> apiResp = await SendApiRequestAsync(url);
                if (!apiResp.IsSuccess)
                {
                    resp.Error = apiResp.Error;
                    return resp;
                }
                List<TxModel> temp = new();
                foreach (var item in apiResp.Result["txrefs"])
                {
                    TxModel tx = new()
                    {
                        TxId = item["txid"].ToString(),
                        BlockHeight = (int)item["status"]["block_height"],
                        Amount = ((Int64)item["tx_input_n"] == -1) ? (Int64)item["value"] : -(Int64)item["value"],
                        ConfirmedTime = (DateTime)item["confirmed"]
                    };

                    TxModel tempTx = temp.Find(x => x.TxId == tx.TxId);
                    if (tempTx is null)
                    {
                        temp.Add(tx);
                    }
                    else
                    {
                        tempTx.Amount += tx.Amount;
                    }
                }
                temp.ForEach(x => x.Amount *= Constants.Satoshi);
                addr.TransactionList = temp;
            }
            return resp;
        }
    }
}
