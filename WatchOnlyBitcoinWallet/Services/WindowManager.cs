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
    }

    public class WindowManager : IWindowManager
    {
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
}
