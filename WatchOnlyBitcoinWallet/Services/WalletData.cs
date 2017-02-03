using System;
using System.Collections.Generic;

using System.IO;
using System.Xml.Serialization;
using WatchOnlyBitcoinWallet.Models;

namespace WatchOnlyBitcoinWallet.Services
{
    public class WalletData
    {
        private static string WalletFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\C.E. Watch Only Bitcoin Wallet";
        private static string WalletFilePath = WalletFolderPath + @"\BitcoinBalance - Info.txt";
        private static string SettingFilePath = WalletFolderPath + @"\BitcoinBalance - Settings.txt";


        public static List<BitcoinAddress> LoadAddresses()
        {
            if (!File.Exists(WalletFilePath))
            {
                if (!Directory.Exists(WalletFolderPath))
                {
                    Directory.CreateDirectory(WalletFolderPath);
                }
                List<BitcoinAddress> emptyList = new List<BitcoinAddress>();
                Save(emptyList);
                return emptyList;
            }
            else
            {
                XmlSerializer ser = new XmlSerializer(typeof(List<BitcoinAddress>));
                using (StreamReader st = new StreamReader(WalletFilePath))
                {
                    return (List<BitcoinAddress>)ser.Deserialize(st);
                }
            }
        }
        public static void Save(List<BitcoinAddress> BitAddList)
        {
            XmlSerializer ser = new XmlSerializer(typeof(List<BitcoinAddress>));
            using (Stream str = File.Create(WalletFilePath))
            {
                ser.Serialize(str, BitAddList);
            }
        }

        public static SettingsModel LoadSettings()
        {
            if (!File.Exists(SettingFilePath))
            {
                SettingsModel emptySettings = new SettingsModel();
                SaveSettings(emptySettings);
                return emptySettings;
            }
            else
            {
                XmlSerializer ser = new XmlSerializer(typeof(SettingsModel));
                using (StreamReader st = new StreamReader(SettingFilePath))
                {
                    return (SettingsModel)ser.Deserialize(st);
                }
            }
        }
        public static void SaveSettings(SettingsModel settings)
        {
            XmlSerializer ser = new XmlSerializer(typeof(SettingsModel));
            using (Stream str = File.Create(SettingFilePath))
            {
                ser.Serialize(str, settings);
            }
        }
    }
}
