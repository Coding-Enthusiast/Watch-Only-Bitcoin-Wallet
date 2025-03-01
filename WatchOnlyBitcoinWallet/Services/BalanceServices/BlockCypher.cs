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

namespace WatchOnlyBitcoinWallet.Services.BalanceServices
{
    public class BlockCypher : ApiBase, IBalanceApi
    {
        public async Task<Response> UpdateBalancesAsync(List<BitcoinAddress> addrList)
        {
            Response resp = new();
            int count = 0;
            Stopwatch timer = Stopwatch.StartNew();
            foreach (BitcoinAddress addr in addrList)
            {
                string url = $"https://api.blockcypher.com/v1/btc/main/addrs/{addr.Address}/balance";
                Response<JObject> apiResp = await SendApiRequestAsync(url);
                if (!apiResp.IsSuccess)
                {
                    resp.Error = apiResp.Error;
                    break;
                }
                Debug.Assert(apiResp.Result is not null);

                ulong? t = (ulong?)apiResp.Result["final_balance"];
                if (t is null)
                {
                    resp.Error = BuildError("final_balance", "BlockCypher");
                    return resp;
                }

                decimal bal = t.Value * Constants.Satoshi;
                addr.Difference = bal - addr.Balance;
                addr.Balance = bal;

                // Blockcypher limits calls to 3 per second
                if (count == 3)
                {
                    count = 0;
                    timer.Stop();
                    if (timer.Elapsed < TimeSpan.FromSeconds(1))
                    {
                        int miliSecDelay = TimeConstants.MilliSeconds.OneSec - timer.Elapsed.Milliseconds;
                        Debug.Assert(miliSecDelay > 0);
                        await Task.Delay(miliSecDelay + 100);
                    }
                    timer.Restart();
                }
            }

            resp.IsSuccess = true;
            return resp;
        }


        public async Task<Response> UpdateTransactionListAsync(List<BitcoinAddress> addrList)
        {
            Response resp = new();
            int count = 0;
            Stopwatch timer = Stopwatch.StartNew();
            foreach (var addr in addrList)
            {
                string url = "https://api.blockcypher.com/v1/btc/main/addrs/" + addr.Address + "?limit=2000";
                Response<JObject> apiResp = await SendApiRequestAsync(url);
                if (!apiResp.IsSuccess)
                {
                    resp.Error = apiResp.Error;
                    break;
                }
                Debug.Assert(apiResp.Result is not null);

                if (!TryExtract(apiResp.Result, "final_n_tx", out int cap, out string error))
                {
                    resp.Error = error;
                    return resp;
                }

                if (cap == 0)
                {
                    resp.IsSuccess = true;
                    return resp;
                }

                List<TxModel> temp = new(cap);
                JToken? array = apiResp.Result["txrefs"];
                if (array is null)
                {
                    resp.Error = BuildError("txrefs", "BlockCypher");
                    return resp;
                }

                foreach (JToken? item in array)
                {
                    if (item is null)
                    {
                        resp.Error = BuildError("txrefs", "BlockCypher");
                        return resp;
                    }

                    if (!TryExtract(item, "tx_hash", out string txId, out error) ||
                        !TryExtract(item, "block_height", out int height, out error) ||
                        !TryExtract(item, "tx_input_n", out int isOutput, out error) ||
                        !TryExtract(item, "value", out ulong value, out error) ||
                        !TryExtract(item, "confirmed", out DateTime time, out error))
                    {
                        resp.Error = error;
                        return resp;
                    }

                    decimal amount = (isOutput == -1) ? (Constants.Satoshi * value) : (-Constants.Satoshi * value);
                    TxModel tx = new(txId, height, amount, time);

                    TxModel? tempTx = temp.Find(x => x.TxId == tx.TxId);
                    if (tempTx is null)
                    {
                        temp.Add(tx);
                    }
                    else
                    {
                        tempTx.Amount += tx.Amount;
                    }
                }

                addr.TransactionList = temp;

                // Blockcypher limits calls to 3 per second
                if (count == 3)
                {
                    count = 0;
                    timer.Stop();
                    if (timer.Elapsed < TimeSpan.FromSeconds(1))
                    {
                        int miliSecDelay = TimeConstants.MilliSeconds.OneSec - timer.Elapsed.Milliseconds;
                        Debug.Assert(miliSecDelay > 0);
                        await Task.Delay(miliSecDelay + 100);
                    }
                    timer.Restart();
                }
            }
            return resp;
        }
    }
}
