using System.Windows;
using System.Windows.Controls;

namespace WatchOnlyBitcoinWallet
{
    /// <summary>
    /// Interaction logic for AddressWindow.xaml
    /// </summary>
    public partial class AddressWindow : Window
    {
        /// <summary>
        /// Indicates whether or not anything was changed with BitcoinAddress
        /// in order to update GUI accordingly.
        /// </summary>
        public bool IsChanged = false;
        public BitcoinAddress BitcoinAddress = new BitcoinAddress();

        public AddressWindow()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Used to construct a new window for editing 
        /// a selected bitcoin address's attributes. 
        /// </summary>
        /// <param name="addr">Bitcoin Address to edit.</param>
        public AddressWindow(BitcoinAddress addr)
        {
            InitializeComponent();

            txtName.Text = addr.Name;
            txtAddress.Text = addr.Address;
            this.BitcoinAddress = addr;
            btnSave.IsEnabled = false;
        }

        private void TextBoxes_TextChanged(object sender, TextChangedEventArgs e)
        {
            btnSave.IsEnabled = true;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (txtAddress.Text != string.Empty)//Should check Base58 here that will cover "Empty", "white space" or "wrong BTC address"
            {
                BitcoinAddress.Name = txtName.Text;
                BitcoinAddress.Address = txtAddress.Text;
                IsChanged = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Please enter a valid Bitcoin Address.", "Error!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
