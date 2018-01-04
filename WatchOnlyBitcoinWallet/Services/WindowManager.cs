using MVVMLibrary;
using System.Windows;
using WatchOnlyBitcoinWallet.Views;

namespace WatchOnlyBitcoinWallet.Services
{
    public interface IWindowManager
    {
        void Show(CommonBase ViewModel);
    }

    public class SettingsWindowManager : IWindowManager
    {
        public void Show(CommonBase ViewModel)
        {
            SettingsWindow myWin = new SettingsWindow();
            myWin.DataContext = ViewModel;
            myWin.Owner = Application.Current.MainWindow;
            myWin.ShowDialog();
        }
    }

    public class ForkBalanceWindowManager : IWindowManager
    {
        public void Show(CommonBase ViewModel)
        {
            ForkBalanceWindow myWin = new ForkBalanceWindow();
            myWin.DataContext = ViewModel;
            myWin.Owner = Application.Current.MainWindow;
            myWin.ShowDialog();
        }
    }
}