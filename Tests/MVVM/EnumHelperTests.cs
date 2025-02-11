// WatchOnlyBitcoinWallet Tests
// Copyright (c) 2016 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using WatchOnlyBitcoinWallet.MVVM;

namespace Tests.MVVM
{
    public class EnumHelperTests
    {
        public enum Foo
        {
            Foo1,
            Foo2,
            Foo3,
            Foo4,
        }

        [Fact]
        public void GetAllEnumValuesTest()
        {
            IEnumerable<Foo> actual = EnumHelper.GetAllEnumValues<Foo>();
            IEnumerable<Foo> expected = new Foo[] { Foo.Foo1, Foo.Foo2, Foo.Foo3, Foo.Foo4 };
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetEnumValues_All_Test()
        {
            IEnumerable<Foo> actual = EnumHelper.GetEnumValues<Foo>();
            IEnumerable<Foo> expected = new Foo[] { Foo.Foo1, Foo.Foo2, Foo.Foo3, Foo.Foo4 };
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetEnumValues_Exclude_Test()
        {
            IEnumerable<Foo> actual = EnumHelper.GetEnumValues(Foo.Foo2, Foo.Foo2, Foo.Foo4);
            IEnumerable<Foo> expected = new Foo[] { Foo.Foo1, Foo.Foo3 };
            Assert.Equal(expected, actual);
        }
    }
}
