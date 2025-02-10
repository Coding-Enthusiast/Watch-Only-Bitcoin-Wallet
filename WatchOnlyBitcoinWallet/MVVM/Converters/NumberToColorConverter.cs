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
    /// </para> positive : green
    /// </para> negative : red
    /// </para> zero : null
    /// </summary>
    public class NumberToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            decimal number = (decimal)value;
            if (number == 0)
            {
                return null;
            }
            else if (number >= 0)
            {
                return Brushes.LightGreen;
            }
            else
            {
                return Brushes.Orange;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
