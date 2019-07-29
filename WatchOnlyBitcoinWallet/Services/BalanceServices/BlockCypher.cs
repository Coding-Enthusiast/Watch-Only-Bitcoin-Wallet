using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WatchOnlyBitcoinWallet.Models;

namespace WatchOnlyBitcoinWallet.Services.BalanceServices
{
    public class BlockCypher : BalanceApi
    {
        public override async Task<Response> UpdateBalancesAsync(List<BitcoinAddress> addrList)
        {
            Response resp = new Response();
            foreach (var addr in addrList)
            {
                string url = "https://api.blockcypher.com/v1/btc/main/addrs/" + addr.Address + "/balance";
                Response<JObject> apiResp = await SendApiRequestAsync(url);
                if (apiResp.Errors.Any())
                {
                    resp.Errors.AddRange(apiResp.Errors);
                    break;
                }
                decimal bal = (Int64)apiResp.Result["final_balance"] * Satoshi;
                addr.Difference = bal - addr.Balance;
                addr.Balance = bal;
            }
            return resp;
        }


        public override async Task<Response> UpdateTransactionListAsync(List<BitcoinAddress> addrList)
        {
            Response resp = new Response();
            foreach (var addr in addrList)
            {
                string url = "https://api.blockcypher.com/v1/btc/main/addrs/" + addr.Address + "?limit=2000";
                Response<JObject> apiResp = await SendApiRequestAsync(url);
                if (apiResp.Errors.Any())
                {
                    resp.Errors.AddRange(apiResp.Errors);
                    break;
                }
                List<Transaction> temp = new List<Transaction>();
                foreach (var item in apiResp.Result["txrefs"])
                {
                    Transaction tx = new Transaction();
                    tx.TxId = item["tx_hash"].ToString();
                    tx.BlockHeight = (int)item["block_height"];
                    tx.Amount = ((Int64)item["tx_input_n"] == -1) ? (Int64)item["value"] : -(Int64)item["value"];
                    tx.ConfirmedTime = (DateTime)item["confirmed"];

                    Transaction tempTx = temp.Find(x => x.TxId == tx.TxId);
                    if (tempTx == null)
                    {
                        temp.Add(tx);
                    }
                    else
                    {
                        tempTx.Amount += tx.Amount;
                    }
                }
                temp.ForEach(x => x.Amount *= Satoshi);
                addr.TransactionList = temp;
            }
            return resp;
        }

    }
}
