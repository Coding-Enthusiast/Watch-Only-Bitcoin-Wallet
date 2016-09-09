using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchOnlyBitcoinWallet
{
    public class BlockchainInfoAPI
    {
        public string hash160 { get; set; }
        public string address { get; set; }
        public int n_tx { get; set; }
        public long total_received { get; set; }
        public long total_sent { get; set; }
        public int final_balance { get; set; }
        public object[] txs { get; set; }
    }
}
