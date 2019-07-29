using MVVMLibrary;
using System;

namespace WatchOnlyBitcoinWallet.Models
{
    public class PriceHistory : CommonBase
    {
        public DateTime Time { get; set; }
        public decimal Price { get; set; }
    }
}
