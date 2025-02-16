// WatchOnlyBitcoinWallet
// Copyright (c) 2016 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using Avalonia.Input.Platform;
using WatchOnlyBitcoinWallet.MVVM;

namespace WatchOnlyBitcoinWallet.ViewModels
{
    public class AboutViewModel : ViewModelBase
    {
        /// <summary>
        /// Make designer happy!
        /// </summary>
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        public AboutViewModel() : this("(Version 1.2.3)", null)
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        {
        }

        public AboutViewModel(string ver, IClipboard clipboard)
        {
            VersionString = ver;
            this.clipboard = clipboard;

            CopyCommand = new BindableCommand<string>(Copy);
        }


        private readonly IClipboard clipboard;
        public string VersionString { get; }
        public string Address1 => "1Q9swRQuwhTtjZZ2yguFWk7m7pszknkWyk";
        public string Address2 => "bc1q3n5t9gv40ayq68nwf0yth49dt5c799wpld376s";
        public string DonateUri1 => $"bitcoin:{Address1}{Bip21Extras}";
        public string DonateUri2 => $"bitcoin:{Address2}{Bip21Extras}";

        private const string Bip21Extras = "?label=Coding-Enthusiast&message=Donation%20to%20WatchOnlyBitcoinWallet%20project";

        public BindableCommand<string> CopyCommand { get; }
        private void Copy(string s)
        {
            clipboard?.SetTextAsync(s);
        }
    }
}
