// WatchOnlyBitcoinWallet
// Copyright (c) 2016 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using Autarkysoft.Bitcoin;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WatchOnlyBitcoinWallet.Models;

namespace WatchOnlyBitcoinWallet.Services.BalanceServices
{
    public class Blockchair : ApiBase, IBalanceApi
    {
        public async Task<Response> UpdateBalancesAsync(List<BitcoinAddress> addrList)
        {
            Response resp = new();
            string all = string.Join(',', addrList.Select(x => x.Address));
            string url = $"https://api.blockchair.com/bitcoin/addresses/balances?addresses={all}";
            Response<JObject> apiResp = await SendApiRequestAsync(url);
            if (!apiResp.IsSuccess)
            {
                resp.Error = apiResp.Error;
                return resp;
            }
            Debug.Assert(apiResp.Result is not null);

            JToken? data = apiResp.Result["data"];
            if (data is null)
            {
                resp.Error = BuildError("data", "blockchair");
                return resp;
            }

            List<string> updatedAddrs = new(addrList.Count);
            foreach (JToken item in data)
            {
                string addr = item.Path.Substring("data.".Length);
                JToken? value = item.First;
                if (value is null)
                {
                    resp.Error = "";
                    return resp;
                }

                decimal bal = (ulong)value * Constants.Satoshi;
                updatedAddrs.Add(addr);
                BitcoinAddress? address = addrList.Find(x => x.Address == addr);
                if (address is null)
                {
                    resp.Error = $"Blockchair API returned {addr} that was not found in your wallet!";
                    return resp;
                }
                address.Difference = bal - address.Balance;
                address.Balance = bal;
            }

            foreach (var item in addrList)
            {
                if (!updatedAddrs.Contains(item.Address))
                {
                    item.Balance = 0;
                }
            }

            resp.IsSuccess = true;
            return resp;
        }

        public Task<Response> UpdateTransactionListAsync(List<BitcoinAddress> addrList)
        {
            throw new NotImplementedException();
        }
    }
}
