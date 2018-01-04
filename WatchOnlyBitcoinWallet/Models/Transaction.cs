using MVVMLibrary;

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
    }
}