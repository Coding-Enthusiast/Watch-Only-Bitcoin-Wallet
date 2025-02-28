// WatchOnlyBitcoinWallet
// Copyright (c) 2016 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using System.Collections.Generic;
using System.Threading.Tasks;
using WatchOnlyBitcoinWallet.Models;

namespace WatchOnlyBitcoinWallet.Services.BalanceServices
{
    public interface IBalanceApi
    {
        public abstract Task<Response> UpdateBalancesAsync(List<BitcoinAddress> addrList);
        public abstract Task<Response> UpdateTransactionListAsync(List<BitcoinAddress> addrList);
    }
}
