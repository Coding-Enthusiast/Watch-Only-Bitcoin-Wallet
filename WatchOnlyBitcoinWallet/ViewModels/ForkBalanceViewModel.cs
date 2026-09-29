// WatchOnlyBitcoinWallet
// Copyright (c) 2016 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.Collections.Generic;
using System.Linq;
using WatchOnlyBitcoinWallet.Models;
using WatchOnlyBitcoinWallet.MVVM;
using WatchOnlyBitcoinWallet.Services;
using WatchOnlyBitcoinWallet.Services.BalanceServices;

namespace WatchOnlyBitcoinWallet.ViewModels
{
    public class ForkBalanceViewModel : ViewModelBase
    {
        /// <summary>
        /// Make designer happy!
        /// </summary>
        public ForkBalanceViewModel() : this(Array.Empty<BitcoinAddress>(), new FileManager())
        {
        }

        public ForkBalanceViewModel(IList<BitcoinAddress> addrList, IFileManager fileManager)
        {
            AddressList = addrList.ToList();
            fileMan = fileManager;
            ForkHeightList = EnumHelper.GetAllEnumValues<ForkHeights>().ToArray();
            GetTransactionsCommand = new BindableCommand(GetTransactions, () => !IsReceiving);
        }



        public enum ForkHeights
        {
            Bitcore = 463604,
            BitcoinCash = 478558,
            BitcoinGold = 491407,
            BitcoinBlake = 961632,
            /// Many forks have been scams or didn't even happen. Uncomment/Edit the following lines to have these listed.
            //Bitcore2 = 492820,
            //BitcoinX = 498888,
            //Super_Bitcoin = 498888,
            //Lightning_Bitcoin = 499999,
            //Bitcoi_Cash_Plus = 501407,
        }


        public string Note => "Note: Balances are only based on bitcoin blockchain and transactions there, assuming coins are not already spent on other chains.";


        private readonly IFileManager fileMan;
        public List<BitcoinAddress> AddressList { get; }
        public ForkHeights[] ForkHeightList { get; }


        private ForkHeights _selForkH;
        public ForkHeights SelectedForkHeight
        {
            get => _selForkH;
            set
            {
                if (SetField(ref _selForkH, value))
                {
                    SelectedBlockHeight = (int)value;
                }
            }
        }


        private int _selectedBlockH;
        public int SelectedBlockHeight
        {
            get => _selectedBlockH;
            set
            {
                if (SetField(ref _selectedBlockH, value))
                {
                    foreach (var addr in AddressList)
                    {
                        if (addr.TransactionList != null)
                        {
                            addr.ForkBalance = addr.TransactionList.Sum(x => x.BlockHeight > value ? 0 : x.Amount);
                        }
                        else
                        {
                            addr.ForkBalance = 0;
                        }
                        RaisePropertyChanged(nameof(Total));
                    }
                }
            }
        }


        public decimal Total => AddressList.Sum(x => x.ForkBalance);


        private bool _isReceiving;
        public bool IsReceiving
        {
            get => _isReceiving;
            set
            {
                if (SetField(ref _isReceiving, value))
                {
                    GetTransactionsCommand.RaiseCanExecuteChanged();
                }
            }
        }



        public BindableCommand GetTransactionsCommand { get; private set; }
        private async void GetTransactions()
        {
            Status = "Updating transaction lists...";
            Error = string.Empty;
            IsReceiving = true;

            IBalanceApi ba = new BlockCypher();
            Response resp = await ba.UpdateTransactionListAsync(AddressList);
            if (!resp.IsSuccess)
            {
                Error = resp.Error;
                Status = "Encountered an error!";
                return;
            }

            MempoolSpace pa = new();
            DateTime start = AddressList.Min(x => x.TransactionList.Min(y => y.ConfirmedTime));
            DateTime end = AddressList.Max(x => x.TransactionList.Max(y => y.ConfirmedTime));
            Response<List<PriceHistory>> resp2 = await pa.GetPriceHistoryAsync(start, end);

            if (resp2.IsSuccess && resp2.Result is not null)
            {
                foreach (var adr in AddressList)
                {
                    if (adr.TransactionList == null)
                    {
                        continue;
                    }
                    foreach (var tx in adr.TransactionList)
                    {
                        PriceHistory? ph = resp2.Result.Find(x => x.Time.Date.Equals(tx.ConfirmedTime.Date));
                        if (ph != null)
                        {
                            tx.UsdValue = ph.Price;
                        }
                    }
                }
            }

            Save();
            Status = "Transaction Update Success!";
            IsReceiving = false;
        }
        private void Save()
        {
            fileMan.WriteWallet(AddressList);
        }
    }
}
