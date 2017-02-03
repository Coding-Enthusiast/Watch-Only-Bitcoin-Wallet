using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MVVMLibrary;
using WatchOnlyBitcoinWallet.Models;
using WatchOnlyBitcoinWallet.Services;

namespace WatchOnlyBitcoinWallet.ViewModels
{
    public class MainWindowViewModel : CommonBase
    {
        public MainWindowViewModel()
        {
            AddressList = new BindingList<BitcoinAddress>(WalletData.LoadAddresses());
            AddressList.ListChanged += AddressList_ListChanged;

            SettingsInstance = WalletData.LoadSettings();

            GetBalanceCommand = AsyncCommand.Create(() => GetBalance());
            SaveCommand = new BindableCommand(() => Save());
            SettingsCommand = new BindableCommand(() => OpenSettings());
        }


        /// <summary>
        /// List of all available addresses in the wallet.
        /// </summary>
        public BindingList<BitcoinAddress> AddressList { get; set; }

        private void AddressList_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.ItemChanged)
            {
                var addrs = (BindingList<BitcoinAddress>)sender;
                StringBuilder sb = new StringBuilder();
                foreach (var item in addrs)
                {
                    if (item.HasErrors)
                    {
                        foreach (var er in item.GetErrors("Address"))
                        {
                            sb.Append(er);
                        }
                    }
                }
                MessageToDisplay = sb.ToString();

                IsSaveButtonEnabled = (sb.Length != 0) ? false : true;
            }
        }

        private string messageToDisplay;
        public string MessageToDisplay
        {
            get { return messageToDisplay; }
            set
            {
                if (messageToDisplay != value)
                {
                    messageToDisplay = value;
                    RaisePropertyChanged("MessageToDisplay");
                }
            }
        }

        private SettingsModel settingsInstance;
        public SettingsModel SettingsInstance
        {
            get { return settingsInstance; }
            set
            {
                if (settingsInstance != value)
                {
                    settingsInstance = value;
                    RaisePropertyChanged("SettingsInstance");
                }
            }
        }


        private decimal bitcoinBalance;
        public decimal BitcoinBalance
        {
            get
            {
                bitcoinBalance = AddressList.Sum(x => (decimal)x.Balance);
                return bitcoinBalance;
            }
            set
            {
                if (bitcoinBalance != value)
                {
                    bitcoinBalance = value;
                    RaisePropertyChanged("BitcoinBalance");
                    RaisePropertyChanged("BitcoinBalanceUSD");
                    RaisePropertyChanged("BitcoinBalanceLC");
                }
            }
        }

        private decimal bitcoinBalanceUSD;
        public decimal BitcoinBalanceUSD
        {
            get
            {
                bitcoinBalanceUSD = bitcoinBalance * SettingsInstance.BitcoinPriceInUSD;
                return bitcoinBalanceUSD;
            }
            set
            {
                if (bitcoinBalanceUSD != value)
                {
                    bitcoinBalanceUSD = value;
                    RaisePropertyChanged("BitcoinBalanceUSD");
                }
            }
        }

        private decimal bitcoinBalanceLC;
        public decimal BitcoinBalanceLC
        {
            get
            {
                bitcoinBalanceLC = bitcoinBalanceUSD * SettingsInstance.DollarPriceInLocalCurrency;
                return bitcoinBalanceLC;
            }
            set
            {
                if (bitcoinBalanceLC != value)
                {
                    bitcoinBalanceLC = value;
                    RaisePropertyChanged("BitcoinBalanceLC");
                }
            }
        }

        private string localCurrencySymbol;
        public string LocalCurrencySymbol
        {
            get
            {
                localCurrencySymbol = SettingsInstance.LocalCurrencySymbol;
                return localCurrencySymbol;
            }
            set
            {
                if (localCurrencySymbol != value)
                {
                    localCurrencySymbol = value;
                    RaisePropertyChanged("LocalCurrencySymbol");
                }
            }
        }



        public BindableCommand SettingsCommand { get; private set; }
        private void OpenSettings()
        {
            IWindowManager winManager = new SettingsWindowManager();
            SettingsViewModel vm = new SettingsViewModel();
            vm.Settings = SettingsInstance;
            winManager.Show(vm);
        }


        public IAsyncCommand GetBalanceCommand { get; private set; }
        private async Task GetBalance()
        {
            bool isChanged = await BlockchainInfoService.GetBalace(AddressList.ToList());
            if (isChanged)
            {
                RaisePropertyChanged("BitcoinBalance");
                RaisePropertyChanged("BitcoinBalanceUSD");
                RaisePropertyChanged("BitcoinBalanceLC");
            }
        }


        public BindableCommand SaveCommand { get; private set; }
        private void Save()
        {
            WalletData.Save(AddressList.ToList());
            IsSaveButtonEnabled = false;
        }

        private bool isSaveButtonEnabled;
        public bool IsSaveButtonEnabled
        {
            get { return isSaveButtonEnabled; }
            set
            {
                if (isSaveButtonEnabled != value)
                {
                    isSaveButtonEnabled = value;
                    RaisePropertyChanged("IsSaveButtonEnabled");
                }
            }
        }
    }
}
