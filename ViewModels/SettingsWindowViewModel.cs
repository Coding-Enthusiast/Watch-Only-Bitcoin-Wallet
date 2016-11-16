using System;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

using CommonLibrary;
using Models;
using WalletServices;

namespace ViewModels
{
    public class SettingsWindowViewModel : CommonBase
    {
        public SettingsWindowViewModel()
        {
            settings = new SettingsClass();

            PriceServiceList = new ObservableCollection<string>(Enum.GetNames(typeof(PriceServices.ServiceNames)));

            GetPriceCommand = AsyncCommand.Create(() => GetPrice());
            SaveCommand = new RelayCommand(() => Save());
        }

        public IAsyncCommand GetPriceCommand { get; private set; }
        private async Task GetPrice()
        {
            decimal price = await PriceServices.GetPrice(SelectedPriceService);
            BitcoinPrice = price;
        }

        public RelayCommand SaveCommand { get; private set; }
        private void Save()
        {
            WalletData.SaveSettings(Settings);
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


        private ObservableCollection<string> priceServiceList;
        public ObservableCollection<string> PriceServiceList
        {
            get { return priceServiceList; }
            set
            {
                if (priceServiceList != value)
                {
                    priceServiceList = value;
                    RaisePropertyChanged("PriceServiceList");
                }
            }
        }

        private string selectedPriceService;
        public string SelectedPriceService
        {
            get { return selectedPriceService; }
            set
            {
                if (selectedPriceService != value)
                {
                    selectedPriceService = value;
                    RaisePropertyChanged("SelectedPriceService");
                }
            }
        }


        private SettingsClass settings;
        public SettingsClass Settings
        {
            get { return settings; }
            set
            {
                if (settings != value)
                {
                    settings = value;
                    RaisePropertyChanged("Settings");
                    IsSaveButtonEnabled = true;
                }
            }
        }

        public decimal BitcoinPrice
        {
            get
            {
                return Settings.BitcoinPriceInUSD;
            }
            set
            {
                if (Settings.BitcoinPriceInUSD != value)
                {
                    Settings.BitcoinPriceInUSD = value;
                    RaisePropertyChanged("BitcoinPrice");
                    IsSaveButtonEnabled = true;
                }
            }
        }

        public decimal USDPrice
        {
            get
            {
                return Settings.DollarPriceInLocalCurrency;
            }
            set
            {
                if (Settings.DollarPriceInLocalCurrency != value)
                {
                    Settings.DollarPriceInLocalCurrency = value;
                    RaisePropertyChanged("USDPrice");
                    IsSaveButtonEnabled = true;
                }
            }
        }

        public string LocalCurrencySymbol
        {
            get
            {
                return Settings.LocalCurrencySymbol;
            }
            set
            {
                if (Settings.LocalCurrencySymbol != value)
                {
                    Settings.LocalCurrencySymbol = value;
                    RaisePropertyChanged("LocalCurrencySymbol");
                    IsSaveButtonEnabled = true;
                }
            }
        }
    }
}
