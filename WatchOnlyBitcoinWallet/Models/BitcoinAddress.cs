// WatchOnlyBitcoinWallet
// Copyright (c) 2016 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using Newtonsoft.Json;
using System.Collections.Generic;
using WatchOnlyBitcoinWallet.ViewModels;

namespace WatchOnlyBitcoinWallet.Models
{
    public class BitcoinAddress : ValidatableBase
    {
        private string name;
        /// <summary>
        /// Name acts as a tag for the address
        /// </summary>
        public string Name
        {
            get { return name; }
            set { SetField(ref name, value); }
        }

        private string address;
        public string Address
        {
            get { return address; }
            set
            {
                if (SetField(ref address, value))
                {
                    Validate(value);
                }
            }
        }

        private decimal balance;
        public decimal Balance
        {
            get { return balance; }
            set { SetField(ref balance, value); }
        }

        private decimal difference;
        [JsonIgnore]
        public decimal Difference
        {
            get { return difference; }
            set { SetField(ref difference, value); }
        }

        private decimal forkBalance;
        /// <summary>
        /// Total balance that was available by the time of fork
        /// </summary>
        [JsonIgnore]
        public decimal ForkBalance
        {
            get { return forkBalance; }
            set { SetField(ref forkBalance, value); }
        }


        public List<Transaction> TransactionList { get; set; }

    }
}