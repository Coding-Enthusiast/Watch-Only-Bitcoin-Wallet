using BitcoinLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitcoinLibraryTests
{
    [TestClass]
    public class Base58Tests
    {
        [TestMethod]
        public void TestMethod1()
        {
            // Examples are from https://en.bitcoin.it/wiki/Address
            string[] correctAddresses = 
            { 
                "1BvBMSEYstWetqTFn5Au4m4GFg7xJaNVN2", 
                "3J98t1WpEZ73CNmQviecrnyiWrnqRhWNLy" 
            };
            string[] incorrectAddresses = 
            { 
                "1BvBMSEYStWetqTFn5Au4m4GFg7xJaNVN2", 
                "3J92t1WpEZ73CNmQviecrnyiWrnqRhWNLy", 
                "5BvBMSEYStWetqTFn5Au4m4GFg7xJaNVN2", 
                " ", 
                "" 
            };

            foreach (var addr in correctAddresses)
            {
                Assert.AreEqual(true, Base58.Verify(addr).IsVerified);
            }

            foreach (var addr in incorrectAddresses)
            {
                Assert.AreEqual(false, Base58.Verify(addr).IsVerified);
            }
        }

    }
}
