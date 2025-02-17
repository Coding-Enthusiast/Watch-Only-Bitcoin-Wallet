// WatchOnlyBitcoinWallet
// Copyright (c) 2016 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using WatchOnlyBitcoinWallet.Models;

namespace WatchOnlyBitcoinWallet.Services
{
    public interface IFileManager
    {
        SettingsModel ReadSettingsFile();
        List<BitcoinAddress> ReadWalletFile();
        void WriteSettings(SettingsModel settings);
        void WriteWallet(List<BitcoinAddress> addresses);
    }


    public class FileManager : IFileManager
    {
        public FileManager()
        {
            mainDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                                   "Autarkysoft",
                                   "Watch Only Bitcoin Wallet");

            walletPath = Path.Combine(mainDir, WalletFileName);
            settingsPath = Path.Combine(mainDir, SettingsFileName);
        }


        private const string WalletFileName = "Wallet.json";
        private const string SettingsFileName = "Settings.json";

        private readonly string mainDir, walletPath, settingsPath;


        public static T? ReadFile<T>(string filePath)
        {
            if (File.Exists(filePath))
            {
                using StreamReader st = File.OpenText(filePath);
                JsonSerializer ser = new();
                var obj = ser.Deserialize(st, typeof(T));
                if (obj is not null)
                {
                    return (T?)obj;
                }
            }

            return default;
        }

        public SettingsModel ReadSettingsFile()
        {
            SettingsModel? result = ReadFile<SettingsModel>(settingsPath);
            return result ?? new SettingsModel();
        }

        public List<BitcoinAddress> ReadWalletFile()
        {
            List<BitcoinAddress>? result = ReadFile<List<BitcoinAddress>>(walletPath);
            return result ?? new List<BitcoinAddress>();
        }


        public void WriteFile<T>(T dataToSave, string filePath)
        {
            if (!Directory.Exists(mainDir))
            {
                Directory.CreateDirectory(mainDir);
            }

            using StreamWriter str = File.CreateText(filePath);
            JsonSerializer ser = new();
            ser.Serialize(str, dataToSave);
        }

        public void WriteSettings(SettingsModel settings)
        {
            WriteFile(settings, settingsPath);
        }

        public void WriteWallet(List<BitcoinAddress> addresses)
        {
            WriteFile(addresses, walletPath);
        }
    }
}
