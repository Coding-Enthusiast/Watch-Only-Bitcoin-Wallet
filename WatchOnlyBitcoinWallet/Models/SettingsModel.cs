// WatchOnlyBitcoinWallet
// Copyright (c) 2016 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using WatchOnlyBitcoinWallet.MVVM;
using WatchOnlyBitcoinWallet.Services;

namespace WatchOnlyBitcoinWallet.Models
{
    public class SettingsModel : InpcBase
    {
        public decimal BitcoinPriceInUSD { get; set => SetField(ref field, value); }
        public decimal DollarPriceInLocalCurrency { get; set => SetField(ref field, value); }
        public string LocalCurrencySymbol { get; set; } = "￥";
        public BalanceServiceNames SelectedBalanceApi { get; set; }
        public PriceServiceNames SelectedPriceApi { get; set; }
    }
}
