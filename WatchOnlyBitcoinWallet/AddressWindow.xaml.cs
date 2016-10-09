using System.Windows;
using System.Windows.Controls;

namespace WatchOnlyBitcoinWallet
{
    /// <summary>
    /// Interaction logic for AddressWindow.xaml
    /// </summary>
    public partial class AddressWindow : Window
    {
        public bool IsChanged = false;
        public BitcoinAddress addr = new BitcoinAddress();

        public AddressWindow()
        {
            InitializeComponent();
        }
        public AddressWindow(BitcoinAddress _addr)
        {
            InitializeComponent();
            txtName.Text = _addr.Name;
            txtAddress.Text = _addr.Address;
            addr = _addr;
            btnSave.IsEnabled = false;
        }

        private void txt_TextChanged(object sender, TextChangedEventArgs e)
        {
            btnSave.IsEnabled = true;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (txtAddress.Text != "")
            {
                addr.Name = txtName.Text;
                addr.Address = txtAddress.Text;
                IsChanged = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Address can not be empty", "Error!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
