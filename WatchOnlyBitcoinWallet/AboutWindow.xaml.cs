// WatchOnlyBitcoinWallet
// Copyright (c) 2016 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Navigation;

namespace WatchOnlyBitcoinWallet
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        private const string DonationAddress = "1Q9swRQuwhTtjZZ2yguFWk7m7pszknkWyk";

        public AboutWindow()
        {
            InitializeComponent();
            DonationHyperLink.NavigateUri = new System.Uri(string.Format("bitcoin:{0}", DonationAddress));
            txtVersion.Text = string.Format("Version {0}", Assembly.GetExecutingAssembly().GetName().Version);
            txtDonate.Text = DonationAddress;
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
