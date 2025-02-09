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
