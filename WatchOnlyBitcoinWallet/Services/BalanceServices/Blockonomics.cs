using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WatchOnlyBitcoinWallet.Models;

namespace WatchOnlyBitcoinWallet.Services.BalanceServices
{
    class Blockonomics : BalanceApi
    {
        private async Task<Response<JObject>> PostApiAsync(string url, JObject body)
        {
            Response<JObject> resp = new Response<JObject>();
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    ServicePointManager.SecurityProtocol =
                        SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls |
                        SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                    StringContent cont = new StringContent(body.ToString(), Encoding.UTF8, "application/json");
                    
                    HttpResponseMessage httpResp = await client.PostAsync(url, cont);
                    string result = await httpResp.Content.ReadAsStringAsync();
                    JObject jResult = JObject.Parse(result);
                    if (httpResp.IsSuccessStatusCode)
                    {
                        resp.Result = jResult;
                    }
                    else
                    {
                        resp.Errors.Add(jResult["message"].ToString());
                    }
                }
                catch (Exception ex)
                {
                    string errMsg = (ex.InnerException == null) ? ex.Message : ex.Message + " " + ex.InnerException;
                    resp.Errors.Add(errMsg);
                }
            }
            return resp;
        }


        public override async Task<Response> UpdateBalancesAsync(List<BitcoinAddress> addrList)
        {
            Response<JObject> resp = new Response<JObject>();

            JObject j = new JObject();
            j.Add("addr", string.Join(" ", addrList.Select(x => x.Address)));
            Response<JObject> apiResp = await PostApiAsync("https://www.blockonomics.co/api/balance", j);
            if (apiResp.Errors.Any())
            {
                resp.Errors.AddRange(apiResp.Errors);
                return resp;
            }

            foreach (var item in apiResp.Result["response"])
            {
                BitcoinAddress addr = addrList.Find(x => x.Address == item["addr"].ToString());
                if (addr != null)
                {
                    decimal bal = (Int64)item["confirmed"] * Satoshi;
                    addr.Difference = bal - addr.Balance;
                    addr.Balance = bal;
                }
            }
            return resp;
        }

        public override Task<Response> UpdateTransactionListAsync(List<BitcoinAddress> addrList)
        {
            throw new NotImplementedException();
        }
    }
}
