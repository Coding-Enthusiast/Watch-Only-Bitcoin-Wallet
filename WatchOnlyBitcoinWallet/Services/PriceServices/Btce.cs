using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace WatchOnlyBitcoinWallet.Services.PriceServices
{
    public class Btce : PriceApi
    {
        public override async Task<Response<decimal>> UpdatePriceAsync()
        {
            Response<decimal> resp = new Response<decimal>();
            Response<JObject> apiResp = await SendApiRequestAsync("https://btc-e.com/api/3/ticker/btc_usd");
            if (apiResp.Errors.Any())
            {
                resp.Errors.AddRange(apiResp.Errors);
                return resp;
            }
            resp.Result = (decimal)apiResp.Result["btc_usd"]["last"];
            return resp;
        }
    }
}
