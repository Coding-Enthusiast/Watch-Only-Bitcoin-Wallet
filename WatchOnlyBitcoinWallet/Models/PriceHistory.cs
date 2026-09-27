// WatchOnlyBitcoinWallet
// Copyright (c) 2016 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using Autarkysoft.Bitcoin.Encoders;
using System;

namespace WatchOnlyBitcoinWallet.Models
{
    public class PriceHistory
    {
        public PriceHistory()
        {
        }

        public PriceHistory(DateTime dt, decimal price)
        {
            Time = dt;
            Price = price;
        }

        public PriceHistory(long epoch, decimal price)
        {
            Time = UnixTimeStamp.EpochToTime(epoch);
            Price = price;
        }

        public DateTime Time { get; set; }
        public decimal Price { get; set; }
    }
}
