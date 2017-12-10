using MVVMLibrary;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using WatchOnlyBitcoinWallet.Models;
using WatchOnlyBitcoinWallet.Services;
using WatchOnlyBitcoinWallet.Services.BalanceServices;

namespace WatchOnlyBitcoinWallet.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            AddressList = new BindingList<BitcoinAddress>(DataManager.ReadFile<List<BitcoinAddress>>(DataManager.FileType.Wallet));
            AddressList.ListChanged += AddressList_ListChanged;

            SettingsInstance = DataManager.ReadFile<SettingsModel>(DataManager.FileType.Settings);

            GetBalanceCommand = new BindableCommand(GetBalance, () => !IsReceiving);
            SettingsCommand = new BindableCommand(() => OpenSettings());

            var ver = Assembly.GetExecutingAssembly().GetName().Version;
            VersionString = string.Format("Version {0}.{1}.{2}", ver.Major, ver.Minor, ver.Build);
        }



        void AddressList_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.ItemChanged)
            {
                var addrs = (BindingList<BitcoinAddress>)sender;
                if (!addrs[e.NewIndex].HasErrors)
                {
                    DataManager.WriteFile(AddressList, DataManager.FileType.Wallet);
                }
            }
            else if (e.ListChangedType == ListChangedType.ItemDeleted)
            {
                DataManager.WriteFile(AddressList, DataManager.FileType.Wallet);
            }
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
                    GetBalanceCommand.RaiseCanExecuteChanged();
                }
            }
        }
        private bool isReceiving;

        public string VersionString { get; private set; }


        public BindingList<BitcoinAddress> AddressList { get; set; }


        private SettingsModel settingsInstance;
        public SettingsModel SettingsInstance
        {
            get { return settingsInstance; }
            set { SetField(ref settingsInstance, value); }
        }

        public decimal BitcoinBalance
        {
            get
            {
                return AddressList.Sum(x => (decimal)x.Balance);
            }
        }

        [DependsOnProperty(new string[] { "BitcoinBalance", "SettingsInstance" })]
        public decimal BitcoinBalanceUSD
        {
            get
            {
                return BitcoinBalance * SettingsInstance.BitcoinPriceInUSD;
            }
        }

        [DependsOnProperty(new string[] { "BitcoinBalance", "SettingsInstance" })]
        public decimal BitcoinBalanceLC
        {
            get
            {
                return BitcoinBalanceUSD * SettingsInstance.DollarPriceInLocalCurrency;
            }
        }


        public BindableCommand SettingsCommand { get; private set; }
        private void OpenSettings()
        {
            IWindowManager winManager = new SettingsWindowManager();
            SettingsViewModel vm = new SettingsViewModel();
            vm.Settings = SettingsInstance;
            winManager.Show(vm);
            RaisePropertyChanged("SettingsInstance");
            DataManager.WriteFile(SettingsInstance, DataManager.FileType.Settings);
        }


        public BindableCommand GetBalanceCommand { get; private set; }
        private async void GetBalance()
        {
            Status = "Updating Balances...";
            Errors = string.Empty;
            IsReceiving = true;

            BalanceApi api = null;
            switch (SettingsInstance.SelectedBalanceApi)
            {
                case BalanceServiceNames.BlockchainInfo:
                    api = new BlockchainInfo();
                    break;
                case BalanceServiceNames.BlockExplorer:
                    api = new BlockExplorer();
                    break;
                default:
                    api = new BlockchainInfo();
                    break;
            }

            Response resp = await api.UpdateBalancesAsync(AddressList.ToList());
            if (resp.Errors.Any())
            {
                Errors = resp.Errors.GetErrors();
                Status = "Encountered an error!";
            }
            else
            {
                DataManager.WriteFile(AddressList, DataManager.FileType.Wallet);
                RaisePropertyChanged("BitcoinBalance");
                Status = "Balance Update Success!";
            }

            IsReceiving = false;
        }

    }
}
