using MVVMLibrary;
using WatchOnlyBitcoinWallet.Services;

namespace WatchOnlyBitcoinWallet.Models
{
    public class SettingsModel : CommonBase
    {
        public decimal BitcoinPriceInUSD { get; set; }
        public decimal DollarPriceInLocalCurrency { get; set; }
        public string LocalCurrencySymbol { get; set; }
        public BalanceServiceNames SelectedBalanceApi { get; set; }
        public PriceServiceNames SelectedPriceApi { get; set; }
    }
}
