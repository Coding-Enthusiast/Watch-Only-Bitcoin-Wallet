using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using CommonLibrary;
using Models;
using WalletServices;

namespace ViewModels
{
    public class MainWindowViewModel : CommonBase
    {
        public MainWindowViewModel()
        {
            AddressList = new BindingList<BitcoinAddress>(WalletData.LoadAddresses());
            AddressList.ListChanged += AddressList_ListChanged;

            SettingsInstance = WalletData.LoadSettings();
            settingsVM = new SettingsWindowViewModel() { Settings = SettingsInstance };

            GetBalanceCommand = AsyncCommand.Create(() => GetBalance());
            SaveCommand = new RelayCommand(() => Save());
            SettingsCommand = new RelayCommand(() => OpenSettings());
        }


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

        private SettingsClass settingsInstance;
        public SettingsClass SettingsInstance
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

        private SettingsWindowViewModel settingsVM;
        public SettingsWindowViewModel SettingsVM
        {
            get { return settingsVM; }
            set
            {
                if (settingsVM != value)
                {
                    settingsVM = value;
                    RaisePropertyChanged("SettingsVM");
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



        public RelayCommand SettingsCommand { get; private set; }
        private void OpenSettings()
        {
            Window myWin = new Window()
            {
                Content = SettingsVM,
                Height = 174.5,
                Width = 322,
                Owner = Application.Current.MainWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Icon = Application.Current.MainWindow.Icon,
                Title = "Settings"
            };
            myWin.ShowDialog();
            RaisePropertyChanged("BitcoinBalanceUSD");
            RaisePropertyChanged("BitcoinBalanceLC");
            RaisePropertyChanged("LocalCurrencySymbol");
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


        public RelayCommand SaveCommand { get; private set; }
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
