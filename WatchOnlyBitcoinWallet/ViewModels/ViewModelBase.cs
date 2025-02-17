// WatchOnlyBitcoinWallet
// Copyright (c) 2016 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using System;
using WatchOnlyBitcoinWallet.MVVM;

namespace WatchOnlyBitcoinWallet.ViewModels
{
    public class ViewModelBase : InpcBase
    {
        public event EventHandler? CLoseEvent;

        public void RaiseCloseEvent() => CLoseEvent?.Invoke(this, EventArgs.Empty);

        private bool _isErrVisible;
        public bool IsErrorMsgVisible
        {
            get => _isErrVisible;
            private set => SetField(ref _isErrVisible, value);
        }

        private string _errors = string.Empty;
        public string Errors
        {
            get => _errors;
            set
            {
                if (SetField(ref _errors, value))
                {
                    IsErrorMsgVisible = !string.IsNullOrEmpty(value);
                }
            }
        }

        private string _status = string.Empty;
        public string Status
        {
            get => _status;
            set => SetField(ref _status, value);
        }
    }
}
