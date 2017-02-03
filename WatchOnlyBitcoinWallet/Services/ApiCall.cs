using System;
using System.Threading.Tasks;

using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace WatchOnlyBitcoinWallet.Services
{
    public class ApiCall
    {
        public static async Task<JObject> GetApiResponse(string reqUri)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    string result = await client.GetStringAsync(reqUri);
                    return JObject.Parse(result);
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
    }

}
