// WatchOnlyBitcoinWallet
// Copyright (c) 2016 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using System;
using WatchOnlyBitcoinWallet.MVVM;

namespace WatchOnlyBitcoinWallet.Models
{
    public class PriceHistory : InpcBase
    {
        public DateTime Time { get; set; }
        public decimal Price { get; set; }
    }
}
