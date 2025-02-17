// WatchOnlyBitcoinWallet
// Copyright (c) 2016 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using Avalonia.Input.Platform;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using WatchOnlyBitcoinWallet.Models;
using WatchOnlyBitcoinWallet.MVVM;
using WatchOnlyBitcoinWallet.Services;
using WatchOnlyBitcoinWallet.Services.BalanceServices;

namespace WatchOnlyBitcoinWallet.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            WindowMan = new WindowManager();
            FileMan = new FileManager();

            AddressList = new BindingList<BitcoinAddress>(FileMan.ReadWalletFile());
            AddressList.ListChanged += AddressList_ListChanged;

            SettingsInstance = FileMan.ReadSettingsFile();

            GetBalanceCommand = new BindableCommand(GetBalance, () => !IsReceiving);
            OpenAboutCommand = new BindableCommand(OpenAbout);
            OpenSettingsCommand = new BindableCommand(OpenSettings);
            ForkBalanceCommand = new BindableCommand(ForkBalance);

            ImportFromTextCommand = new BindableCommand(ImportFromText);
            ImportFromFileCommand = new BindableCommand(ImportFromFile);

            Version ver = Assembly.GetExecutingAssembly().GetName().Version ?? new Version();
            VersionString = string.Format("Version {0}.{1}.{2}", ver.Major, ver.Minor, ver.Build);
        }



        void AddressList_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.ItemChanged)
            {
                BitcoinAddress addr = ((BindingList<BitcoinAddress>)sender)[e.NewIndex];
                if (addr.Address != null)
                {
                    addr.Validate(addr.Address);
                }
                if (!addr.HasErrors)
                {
                    DataManager.WriteFile(AddressList, DataManager.FileType.Wallet);
                }
            }
            else if (e.ListChangedType == ListChangedType.ItemDeleted || e.ListChangedType == ListChangedType.ItemAdded)
            {
                DataManager.WriteFile(AddressList, DataManager.FileType.Wallet);
            }
        }


        public IWindowManager WindowMan { get; set; }
        public IClipboard Clipboard { get; set; }
        public IFileManager FileMan { get; set; }


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


        public BindableCommand OpenAboutCommand { get; private set; }
        private async void OpenAbout()
        {
            AboutViewModel vm = new($"({VersionString})", Clipboard);
            await WindowMan.ShowDialog(vm);
        }

        public BindableCommand OpenSettingsCommand { get; private set; }
        private async void OpenSettings()
        {
            SettingsViewModel vm = new(SettingsInstance);
            await WindowMan.ShowDialog(vm);
            DataManager.WriteFile(SettingsInstance, DataManager.FileType.Settings);
        }


        public BindableCommand ForkBalanceCommand { get; private set; }
        private void ForkBalance()
        {
            IWindowManager winManager = new ForkBalanceWindowManager();
            ForkBalanceViewModel vm = new ForkBalanceViewModel();
            vm.AddressList = new ObservableCollection<BitcoinAddress>(AddressList);
            winManager.Show(vm);
        }


        public BindableCommand ImportFromTextCommand { get; private set; }
        private void ImportFromText()
        {
            IWindowManager winManager = new ImportWindowManager();
            ImportViewModel vm = new ImportViewModel();
            winManager.Show(vm);

            if (vm.AddressList != null && vm.AddressList.Count != 0)
            {
                vm.AddressList.ForEach(x => AddressList.Add(x));
                Status = string.Format("Successfully added {0} addresses.", vm.AddressList.Count);
            }
        }


        public BindableCommand ImportFromFileCommand { get; private set; }
        private void ImportFromFile()
        {
            Response<string[]> resp = DataManager.OpenFileDialog();
            if (resp.Errors.Any())
            {
                Errors = resp.Errors.GetErrors();
                Status = "Encountered an error while reading from file!";
            }
            else if (resp.Result != null)
            {
                int addrCount = 0;
                foreach (var s in resp.Result)
                {
                    // remove possible white space
                    string addr = s.Replace(" ", "");

                    VerificationResult vr = ValidateAddr(addr);
                    if (vr.IsVerified)
                    {
                        AddressList.Add(new BitcoinAddress() { Address = addr });
                        addrCount++;
                    }
                    else
                    {
                        Errors += Environment.NewLine + vr.Error + ": " + addr;
                    }
                }
                Status = string.Format("Successfully added {0} addresses.", addrCount);
            }
        }
        private VerificationResult ValidateAddr(string addr)
        {
            VerificationResult vr = new VerificationResult();
            if (addr.StartsWith("bc1"))
            {
                vr = SegWitAddress.Verify(addr, SegWitAddress.NetworkType.MainNet);
            }
            else
            {
                vr = Base58.Verify(addr);
            }
            return vr;
        }


        public BindableCommand GetBalanceCommand { get; private set; }
        private async void GetBalance()
        {
            if (!AddressList.ToList().TrueForAll(x => !x.HasErrors))
            {
                Errors = "Fix the errors in addresses first!";
                return;
            }
            Status = "Updating Balances...";
            Errors = string.Empty;
            IsReceiving = true;

            BalanceApi api = null;
            switch (SettingsInstance.SelectedBalanceApi)
            {
                case BalanceServiceNames.BlockCypher:
                    api = new BlockCypher();
                    break;
                case BalanceServiceNames.Blockonomics:
                    api = new Blockonomics();
                    break;
                default:
                    api = new BlockCypher();
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
