// WatchOnlyBitcoinWallet Tests
// Copyright (c) 2016 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using Avalonia.Media;
using System.Globalization;
using WatchOnlyBitcoinWallet.MVVM.Converters;

namespace Tests.MVVM.Converters
{
    public class NumberToColorConverterTests
    {
        public static TheoryData<decimal, IImmutableSolidColorBrush> GetConvertCases()
        {
            TheoryData<decimal, IImmutableSolidColorBrush> result = new()
            {
                { 0, Brushes.Transparent },
                { 0.01m, Brushes.LightGreen },
                { -0.002m, Brushes.Orange },
            };

            return result;
        }
        [Theory]
        [MemberData(nameof(GetConvertCases))]
        public void ConvertTest(decimal value, IImmutableSolidColorBrush expected)
        {
            NumberToColorConverter converter = new();
            object actual = converter.Convert(value, typeof(decimal), null, CultureInfo.InvariantCulture);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ExceptionTests()
        {
            NumberToColorConverter converter = new();
            Type t = typeof(decimal);
            CultureInfo ci = CultureInfo.InvariantCulture;

            Assert.Throws<NotImplementedException>(() => converter.Convert(null, t, null, ci));
            Assert.Throws<NotImplementedException>(() => converter.Convert("foo", t, null, ci));
            Assert.Throws<NotImplementedException>(() => converter.Convert(123, t, null, ci));
            Assert.Throws<NotImplementedException>(() => converter.ConvertBack(null, t, null, ci));
        }
    }
}
