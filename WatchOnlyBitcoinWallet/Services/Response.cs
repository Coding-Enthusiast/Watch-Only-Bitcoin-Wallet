// WatchOnlyBitcoinWallet
// Copyright (c) 2016 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

namespace WatchOnlyBitcoinWallet.Services
{
    public class Response<T> : Response
    {
        public T Result { get; set; }
    }


    public class Response
    {
        private readonly ErrorCollection errors = new ErrorCollection();

        public ErrorCollection Errors
        {
            get { return errors; }
        }
    }
}
