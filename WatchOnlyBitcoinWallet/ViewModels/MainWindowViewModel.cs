// WatchOnlyBitcoinWallet
// Copyright (c) 2016 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using Avalonia.Input.Platform;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
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
            FileMan = new FileManager()
            {
                WinMan = WindowMan
            };

            AddressList = new(FileMan.ReadWalletFile());
            SettingsInstance = FileMan.ReadSettingsFile();

            GetBalanceCommand = new BindableCommand(GetBalance, () => !IsReceiving);
            OpenAboutCommand = new BindableCommand(OpenAbout);
            OpenSettingsCommand = new BindableCommand(OpenSettings);
            ForkBalanceCommand = new BindableCommand(ForkBalance);
            ImportCommand = new BindableCommand(Import);

            AddCommand = new(Add);
            RemoveCommand = new(Remove, () => SelectedAddress is not null);
            EditCommand = new(Edit, () => SelectedAddress is not null);
            CopyCommand = new(Copy, () => SelectedAddress is not null);
            MoveUpCommand = new(MoveUp, () => SelectedIndex > 0);
            MoveDownCommand = new(MoveDown, () => SelectedIndex >= 0 && SelectedIndex < AddressList.Count - 1);

            Version ver = Assembly.GetExecutingAssembly().GetName().Version ?? new Version();
            VersionString = string.Format("Version {0}.{1}.{2}", ver.Major, ver.Minor, ver.Build);
        }



        public ObservableCollection<BitcoinAddress> AddressList { get; set; }

        private BitcoinAddress? _selAddr;
        public BitcoinAddress? SelectedAddress
        {
            get => _selAddr;
            set
            {
                if (SetField(ref _selAddr, value))
                {
                    RemoveCommand.RaiseCanExecuteChanged();
                    EditCommand.RaiseCanExecuteChanged();
                    CopyCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private int _selIndex = -1;
        public int SelectedIndex
        {
            get => _selIndex;
            set
            {
                if (SetField(ref _selIndex, value))
                {
                    MoveUpCommand.RaiseCanExecuteChanged();
                    MoveDownCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public IClipboard? Clipboard { get; set; }
        public IFileManager FileMan { get; set; }
        public IWindowManager WindowMan { get; set; }
        public SettingsModel SettingsInstance { get; }
        public string VersionString { get; }


        private bool _isReceiving;
        public bool IsReceiving
        {
            get => _isReceiving;
            set
            {
                if (SetField(ref _isReceiving, value))
                {
                    GetBalanceCommand.RaiseCanExecuteChanged();
                }
            }
        }


        public decimal BitcoinBalance => AddressList.Sum(x => x.Balance);

        [DependsOnProperty(nameof(BitcoinBalance))]
        public decimal BitcoinBalanceUSD => BitcoinBalance * SettingsInstance.BitcoinPriceInUSD;

        [DependsOnProperty(nameof(BitcoinBalance))]
        public decimal BitcoinBalanceLC => BitcoinBalanceUSD * SettingsInstance.DollarPriceInLocalCurrency;


        private void SaveWallet()
        {
            FileMan.WriteWallet(AddressList.ToList());
        }

        private readonly object lockObj = new();
        private bool isSavePending = false;
        private async void QueueSaveWallet()
        {
            lock (lockObj)
            {
                if (isSavePending)
                {
                    return;
                }
                else
                {
                    isSavePending = true;
                }
            }

            await Task.Delay(TimeSpan.FromSeconds(3));
            lock (lockObj)
            {
                SaveWallet();
                isSavePending = false;
            }
        }


        public BindableCommand OpenAboutCommand { get; private set; }
        private async void OpenAbout()
        {
            Debug.Assert(Clipboard is not null);
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
        private async void ForkBalance()
        {
            ForkBalanceViewModel vm = new(AddressList, FileMan);
            await WindowMan.ShowDialog(vm);
        }


        public BindableCommand ImportCommand { get; private set; }
        private async void Import()
        {
            ImportViewModel vm = new(AddressList, FileMan);
            await WindowMan.ShowDialog(vm);
            if (vm.IsChanged)
            {
                foreach (var item in vm.Result)
                {
                    Debug.Assert(AddressList.All(x => x.Name != item.Name));
                    AddressList.Add(item);
                }
                SaveWallet();
                Status = $"Successfully added {vm.Result.Count} addresses.";
            }
        }


        public BindableCommand GetBalanceCommand { get; private set; }
        private async void GetBalance()
        {
            Status = "Updating Balances...";
            Error = string.Empty;
            IsReceiving = true;

            IBalanceApi api = SettingsInstance.SelectedBalanceApi switch
            {
                BalanceServiceNames.MempoolSpace => new MempoolSpace(),
                BalanceServiceNames.BlockCypher => new BlockCypher(),
                BalanceServiceNames.Blockchair => new Blockchair(),
                _ => new BlockCypher(),
            };

            Response resp = await api.UpdateBalancesAsync(AddressList.ToList());
            if (!resp.IsSuccess)
            {
                Error = resp.Error;
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


        public BindableCommand AddCommand { get; }
        public async void Add()
        {
            AddEditViewModel vm = new(AddressList);
            await WindowMan.ShowDialog(vm);
            if (vm.IsChanged)
            {
                BitcoinAddress t = new()
                {
                    Address = vm.AddressString,
                    Name = vm.Tag
                };
                AddressList.Add(t);
                SaveWallet();
            }
        }

        public BindableCommand RemoveCommand { get; }
        private void Remove()
        {
            if (SelectedAddress is not null)
            {
                AddressList.Remove(SelectedAddress);
                SaveWallet();
            }
            else
            {
                Error = "Nothing is selected!";
            }
        }

        public BindableCommand EditCommand { get; }
        public async void Edit()
        {
            if (SelectedAddress is not null)
            {
                AddEditViewModel vm = new(SelectedAddress.Address, SelectedAddress.Name, Array.Empty<BitcoinAddress>());
                await WindowMan.ShowDialog(vm);
                if (vm.IsChanged)
                {
                    SelectedAddress.Address = vm.AddressString;
                    SelectedAddress.Name = vm.Tag;

                    SaveWallet();
                }
            }
            else
            {
                Error = "Nothing is selected!";
            }
        }

        public BindableCommand CopyCommand { get; }
        private void Copy()
        {
            if (Clipboard is null)
            {
                Error = "Clipboard object is not set (this is a bug!).";
            }
            else if (SelectedAddress is not null)
            {
                Clipboard.SetTextAsync(SelectedAddress.Address);
            }
            else
            {
                Error = "Nothing is selected!";
            }
        }

        public BindableCommand MoveUpCommand { get; }
        private void MoveUp()
        {
            Debug.Assert(SelectedIndex != -1); // selected is not null
            Debug.Assert(SelectedIndex != 0); // selected is not first item

            // When items are swapped, the selected item turns into null so SelectedIndex value will turn into -1.
            // Store it here to use later when setting the new selected item
            int i = SelectedIndex;
            (AddressList[i - 1], AddressList[i]) = (AddressList[i], AddressList[i - 1]);
            SelectedAddress = AddressList[i - 1];

            QueueSaveWallet();
        }

        public BindableCommand MoveDownCommand { get; }
        private void MoveDown()
        {
            Debug.Assert(SelectedIndex != -1); // selected is not null
            Debug.Assert(SelectedIndex != AddressList.Count - 1); // selected is not last item

            // When items are swapped, the selected item turns into null so SelectedIndex value will turn into -1.
            // Store it here to use later when setting the new selected item
            int i = SelectedIndex;
            (AddressList[i + 1], AddressList[i]) = (AddressList[i], AddressList[i + 1]);
            SelectedAddress = AddressList[i + 1];

            QueueSaveWallet();
        }
    }
}
