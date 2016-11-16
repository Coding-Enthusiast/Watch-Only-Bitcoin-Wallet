using System.Windows;


namespace WatchOnlyBitcoinWallet
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var ver = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            txtVersion.Text = string.Format("Version {0}.{1}.{2}", ver.Major, ver.Minor, ver.Build);
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
