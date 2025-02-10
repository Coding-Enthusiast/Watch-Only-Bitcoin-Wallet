﻿// WatchOnlyBitcoinWallet Tests
// Copyright (c) 2016 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using WatchOnlyBitcoinWallet.Services;

namespace Tests
{
    public class SegWitAddressTests
    {
        [Fact]
        public void VerifyTest()
        {
            // Examples are from https://github.com/bitcoin/bips/blob/master/bip-0173.mediawiki
            string[] correctAddresses =
            {
                "BC1QW508D6QEJXTDG4Y5R3ZARVARY0C5XW7KV8F3T4",
                "bc1pw508d6qejxtdg4y5r3zarvary0c5xw7kw508d6qejxtdg4y5r3zarvary0c5xw7k7grplx",
                "BC1SW50QA3JX3S",
                "bc1zw508d6qejxtdg4y5r3zarvaryvg6kdaj"
            };
            string[] incorrectAddresses =
            {
                "bc1qw508d6qejxtdg4y5r3zarvary0c5xw7kv8f3t5",
                "BC13W508D6QEJXTDG4Y5R3ZARVARY0C5XW7KN40WF2",
                "bc1rw5uspcuh",
                "bc10w508d6qejxtdg4y5r3zarvary0c5xw7kw508d6qejxtdg4y5r3zarvary0c5xw7kw5rljs90",
                "BC1QR508D6QEJXTDG4Y5R3ZARVARYV98GJ9P",
                "bc1zw508d6qejxtdg4y5r3zarvaryvqyzf3du",
                "bc1gmk9yu"
            };

            foreach (var addr in correctAddresses)
            {
                Assert.True(SegWitAddress.Verify(addr, SegWitAddress.NetworkType.MainNet).IsVerified);
            }

            foreach (var addr in incorrectAddresses)
            {
                Assert.False(SegWitAddress.Verify(addr, SegWitAddress.NetworkType.MainNet).IsVerified);
            }
        }

    }
}
