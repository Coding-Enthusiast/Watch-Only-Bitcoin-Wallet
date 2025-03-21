﻿// WatchOnlyBitcoinWallet
// Copyright (c) 2016 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using System.Collections.Generic;
using System.IO;

namespace WatchOnlyBitcoinWallet.Services
{
    public static class StringExtension
    {
        /// <summary>
        /// Splits an input string on new lines.
        /// </summary>
        /// <param name="input">The string to split</param>
        /// <returns></returns>
        public static IEnumerable<string> SplitToLines(this string input)
        {
            if (input == null)
            {
                yield break;
            }

            using (StringReader reader = new StringReader(input))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }
    }
}
