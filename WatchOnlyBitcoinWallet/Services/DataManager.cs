// WatchOnlyBitcoinWallet
// Copyright (c) 2016 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using Newtonsoft.Json;
using System;
using System.IO;

namespace WatchOnlyBitcoinWallet.Services
{
    [Obsolete]
    public static class DataManager
    {
        static DataManager()
        {
            mainFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\C.E. Watch Only Bitcoin Wallet";
        }


        private static readonly string mainFolderPath;


        public enum FileType
        {
            Wallet,
            Settings,
        }

        /// <summary>
        /// Reads file from disk. Returns a new instance (empty) if file not found.
        /// </summary>
        /// <typeparam name="T">Type of the file being read.</typeparam>
        /// <param name="f">Name of the folder containing file to read.</param>
        /// <param name="fileName">Name of the file to read.</param>
        /// <returns>Read data.</returns>
        public static T ReadFile<T>(FileType f)
        {
            string myFilePath = string.Empty;
            switch (f)
            {
                case FileType.Wallet:
                    myFilePath = mainFolderPath + @"\Wallet.json";
                    break;
                case FileType.Settings:
                    myFilePath = mainFolderPath + @"\Settings.json";
                    break;
            }
            T result;
            if (File.Exists(myFilePath))
            {
                using StreamReader st = File.OpenText(myFilePath);
                JsonSerializer ser = new();
                result = (T)ser.Deserialize(st, typeof(T));

                return result;
            }
            else
            {
                return (T)Activator.CreateInstance(typeof(T));
            }
        }
    }
}
