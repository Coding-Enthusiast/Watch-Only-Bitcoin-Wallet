using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace WatchOnlyBitcoinWallet.Services.PriceServices
{
    public class Bitfinex : PriceApi
    {
        public override async Task<Response<decimal>> UpdatePriceAsync()
        {
            Response<decimal> resp = new Response<decimal>();
            Response<JObject> apiResp = await SendApiRequestAsync("https://api.bitfinex.com/v1/pubticker/btcusd");
            if (apiResp.Errors.Any())
            {
                resp.Errors.AddRange(apiResp.Errors);
                return resp;
            }
            resp.Result = (decimal)apiResp.Result["last_price"];
            return resp;
        }
    }
}
