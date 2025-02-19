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
            FileMan = new FileManager();

            AddressList = new(FileMan.ReadWalletFile());
            SettingsInstance = FileMan.ReadSettingsFile();

            GetBalanceCommand = new BindableCommand(GetBalance, () => !IsReceiving);
            OpenAboutCommand = new BindableCommand(OpenAbout);
            OpenSettingsCommand = new BindableCommand(OpenSettings);
            ForkBalanceCommand = new BindableCommand(ForkBalance);

            ImportFromTextCommand = new BindableCommand(ImportFromText);
            ImportFromFileCommand = new BindableCommand(ImportFromFile);

            AddCommand = new(Add);
            RemoveCommand = new(Remove, () => SelectedAddress is not null);
            EditCommand = new(Edit, () => SelectedAddress is not null);
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

        public IClipboard Clipboard { get; set; }
        public IFileManager FileMan { get; set; }
        public IWindowManager WindowMan { get; set; }
        public SettingsModel SettingsInstance { get; }
        public string VersionString { get; }

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

            await Task.Delay(TimeSpan.FromSeconds(10));
            lock (lockObj)
            {
                SaveWallet();
                isSavePending = false;
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


        public BindableCommand AddCommand { get; }
        public async void Add()
        {
            AddEditViewModel vm = new();
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
                Errors = "Nothing is selected!";
            }
        }

        public BindableCommand EditCommand { get; }
        public async void Edit()
        {
            if (SelectedAddress is not null)
            {
                AddEditViewModel vm = new()
                {
                    AddressString = SelectedAddress.Address,
                    Tag = SelectedAddress.Name
                };

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
                Errors = "Nothing is selected!";
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
