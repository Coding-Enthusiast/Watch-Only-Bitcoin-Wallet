using MVVMLibrary;
using System;
using System.Collections.ObjectModel;
using WatchOnlyBitcoinWallet.Models;
using WatchOnlyBitcoinWallet.Services;
using WatchOnlyBitcoinWallet.Services.PriceServices;

namespace WatchOnlyBitcoinWallet.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        public SettingsViewModel()
        {
            BalanceApiList = new ObservableCollection<BalanceServiceNames>((BalanceServiceNames[])Enum.GetValues(typeof(BalanceServiceNames)));
            PriceApiList = new ObservableCollection<PriceServiceNames>((PriceServiceNames[])Enum.GetValues(typeof(PriceServiceNames)));

            UpdatePriceCommand = new BindableCommand(UpdatePrice, () => !IsReceiving);
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
                    UpdatePriceCommand.RaiseCanExecuteChanged();
                }
            }
        }
        private bool isReceiving;


        public ObservableCollection<BalanceServiceNames> BalanceApiList { get; set; }

        public ObservableCollection<PriceServiceNames> PriceApiList { get; set; }


        private SettingsModel settings;
        public SettingsModel Settings
        {
            get { return settings; }
            set { SetField(ref settings, value); }
        }


        public BalanceServiceNames SelectedBalanceApi
        {
            get { return Settings.SelectedBalanceApi; }
            set
            {
                if (Settings.SelectedBalanceApi != value) // Can't use SetField here because of "ref"
                {
                    Settings.SelectedBalanceApi = value;
                    RaisePropertyChanged("SelectedBalanceApi");
                }
            }
        }


        public PriceServiceNames SelectedPriceApi
        {
            get { return Settings.SelectedPriceApi; }
            set
            {
                if (Settings.SelectedPriceApi != value)
                {
                    Settings.SelectedPriceApi = value;
                    RaisePropertyChanged("SelectedPriceApi");
                }
            }
        }


        public decimal BitcoinPrice
        {
            get { return Settings.BitcoinPriceInUSD; }
            set
            {
                if (Settings.BitcoinPriceInUSD != value)
                {
                    Settings.BitcoinPriceInUSD = value;
                    RaisePropertyChanged("BitcoinPrice");
                }
            }
        }

        public decimal USDPrice
        {
            get { return Settings.DollarPriceInLocalCurrency; }
            set
            {
                if (Settings.DollarPriceInLocalCurrency != value)
                {
                    Settings.DollarPriceInLocalCurrency = value;
                    RaisePropertyChanged("USDPrice");
                }
            }
        }

        public string LocalCurrencySymbol
        {
            get { return Settings.LocalCurrencySymbol; }
            set
            {
                if (Settings.LocalCurrencySymbol != value)
                {
                    Settings.LocalCurrencySymbol = value;
                    RaisePropertyChanged("LocalCurrencySymbol");
                }
            }
        }


        public BindableCommand UpdatePriceCommand { get; private set; }
        private async void UpdatePrice()
        {
            Status = "Fetching Bitcoin Price...";
            Errors = string.Empty;
            IsReceiving = true;

            PriceApi api = null;
            switch (Settings.SelectedPriceApi)
            {
                case PriceServiceNames.Bitfinex:
                    api = new Bitfinex();
                    break;
                case PriceServiceNames.Btce:
                    api = new Btce();
                    break;
                case PriceServiceNames.Coindesk:
                    api = new Coindesk();
                    break;
                default:
                    api = new Bitfinex();
                    break;
            }

            Response<decimal> resp = await api.UpdatePriceAsync();
            if (resp.Errors.Any())
            {
                Errors = resp.Errors.GetErrors();
                Status = "Encountered an error!";
            }
            else
            {
                Settings.BitcoinPriceInUSD = resp.Result;
                RaisePropertyChanged("BitcoinPrice");
                Status = "Price Update Success!";
            }

            IsReceiving = false;
        }

    }
}
