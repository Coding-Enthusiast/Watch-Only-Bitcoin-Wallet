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

        private string _name = string.Empty;
        /// <summary>
        /// Name acts as a tag for the address
        /// </summary>
        public string Name
        {
            get => _name;
            set => SetField(ref _name, value);
        }

        private string _address = string.Empty;
        public string Address
        {
            get => _address;
            set => SetField(ref _address, value);
        }

        private decimal _balance;
        public decimal Balance
        {
            get => _balance;
            set => SetField(ref _balance, value);
        }

        private decimal _diff;
        [JsonIgnore]
        public decimal Difference
        {
            get => _diff;
            set => SetField(ref _diff, value);
        }

        private decimal _forkBalance;
        /// <summary>
        /// Total balance that was available by the time of fork
        /// </summary>
        [JsonIgnore]
        public decimal ForkBalance
        {
            get => _forkBalance;
            set => SetField(ref _forkBalance, value);
        }

        public List<TxModel> TransactionList { get; set; } = new();
    }
}