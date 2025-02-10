// WatchOnlyBitcoinWallet
// Copyright (c) 2016 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using WatchOnlyBitcoinWallet.MVVM;

namespace WatchOnlyBitcoinWallet.ViewModels
{
    public class ViewModelBase : InpcBase
    {
        /// <summary>
        /// Used for changing the visibility of error message TextBox.
        /// </summary>
        public bool IsErrorMsgVisible
        {
            get => isErrorMsgVisible;
            private set => SetField(ref isErrorMsgVisible, value);
        }
        private bool isErrorMsgVisible;

        /// <summary>
        /// String containing all the errors.
        /// </summary>
        public string Errors
        {
            get => errors;
            set
            {
                if (SetField(ref errors, value))
                {
                    IsErrorMsgVisible = !string.IsNullOrEmpty(value);
                }
            }
        }
        private string errors;

        /// <summary>
        /// Status, showing current action being performed.
        /// </summary>
        public string Status
        {
            get => status;
            set => SetField(ref status, value);
        }
        private string status;

    }
}
