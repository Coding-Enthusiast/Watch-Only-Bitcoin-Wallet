// WatchOnlyBitcoinWallet
// Copyright (c) 2016 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using System.Diagnostics;
using System.Threading.Tasks;
using WatchOnlyBitcoinWallet.ViewModels;

namespace WatchOnlyBitcoinWallet.Services
{
    public interface IWindowManager
    {
        Task ShowDialog(ViewModelBase vm);
        Task<MessageBoxResult> ShowMessageBox(MessageBoxType mbType, string message);
    }

    public class WindowManager : IWindowManager
    {
        public Task ShowDialog(ViewModelBase vm)
        {
            var lf = (IClassicDesktopStyleApplicationLifetime?)Application.Current?.ApplicationLifetime;
            Debug.Assert(lf is not null);
            Debug.Assert(lf.MainWindow is not null);

            Window win = new()
            {
                Content = vm,
                SizeToContent = SizeToContent.WidthAndHeight,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                CanResize = false,
                ShowInTaskbar = false,
                Title = vm.GetType().Name.Replace("ViewModel", ""),
                Icon = lf.MainWindow.Icon
            };

            vm.CLoseEvent += (s, e) => win.Close();

            return win.ShowDialog(lf.MainWindow);
        }

        public async Task<MessageBoxResult> ShowMessageBox(MessageBoxType mbType, string message)
        {
            MessageBoxViewModel vm = new(mbType, message);
            Window win = new()
            {
                Content = vm,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                CanResize = false,
                ShowInTaskbar = false,
                SizeToContent = SizeToContent.WidthAndHeight,
                Title = "Warning!",
            };
            vm.CLoseEvent += (s, e) => win.Close();

            var lf = (IClassicDesktopStyleApplicationLifetime?)Application.Current?.ApplicationLifetime;
            Debug.Assert(lf is not null);
            Debug.Assert(lf.MainWindow is not null);

            await win.ShowDialog(lf.MainWindow);

            return vm.Result;
        }
    }
}
