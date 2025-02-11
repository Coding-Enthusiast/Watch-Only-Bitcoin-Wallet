// WatchOnlyBitcoinWallet
// Copyright (c) 2016 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.Collections.Generic;
using System.Linq;

namespace WatchOnlyBitcoinWallet.MVVM
{
    public static class EnumHelper
    {
        public static IEnumerable<T> GetAllEnumValues<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        public static IEnumerable<T> GetEnumValues<T>(params T[] exclude) where T : Enum
        {
            foreach (T item in Enum.GetValues(typeof(T)))
            {
                if (exclude != null && !exclude.Contains(item))
                {
                    yield return item;
                }
            }
        }
    }
}
