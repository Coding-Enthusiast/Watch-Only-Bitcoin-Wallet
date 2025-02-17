// WatchOnlyBitcoinWallet
// Copyright (c) 2016 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using WatchOnlyBitcoinWallet.ViewModels;
using WatchOnlyBitcoinWallet.Views;

namespace WatchOnlyBitcoinWallet.Services
{
    public interface IWindowManager
    {
        void Show(ViewModelBase ViewModel);
        Task ShowDialog(ViewModelBase vm);
    }

    public class WindowManager : IWindowManager
    {
        public void Show(ViewModelBase ViewModel)
        {
            throw new NotImplementedException();
        }

        public Task ShowDialog(ViewModelBase vm)
        {
            Window win = new()
            {
                Content = vm,
                SizeToContent = SizeToContent.WidthAndHeight,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                CanResize = false,
                ShowInTaskbar = false,
                Title = vm.GetType().Name.Replace("ViewModel", ""),
            };

            vm.CLoseEvent += (s, e) => win.Close();

            var lf = (IClassicDesktopStyleApplicationLifetime?)Application.Current?.ApplicationLifetime;
            Debug.Assert(lf is not null);
            Debug.Assert(lf.MainWindow is not null);

            return win.ShowDialog(lf.MainWindow);
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
        public Task ShowDialog(ViewModelBase vm)
        {
            throw new NotImplementedException();
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
        public Task ShowDialog(ViewModelBase vm)
        {
            throw new NotImplementedException();
        }
    }
}