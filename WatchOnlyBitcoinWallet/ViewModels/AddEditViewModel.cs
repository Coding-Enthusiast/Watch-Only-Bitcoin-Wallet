// WatchOnlyBitcoinWallet
// Copyright (c) 2016 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using Autarkysoft.Bitcoin;
using Autarkysoft.Bitcoin.Encoders;
using WatchOnlyBitcoinWallet.MVVM;

namespace WatchOnlyBitcoinWallet.ViewModels
{
    public class AddEditViewModel : ViewModelBase
    {
        public AddEditViewModel() : this(string.Empty, string.Empty)
        {
        }

        public AddEditViewModel(string addr, string tag)
        {
            // Set fields since we don't want to change IsChanged to true.
            _addr = addr;
            _tag = tag;

            OkCommand = new(Ok);
            CancelCommand = new(Cancel);
        }


        public bool IsChanged { get; private set; } = false;

        private string _addr;
        public string AddressString
        {
            get => _addr;
            set
            {
                if (SetField(ref _addr, value))
                {
                    IsChanged = true;
                }
            }
        }

        private string _tag;
        public string Tag
        {
            get => _tag;
            set
            {
                if (SetField(ref _tag, value))
                {
                    IsChanged = true;
                }
            }
        }

        private string _error = string.Empty;
        public string Error
        {
            get => _error;
            set => SetField(ref _error, value);
        }


        public BindableCommand OkCommand { get; }
        private void Ok()
        {
            AddressType type = Address.GetAddressType(AddressString, NetworkType.MainNet);
            if (type is AddressType.Unknown or AddressType.Invalid)
            {
                Error = "Invalid address type.";
            }
            else
            {
                RaiseCloseEvent();
            }
        }

        public BindableCommand CancelCommand { get; }
        private void Cancel()
        {
            IsChanged = false;
            RaiseCloseEvent();
        }
    }
}
