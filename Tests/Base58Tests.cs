// WatchOnlyBitcoinWallet Tests
// Copyright (c) 2016 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using WatchOnlyBitcoinWallet.Services;

namespace Tests
{
    public class Base58Tests
    {
        [Fact]
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
                Assert.True(Base58.Verify(addr).IsVerified);
            }

            foreach (var addr in incorrectAddresses)
            {
                Assert.False(Base58.Verify(addr).IsVerified);
            }
        }

    }
}