// WatchOnlyBitcoinWallet
// Copyright (c) 2016 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.Collections.Generic;
using WatchOnlyBitcoinWallet.Models;
using WatchOnlyBitcoinWallet.MVVM;
using WatchOnlyBitcoinWallet.Services;
using WatchOnlyBitcoinWallet.Services.BalanceServices;
using WatchOnlyBitcoinWallet.Services.PriceServices;

namespace WatchOnlyBitcoinWallet.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        /// <summary>
        /// Make designer happy!
        /// </summary>
        public SettingsViewModel() : this(new SettingsModel())
        {
        }

        public SettingsViewModel(SettingsModel settings)
        {
            Settings = settings;

            BalanceApiList = new(EnumHelper.GetAllEnumValues<BalanceServiceNames>());
            PriceApiList = new(EnumHelper.GetAllEnumValues<PriceServiceNames>());

            UpdatePriceCommand = new BindableCommand(UpdatePrice, () => !IsReceiving);
        }


        public SettingsModel Settings { get; }

        public List<BalanceServiceNames> BalanceApiList { get; }
        public List<PriceServiceNames> PriceApiList { get; }


        private bool _isRcv;
        public bool IsReceiving
        {
            get => _isRcv;
            set
            {
                if (SetField(ref _isRcv, value))
                {
                    UpdatePriceCommand.RaiseCanExecuteChanged();
                }
            }
        }


        public BindableCommand UpdatePriceCommand { get; private set; }
        private async void UpdatePrice()
        {
            Status = "Fetching Bitcoin Price...";
            Error = string.Empty;
            IsReceiving = true;

            IPriceApi api = Settings.SelectedPriceApi switch
            {
                PriceServiceNames.MempoolSpace => new MempoolSpace(),
                PriceServiceNames.Bitfinex => new Bitfinex(),
                PriceServiceNames.Coindesk => new Coindesk(),
                _ => throw new NotImplementedException(),
            };

            Response<decimal> resp = await api.UpdatePriceAsync();
            if (!resp.IsSuccess)
            {
                Error = resp.Error;
                Status = "Encountered an error!";
            }
            else
            {
                Settings.BitcoinPriceInUSD = resp.Result;
                Status = "Price Update Success!";
            }

            IsReceiving = false;
        }

    }
}
