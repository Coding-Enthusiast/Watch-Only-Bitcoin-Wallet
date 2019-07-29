using MVVMLibrary;
using System;

namespace WatchOnlyBitcoinWallet.Models
{
    public class Transaction : CommonBase
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