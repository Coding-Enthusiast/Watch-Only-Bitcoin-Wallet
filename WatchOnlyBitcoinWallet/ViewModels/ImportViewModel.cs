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
    public class ImportViewModel : ViewModelBase
    {
        /// <summary>
        /// Make designer happy!
        /// </summary>
        public ImportViewModel() : this(Array.Empty<BitcoinAddress>())
        {
        }

        public ImportViewModel(IList<BitcoinAddress> addrList)
        {
            addresses = addrList;
            ImportCommand = new BindableCommand(CheckAndImport);
        }


        public static string Note => $"Enter 1 address per line.{Environment.NewLine}" +
                                     $"Add the optional name at the end separated by comma.{Environment.NewLine}" +
                                     $"Example: address,name";


        private readonly IList<BitcoinAddress> addresses;
        public bool IsChanged { get; private set; } = false;
        public List<BitcoinAddress> Result { get; } = new();


        public string ImportText { get; set; } = string.Empty;


        public BindableCommand ImportCommand { get; private set; }
        private void CheckAndImport()
        {
            Errors = string.Empty;
            if (string.IsNullOrWhiteSpace(ImportText))
            {
                Errors = "Input text can not be empty!";
            }
            else
            {
                string[] lines = ImportText.ReplaceLineEndings()
                                           .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                for (int i = 0; i < lines.Length; i++)
                {
                    int index = lines[i].IndexOf(',');
                    string addr = lines[i].Substring(0, index > 0 ? index : lines[i].Length);
                    string name = index > 0 ? lines[i].Substring(index + 1).Trim() : string.Empty;

                    AddressType type = Address.GetAddressType(addr, NetworkType.MainNet);
                    if (type is AddressType.Unknown or AddressType.Invalid)
                    {
                        Errors = $"Address on the {(i + 1).ToOrdinal()} line ({addr}) is invalid.";
                        return;
                    }
                    else if (addresses.Any(x => x.Address == addr))
                    {
                        Errors = $"Wallet already contains the address on the {(i + 1).ToOrdinal()} line ({addr}).";
                        return;
                    }

                    Result.Add(new BitcoinAddress(addr, name));
                }

                IsChanged = true;
                RaiseCloseEvent();
            }
        }
    }
}
