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
