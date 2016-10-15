using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.ComponentModel;

namespace WatchOnlyBitcoinWallet
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        GridViewColumnHeader listViewSortCol = null;
        SortAdorner listViewSortAdorner = null;

        public MainWindow()
        {
            InitializeComponent();

            WalletData.Load();
            lvAddresses.ItemsSource = WalletData.BitAddList;
            lblLocalCurrStmbol.Content = WalletData.Settings.LocalCurrencySymbol;
            CalculateTotal();

            txtTotalB.Background = Brushes.White;
            btnSave.IsEnabled = false;
            headerSave.IsEnabled = false;

            var ver = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            txtVersion.Text = string.Format("Version {0}.{1}.{2}", ver.Major, ver.Minor, ver.Build);
        }

        /// <summary>
        /// Gets total bitcoin balance by adding all balances.
        /// And calculates value in Dollar and local currency.
        /// </summary>
        void CalculateTotal()
        {
            decimal tempBitcoin = decimal.Parse(txtTotalB.Text);
            decimal tempDollar = decimal.Parse(txtTotalD.Text);
            decimal tempLocanCurrency = decimal.Parse(txtTotalC.Text);
            decimal totalB = 0;
            foreach (var bitAd in WalletData.BitAddList)
            {
                totalB += bitAd.Balance;
            }
            decimal totalD = totalB * WalletData.Settings.BitcoinPriceInUSD;
            decimal totalC = totalD * WalletData.Settings.DollarPriceInLocalCurrency;
            txtTotalB.Text = string.Format("{0:0.00000000}", totalB);
            txtTotalD.Text = string.Format("{0:#,##0.00}", totalD);
            txtTotalC.Text = string.Format("{0:#,##0}", totalC);
            if (tempBitcoin == totalB)
            {
                txtTotalB.Background = Brushes.White;
                if (tempDollar != totalD)
                {
                    txtTotalD.ToolTip = string.Format("{0:#,##0.00}", totalD - tempDollar);
                    txtTotalC.ToolTip = string.Format("{0:#,##0}", totalC - tempLocanCurrency);
                }
            }
            else
            {
                if (tempBitcoin < totalB)
                {
                    txtTotalB.Background = Brushes.LightGreen;
                }
                else if (tempBitcoin > totalB)
                {
                    txtTotalB.Background = Brushes.Orange;
                }
                txtTotalB.ToolTip = string.Format("{0:0.00000000}", totalB - tempBitcoin);
                txtTotalD.ToolTip = string.Format("{0:#,##0.00}", totalD - tempDollar);
                txtTotalC.ToolTip = string.Format("{0:#,##0}", totalC - tempLocanCurrency);
            }
        }

        /// <summary>
        /// Used for sorting the ColumnHeaders.
        /// </summary>
        private void lvAddressesColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader column = (GridViewColumnHeader)sender;
            string sortBy = column.Tag.ToString();
            if (listViewSortCol != null)
            {
                AdornerLayer.GetAdornerLayer(listViewSortCol).Remove(listViewSortAdorner);
                lvAddresses.Items.SortDescriptions.Clear();
            }

            ListSortDirection newDir = ListSortDirection.Ascending;
            if (listViewSortCol == column && listViewSortAdorner.Direction == newDir)
            {
                newDir = ListSortDirection.Descending;
            }

            listViewSortCol = column;
            listViewSortAdorner = new SortAdorner(listViewSortCol, newDir);
            AdornerLayer.GetAdornerLayer(listViewSortCol).Add(listViewSortAdorner);
            lvAddresses.Items.SortDescriptions.Add(new SortDescription(sortBy, newDir));
        }

        /// <summary>
        /// Used for handling DoubleClick event,
        /// Which will open a new instance of AddressWindow to edit the double clicked address.
        /// </summary>
        private void lvAddresses_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lvAddresses.SelectedItem != null)
            {
                var myWin = new AddressWindow((BitcoinAddress)lvAddresses.SelectedItem);
                myWin.Owner = this;
                myWin.ShowDialog();
                if (myWin.IsChanged)
                {
                    lvAddresses.Items.Refresh();
                    btnSave.IsEnabled = true;
                }
            }
        }

        private async void GetBalances_Click(object sender, RoutedEventArgs e)
        {
            btnGetBalance.IsEnabled = false;
            double total = WalletData.BitAddList.Count;
            progressBar.Value = 0;
            bool balChanged = false;
            for (int i = 0; i < WalletData.BitAddList.Count; i++)
            {
                decimal tempBal = WalletData.BitAddList[i].Balance;
                await WalletData.GetBalance(WalletData.BitAddList[i]);
                if (balChanged != true && WalletData.BitAddList[i].Balance != tempBal)
                {
                    balChanged = true;
                }
                progressBar.Value = ((i + 1) / total) * 100;
            }
            if (balChanged)
            {
                btnSave.IsEnabled = true;
                headerSave.IsEnabled = true;
                lvAddresses.Items.Refresh();
                CalculateTotal();
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            WalletData.Save();
            btnSave.IsEnabled = false;
            headerSave.IsEnabled = false;
        }
        private void NewAddress_Click(object sender, RoutedEventArgs e)
        {
            var myWin = new AddressWindow();
            myWin.Owner = this;
            myWin.ShowDialog();
            if (myWin.IsChanged)
            {
                WalletData.BitAddList.Add(myWin.BitcoinAddress);
                lvAddresses.Items.Refresh();
                btnSave.IsEnabled = true;
                headerSave.IsEnabled = true;
                btnGetBalance.IsEnabled = true;
            }
        }
        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            Window myWin = new SettingsWindow();
            myWin.Owner = this;
            myWin.ShowDialog();
            CalculateTotal();
            lblLocalCurrStmbol.Content = WalletData.Settings.LocalCurrencySymbol;
        }
        private void About_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow myWin = new AboutWindow();
            myWin.Owner = this;
            myWin.ShowDialog();
        }
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
