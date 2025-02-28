// WatchOnlyBitcoinWallet
// Copyright (c) 2016 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

namespace WatchOnlyBitcoinWallet.Services
{
    public class Response
    {
        public bool IsSuccess { get; set; } = true;

        private string _error = string.Empty;
        public string Error
        {
            get => _error;
            set
            {
                _error = value;
                IsSuccess = false;
            }
        }
    }

    public class Response<T> : Response
    {
        public T? Result { get; set; }
    }
}
