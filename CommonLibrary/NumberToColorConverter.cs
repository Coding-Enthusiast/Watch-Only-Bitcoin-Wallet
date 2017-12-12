using System;
using System.Windows.Data;
using System.Windows.Media;

namespace MVVMLibrary
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
