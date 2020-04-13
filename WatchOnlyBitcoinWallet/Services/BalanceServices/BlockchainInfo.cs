//using Newtonsoft.Json.Linq;
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using WatchOnlyBitcoinWallet.Models;

//namespace WatchOnlyBitcoinWallet.Services.BalanceServices
//{
//    public class BlockchainInfo : BalanceApi
//    {
//        public override async Task<Response> UpdateBalancesAsync(List<BitcoinAddress> addrList)
//        {
//            Response resp = new Response();
//            foreach (var addr in addrList)
//            {
//                string url = "https://blockchain.info/address/" + addr.Address + "?format=json&limit=0";
//                Response<JObject> apiResp = await SendApiRequestAsync(url);
//                if (apiResp.Errors.Any())
//                {
//                    resp.Errors.AddRange(apiResp.Errors);
//                    break;
//                }
//                decimal bal = (Int64)apiResp.Result["final_balance"] * Satoshi;
//                addr.Difference = bal - addr.Balance;
//                addr.Balance = bal;
//            }
//            return resp;
//        }

//        public override Task<Response> UpdateTransactionListAsync(List<BitcoinAddress> addrList)
//        {
//            throw new System.NotImplementedException();
//        }

//    }
//}