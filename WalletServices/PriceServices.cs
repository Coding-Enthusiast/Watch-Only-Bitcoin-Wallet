using System;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

namespace WalletServices
{
    public class PriceServices : ApiCall
    {
        public enum ServiceNames
        {
            Bitfinex,
            Btce
        }

        public static async Task<decimal> GetPrice(string serviceName)
        {
            JObject jResult;
            decimal price;
            var name = Enum.Parse(typeof(ServiceNames), serviceName);
            switch ((ServiceNames)name)
            {
                case ServiceNames.Bitfinex:
                    jResult = await GetApiResponse("https://api.bitfinex.com/v1/pubticker/btcusd");
                    price = (decimal)jResult["last_price"];
                    break;
                case ServiceNames.Btce:
                    jResult = await GetApiResponse("https://btc-e.com/api/3/ticker/btc_usd");
                    price = (decimal)jResult["btc_usd"]["last"];
                    break;
                default:
                    price = 0;
                    break;
            }
            return price;
        }
    }
}
