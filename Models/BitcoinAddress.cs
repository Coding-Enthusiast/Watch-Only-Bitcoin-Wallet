using CommonLibrary;

namespace Models
{
    public class BitcoinAddress : ValidatableBase
    {
        /// <summary>
        /// Name acts as a tag for the address
        /// </summary>
        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    RaisePropertyChanged("Name");
                }
            }
        }

        private string address;
        public string Address
        {
            get { return address; }
            set
            {
                if (address != value)
                {
                    address = value;
                    // Check to see if input is a valid bitcoin address
                    Validate(value);
                    RaisePropertyChanged("Address");
                }
            }
        }

        private decimal balance;
        public decimal Balance
        {
            get { return balance; }
            set
            {
                if (balance != value)
                {
                    balance = value;
                    RaisePropertyChanged("Balance");
                }
            }
        }
    }
}
