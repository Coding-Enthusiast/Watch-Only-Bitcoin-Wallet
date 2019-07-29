using MVVMLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using WatchOnlyBitcoinWallet.Models;
using WatchOnlyBitcoinWallet.Services;
using WatchOnlyBitcoinWallet.Services.BalanceServices;
using WatchOnlyBitcoinWallet.Services.PriceServices;

namespace WatchOnlyBitcoinWallet.ViewModels
{
    public class ForkBalanceViewModel : ViewModelBase
    {
        public ForkBalanceViewModel()
        {
            ForkHeightList = new ObservableCollection<ForkHeights>(Enum.GetValues(typeof(ForkHeights)).Cast<ForkHeights>());

            GetTransactionsCommand = new BindableCommand(GetTransactions, () => !IsReceiving);
        }



        public enum ForkHeights
        {
            Bitcore = 463604,
            BitcoinCash = 478558,
            BitcoinGold = 491407,
            /// Many forks have been scams or didn't even happen. Uncomment/Edit the following lines to have these listed.
            //Bitcore2 = 492820,
            //BitcoinX = 498888,
            //Super_Bitcoin = 498888,
            //Lightning_Bitcoin = 499999,
            //Bitcoi_Cash_Plus = 501407,
        }


        public string Note
        {
            get
            {
                return "Note: Balances are only based on bitcoin blockchain and transactions there, assuming coins are not already spent on other chains.";
            }
        }


        public ObservableCollection<BitcoinAddress> AddressList { get; set; }


        public ObservableCollection<ForkHeights> ForkHeightList { get; set; }


        private ForkHeights selectedForkHeight;
        public ForkHeights SelectedForkHeight
        {
            get { return selectedForkHeight; }
            set
            {
                if (SetField(ref selectedForkHeight, value))
                {
                    SelectedBlockHeight = (int)value;
                }
            }
        }


        private int selectedBlockHeight;
        public int SelectedBlockHeight
        {
            get { return selectedBlockHeight; }
            set
            {
                if (SetField(ref selectedBlockHeight, value))
                {
                    if (AddressList != null)
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
                            RaisePropertyChanged("Total");
                        }
                    }
                }
            }
        }


        public decimal Total
        {
            get { return AddressList.Sum(x => x.ForkBalance); }
        }


        /// <summary>
        /// Indicating an active connection.
        /// <para/> Used to enable/disable buttons
        /// </summary>
        public bool IsReceiving
        {
            get { return isReceiving; }
            set
            {
                if (SetField(ref isReceiving, value))
                {
                    GetTransactionsCommand.RaiseCanExecuteChanged();
                }
            }
        }
        private bool isReceiving;



        public BindableCommand GetTransactionsCommand { get; private set; }
        private async void GetTransactions()
        {
            Status = "Updating transaction lists...";
            Errors = string.Empty;
            IsReceiving = true;

            BalanceApi ba = new BlockCypher();
            // Bech32 addresses in AddressList should be ignored until the block explorers and forks start supporting it.
            Response resp = await ba.UpdateTransactionListAsync(AddressList.Where(x =>
                    !x.Address.StartsWith("bc1", StringComparison.OrdinalIgnoreCase)).ToList());

            Coindesk pa = new Coindesk();
            DateTime start = AddressList.Where(x =>
                    !x.Address.StartsWith("bc1", StringComparison.OrdinalIgnoreCase))
                    .Min(x => x.TransactionList.Min(y => y.ConfirmedTime));
            DateTime end = AddressList.Where(x =>
                    !x.Address.StartsWith("bc1", StringComparison.OrdinalIgnoreCase)).Max(x => x.TransactionList.Max(y => y.ConfirmedTime)); ;
            Response<List<PriceHistory>> resp2 = await pa.GetPriceHistoryAsync(start, end);

            foreach (var adr in AddressList)
            {
                if (adr.TransactionList == null)
                {
                    continue;
                }
                foreach (var tx in adr.TransactionList)
                {
                    PriceHistory ph = resp2.Result.Find(x => x.Time.Date.Equals(tx.ConfirmedTime.Date));
                    if (ph != null)
                    {
                        tx.UsdValue = ph.Price;
                    }
                }
            }

            if (resp.Errors.Any())
            {
                Errors = resp.Errors.GetErrors();
                Status = "Encountered an error!";
            }
            else
            {
                Save();
                Status = "Transaction Update Success!";
            }

            IsReceiving = false;
        }
        private void Save()
        {
            DataManager.WriteFile(AddressList, DataManager.FileType.Wallet);
        }

    }
}