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

namespace WatchOnlyBitcoinWallet
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<TextBox> balanceTextList = new List<TextBox>();
        public MainWindow()
        {
            InitializeComponent();
            WalletData.Load();
            foreach (var item in WalletData.BitAddList)
            {
                AddTextBox(item);
            }
            CalculateTotal();
            lblLocalCurrStmbol.Content = WalletData.Settings.LocalCurrencySymbol;
            btnSave.IsEnabled = false;
            headerSave.IsEnabled = false;
            txtTotalB.Background = Brushes.White;
            var ver = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            txtVersion.Text = string.Format("Version {0}.{1}", ver.Major,ver.Minor);
        }

        private void AddTextBox(BitcoinAddress bitAdd)
        {
            int spaceFromTop = 70;
            double minWinHeight = 333;
            int i = WalletData.BitAddList.IndexOf(bitAdd);

            TextBox tbName = new TextBox() { Width = 85, Height = 25, TextAlignment = TextAlignment.Center, Name = "txtName" + i };
            tbName.Margin = new Thickness(10, spaceFromTop + (i * 30), 0, 0);
            tbName.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            tbName.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            gridMain.Children.Add(tbName);

            TextBox tbAddress = new TextBox() { Width = 300, Height = 25, Name = "txtAddress" + i };
            tbAddress.Margin = new Thickness(100, spaceFromTop + (i * 30), 0, 0);
            tbAddress.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            tbAddress.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            gridMain.Children.Add(tbAddress);

            TextBox tbBalance = new TextBox() { Width = 100, Height = 25, IsReadOnly = true, Name = "txtBalance" + i };
            tbBalance.Width = 100;
            tbBalance.Margin = new Thickness(405, spaceFromTop + (i * 30), 0, 0);
            tbBalance.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            tbBalance.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            gridMain.Children.Add(tbBalance);

            double myWinHeight = ((i + 1) * (25 + 5)) + spaceFromTop + 50;
            windowMain.Height = (myWinHeight > minWinHeight) ? myWinHeight : minWinHeight;

            tbName.Text = bitAdd.Name;
            tbName.TextChanged += textbox_TextChanged;
            tbAddress.Text = bitAdd.Address;
            tbAddress.TextChanged += textbox_TextChanged;
            tbBalance.Text = string.Format("{0:0.00000000}", bitAdd.Balance);
            tbBalance.TextChanged += textbox_TextChanged;
            balanceTextList.Add(tbBalance);
        }
        private void textbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //i is the index of the textbox e.g. "txtName3"
            int i;
            TextBox tb = (TextBox)sender;
            if (tb.Name.Contains("txtName"))
            {
                i = int.Parse(tb.Name.Substring(7));
                WalletData.BitAddList[i].Name = tb.Text;
            }
            else if (tb.Name.Contains("txtAddress"))
            {
                i = int.Parse(tb.Name.Substring(10));
                WalletData.BitAddList[i].Address = tb.Text;
            }
            else if (tb.Name.Contains("txtBalance"))
            {
                i = int.Parse(tb.Name.Substring(10));
                WalletData.BitAddList[i].Balance = decimal.Parse(tb.Text);
            }
            btnSave.IsEnabled = true;
            headerSave.IsEnabled = true;
        }


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


        private void NewAddress_Click(object sender, RoutedEventArgs e)
        {
            var myAddress = new BitcoinAddress();
            WalletData.BitAddList.Add(myAddress);
            AddTextBox(myAddress);
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            WalletData.Save();
            btnSave.IsEnabled = false;
            headerSave.IsEnabled = false;
        }


        private async void GetBalances_Click(object sender, RoutedEventArgs e)
        {
            btnGetBalance.IsEnabled = false;
            double total = WalletData.BitAddList.Count;
            progressBar.Value = 0;
            for (int i = 0; i < WalletData.BitAddList.Count; i++)
            {
                decimal tempBalance = WalletData.BitAddList[i].Balance;
                balanceTextList[i].Background = Brushes.Yellow;
                await WalletData.GetBalance(WalletData.BitAddList[i]);
                if (WalletData.BitAddList[i].Balance > tempBalance)
                {
                    balanceTextList[i].Background = Brushes.LightGreen;
                    balanceTextList[i].Text = string.Format("{0:0.00000000}", WalletData.BitAddList[i].Balance);
                    balanceTextList[i].ToolTip = string.Format("{0:0.00000000}", WalletData.BitAddList[i].Balance - tempBalance);
                }
                else if (WalletData.BitAddList[i].Balance < tempBalance)
                {
                    balanceTextList[i].Background = Brushes.Orange;
                    balanceTextList[i].Text = string.Format("{0:0.00000000}", WalletData.BitAddList[i].Balance);
                    balanceTextList[i].ToolTip = string.Format("{0:0.00000000}", WalletData.BitAddList[i].Balance - tempBalance);
                }
                else if (WalletData.BitAddList[i].Balance == tempBalance)
                {
                    balanceTextList[i].Background = Brushes.White;
                }
                progressBar.Value = ((i + 1) / total) * 100;
            }
            CalculateTotal();
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
