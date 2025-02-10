// WatchOnlyBitcoinWallet
// Copyright (c) 2016 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using System;
using WatchOnlyBitcoinWallet.MVVM;

namespace WatchOnlyBitcoinWallet.Models
{
    public class Transaction : InpcBase
    {
        /// <summary>
        /// Transaction ID
        /// </summary>
        public string TxId { get; set; }

        /// <summary>
        /// Block height that contains the transaction.
        /// </summary>
        public int BlockHeight { get; set; }

        /// <summary>
        /// Amount that was sent or received (+/-)
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// <see cref="DateTime"/> value of when this transaction was confirmed.
        /// </summary>
        public DateTime ConfirmedTime { get; set; }

        /// <summary>
        /// Value of this transaction in USD.
        /// </summary>
        public decimal UsdValue { get; set; }
    }
}