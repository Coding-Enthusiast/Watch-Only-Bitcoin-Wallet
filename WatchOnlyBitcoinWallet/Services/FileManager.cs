// WatchOnlyBitcoinWallet
// Copyright (c) 2016 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using Avalonia.Platform.Storage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WatchOnlyBitcoinWallet.Models;

namespace WatchOnlyBitcoinWallet.Services
{
    public interface IFileManager
    {
        public Task<string[]> OpenFilePickerAsync();
        public IStorageProvider? StorageProvider { get; set; }

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

        public IWindowManager? WinMan { get; set; }
        public IStorageProvider? StorageProvider { get; set; }


        public async Task<string[]> OpenFilePickerAsync()
        {
            if (WinMan is null)
            {
                return ["WindowManager instance is not set (this is a bug)."];
            }
            if (StorageProvider is null)
            {
                await WinMan.ShowMessageBox(MessageBoxType.Ok, "StorageProvider is not set (this is a bug).");
                return Array.Empty<string>();
            }

            FilePickerFileType fileType = new("txt")
            {
                Patterns = ["*.txt"]
            };

            FilePickerOpenOptions options = new()
            {
                AllowMultiple = false,
                FileTypeFilter = [fileType],
                Title = "Text files (.txt)"
            };

            try
            {
                IReadOnlyList<IStorageFile> dir = await StorageProvider.OpenFilePickerAsync(options);
                if (dir != null && dir.Count > 0)
                {
                    return File.ReadAllLines(dir.ElementAt(0).Path.LocalPath);
                }
            }
            catch (Exception ex)
            {
                await WinMan.ShowMessageBox(MessageBoxType.Ok, ex.Message);
            }
            return Array.Empty<string>();
        }

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
