// WatchOnlyBitcoinWallet
// Copyright (c) 2016 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using Autarkysoft.Bitcoin;
using Autarkysoft.Bitcoin.Encoders;
using System;
using System.Collections.Generic;
using System.Linq;
using WatchOnlyBitcoinWallet.Models;
using WatchOnlyBitcoinWallet.MVVM;

namespace WatchOnlyBitcoinWallet.ViewModels
{
    public class AddEditViewModel : ViewModelBase
    {
        /// <summary>
        /// Make designer happy!
        /// </summary>
        public AddEditViewModel() : this(string.Empty, string.Empty, Array.Empty<BitcoinAddress>())
        {
        }


        public AddEditViewModel(IList<BitcoinAddress> addrList) : this(string.Empty, string.Empty, addrList)
        {
        }

        public AddEditViewModel(string addr, string tag, IList<BitcoinAddress> addrList)
        {
            addresses = addrList;
            // Set fields since we don't want to change IsChanged to true.
            _addr = addr;
            _tag = tag;

            OkCommand = new(Ok);
            CancelCommand = new(Cancel);
        }


        private readonly IList<BitcoinAddress> addresses;
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


        public BindableCommand OkCommand { get; }
        private void Ok()
        {
            AddressType type = Address.GetAddressType(AddressString, NetworkType.MainNet);
            if (type is AddressType.Unknown or AddressType.Invalid)
            {
                Error = "Invalid address type.";
            }
            else if (addresses.Any(x => x.Address == AddressString))
            {
                Error = "Wallet already contains the given address.";
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
