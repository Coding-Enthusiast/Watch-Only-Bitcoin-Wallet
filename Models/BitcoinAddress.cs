using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class BitcoinAddress
    {
        /// <summary>
        /// Name acts as a tag for the address
        /// </summary>
        public string Name { get; set; }
        public string Address { get; set; }
        public decimal Balance { get; set; }

        public bool Validate()
        {
            //Check Base58

            return true;
        }
    }
}
