// WatchOnlyBitcoinWallet
// Copyright (c) 2016 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace WatchOnlyBitcoinWallet.Services
{
    public enum PriceServiceNames
    {
        MempoolSpace,
        Bitfinex,
        Coindesk,
    }
    public enum BalanceServiceNames
    {
        MempoolSpace,
        BlockCypher,
        Blockonomics,
    }

    public abstract class ApiBase
    {
        protected static async Task<Response<JObject>> SendApiRequestAsync(string url)
        {
            Response<JObject> resp = new();
            try
            {
                using HttpClient client = new();
                string result = await client.GetStringAsync(url);
                resp.Result = JObject.Parse(result);
                resp.IsSuccess = true;
            }
            catch (Exception ex)
            {
                resp.Error = ex.Message;
            }
            return resp;
        }

        protected static string BuildError(string paramName, string apiName)
        {
            return $"JSON in API response doesn't include \"{paramName}\" parameter " +
                   $"({apiName} API may have changed, please report this as a bug).";
        }

        protected static bool TryExtract(JToken token, string fieldName, out int result, out string error)
        {
            JToken? temp = token[fieldName];
            if (temp is null)
            {
                error = BuildError(fieldName, "BlockCypher");
                result = 0;
                return false;
            }

            try
            {
                result = (int)temp;
                error = string.Empty;
                return true;
            }
            catch (Exception ex)
            {
                result = 0;
                error = $"Cannot convert to int {ex.Message}";
                return false;
            }
        }

        protected static bool TryExtract(JToken token, string fieldName, out ulong result, out string error)
        {
            JToken? temp = token[fieldName];
            if (temp is null)
            {
                error = BuildError(fieldName, "BlockCypher");
                result = 0;
                return false;
            }

            try
            {
                result = (ulong)temp;
                error = string.Empty;
                return true;
            }
            catch (Exception ex)
            {
                result = 0;
                error = $"Cannot convert to ulong {ex.Message}";
                return false;
            }
        }

        protected static bool TryExtract(JToken token, string fieldName, out DateTime result, out string error)
        {
            JToken? temp = token[fieldName];
            if (temp is null)
            {
                error = BuildError(fieldName, "BlockCypher");
                result = DateTime.Now;
                return false;
            }

            try
            {
                result = (DateTime)temp;
                error = string.Empty;
                return true;
            }
            catch (Exception ex)
            {
                result = DateTime.Now;
                error = $"Cannot convert to ulong {ex.Message}";
                return false;
            }
        }

        protected static bool TryExtract(JToken token, string fieldName, out string result, out string error)
        {
            JToken? temp = token[fieldName];
            if (temp is null)
            {
                error = BuildError(fieldName, "BlockCypher");
                result = string.Empty;
                return false;
            }

            result = temp.ToString();
            error = string.Empty;
            return true;
        }
    }
}
