using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchOnlyBitcoinWallet
{
    public class SettingsClass
    {
        public decimal BitcoinPriceInUSD { get; set; }
        public decimal DollarPriceInLocalCurrency { get; set; }
        public string LocalCurrencySymbol { get; set; }
    }
}
