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
        private decimal _btcUsd;
        public decimal BitcoinPriceInUSD
        {
            get => _btcUsd;
            set => SetField(ref _btcUsd, value);
        }

        decimal _usdLocal;
        public decimal DollarPriceInLocalCurrency
        {
            get => _usdLocal;
            set => SetField(ref _usdLocal, value);
        }

        public string LocalCurrencySymbol { get; set; } = "￥";
        public BalanceServiceNames SelectedBalanceApi { get; set; }
        public PriceServiceNames SelectedPriceApi { get; set; }
    }
}
