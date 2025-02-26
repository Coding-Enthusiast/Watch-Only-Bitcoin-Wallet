// WatchOnlyBitcoinWallet
// Copyright (c) 2016 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using System.Diagnostics;
using WatchOnlyBitcoinWallet.ViewModels;

namespace WatchOnlyBitcoinWallet
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                MainWindowViewModel vm = new();
                desktop.MainWindow = new MainWindow
                {
                    DataContext = vm
                };

                Debug.Assert(desktop.MainWindow.Clipboard is not null);
                vm.Clipboard = desktop.MainWindow.Clipboard;
                vm.FileMan.StorageProvider = desktop.MainWindow.StorageProvider;
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
