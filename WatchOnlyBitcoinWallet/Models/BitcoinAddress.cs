// WatchOnlyBitcoinWallet
// Copyright (c) 2016 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using Newtonsoft.Json;
using System.Collections.Generic;
using WatchOnlyBitcoinWallet.MVVM;

namespace WatchOnlyBitcoinWallet.Models
{
    public class BitcoinAddress : InpcBase
    {
        public BitcoinAddress()
        {
        }

        public BitcoinAddress(string addr, string tag)
        {
            Address = addr;
            Name = tag;
        }

        /// <summary>
        /// Name acts as a tag for the address
        /// </summary>
        public string Name { get; set => SetField(ref field, value); } = string.Empty;
        public string Address { get; set => SetField(ref field, value); } = string.Empty;
        public decimal Balance { get; set => SetField(ref field, value); }
        [JsonIgnore]
        public decimal Difference { get; set => SetField(ref field, value); }

        /// <summary>
        /// Total balance that was available by the time of fork
        /// </summary>
        [JsonIgnore]
        public decimal ForkBalance { get; set => SetField(ref field, value); }

        public List<TxModel> TransactionList { get; set; } = new();
    }
}