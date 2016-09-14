using System.Windows;
using System.Windows.Controls;

namespace WatchOnlyBitcoinWallet
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            txtBitcoinPriceInUSD.Text = WalletData.Settings.BitcoinPriceInUSD.ToString();
            txtDollarPriceInLocalCurrency.Text = WalletData.Settings.DollarPriceInLocalCurrency.ToString();
            txtLocalCurrencySymbol.Text = WalletData.Settings.LocalCurrencySymbol;
            btnSave.IsEnabled = false;
        }

        private void txt_TextChanged(object sender, TextChangedEventArgs e)
        {
            btnSave.IsEnabled = true;
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            WalletData.Settings.BitcoinPriceInUSD = decimal.Parse(txtBitcoinPriceInUSD.Text);
            WalletData.Settings.DollarPriceInLocalCurrency = decimal.Parse(txtDollarPriceInLocalCurrency.Text);
            WalletData.Settings.LocalCurrencySymbol = txtLocalCurrencySymbol.Text;
            WalletData.SaveSettings();
            this.Close();
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
