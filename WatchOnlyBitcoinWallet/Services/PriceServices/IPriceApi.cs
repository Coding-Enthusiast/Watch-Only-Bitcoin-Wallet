// WatchOnlyBitcoinWallet
// Copyright (c) 2016 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using System.Threading.Tasks;

namespace WatchOnlyBitcoinWallet.Services.PriceServices
{
    public interface IPriceApi
    {
        Task<Response<decimal>> UpdatePriceAsync();
    }
}
