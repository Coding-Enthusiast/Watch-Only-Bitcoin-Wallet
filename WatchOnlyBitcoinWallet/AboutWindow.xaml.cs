using System.Windows;

using System.Diagnostics;

namespace WatchOnlyBitcoinWallet
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
            DonationHyperLink.NavigateUri = new System.Uri(string.Format("bitcoin:{0}", WalletData.DonationAddress));
            txtVersion.Text = string.Format("Version {0}", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
