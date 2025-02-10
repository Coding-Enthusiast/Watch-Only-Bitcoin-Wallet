// WatchOnlyBitcoinWallet
// Copyright (c) 2016 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using System.Windows;
using WatchOnlyBitcoinWallet.ViewModels;
using WatchOnlyBitcoinWallet.Views;

namespace WatchOnlyBitcoinWallet.Services
{
    public interface IWindowManager
    {
        void Show(ViewModelBase ViewModel);
    }

    public class SettingsWindowManager : IWindowManager
    {
        public void Show(ViewModelBase ViewModel)
        {
            SettingsWindow myWin = new SettingsWindow();
            myWin.DataContext = ViewModel;
            //myWin.Owner = Application.Current.MainWindow;
            //myWin.ShowDialog();
        }
    }

    public class ForkBalanceWindowManager : IWindowManager
    {
        public void Show(ViewModelBase ViewModel)
        {
            ForkBalanceWindow myWin = new ForkBalanceWindow();
            myWin.DataContext = ViewModel;
            //myWin.Owner = Application.Current.MainWindow;
            //myWin.ShowDialog();
        }
    }

    public class ImportWindowManager : IWindowManager
    {
        public void Show(ViewModelBase ViewModel)
        {
            ImportWindow myWin = new ImportWindow();
            myWin.DataContext = ViewModel;
            ((ImportViewModel)ViewModel).ClosingRequest += (sender, e) => myWin.Close();
            //myWin.Owner = Application.Current.MainWindow;
            //myWin.ShowDialog();
        }
    }
}