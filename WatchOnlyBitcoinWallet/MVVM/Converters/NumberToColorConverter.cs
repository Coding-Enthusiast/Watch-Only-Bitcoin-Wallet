// WatchOnlyBitcoinWallet
// Copyright (c) 2016 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using Avalonia.Data.Converters;
using Avalonia.Media;
using System;

namespace WatchOnlyBitcoinWallet.MVVM.Converters
{
    /// <summary>
    /// Converts a decimal number to color.
    /// <para/> positive : green
    /// <para/> negative : red
    /// <para/> zero : transparent
    /// </summary>
    public class NumberToColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        {
            if (value is decimal d)
            {
                switch (d)
                {
                    case 0:
                        return Brushes.Transparent;
                    case > 0:
                        return Brushes.LightGreen;
                    case < 0:
                        return Brushes.Orange;
                }
            }

            throw new NotImplementedException();
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
