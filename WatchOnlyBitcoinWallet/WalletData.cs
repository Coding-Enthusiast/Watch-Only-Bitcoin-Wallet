using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using System.IO;
using System.Xml.Serialization;
using System.Net.Http;
using Newtonsoft.Json;

namespace WatchOnlyBitcoinWallet
{
    public class WalletData
    {
        private static string myFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\C.E. Watch Only Bitcoin Wallet";
        private static string myFilePath = myFolderPath + @"\BitcoinBalance - Info.txt";
        private static string mySettingPath = myFolderPath + @"\BitcoinBalance - Settings.txt";
        public static List<BitcoinAddress> BitAddList = new List<BitcoinAddress>();
        public static SettingsClass Settings = new SettingsClass();
        public static string DonationAddress = "1Q9swRQuwhTtjZZ2yguFWk7m7pszknkWyk";

        public static void Load()
        {
            if (!File.Exists(myFilePath))
            {
                if (!Directory.Exists(myFolderPath))
                {
                    Directory.CreateDirectory(myFolderPath);
                }
                BitAddList.Add(new BitcoinAddress() { Name = "Donations", Address = DonationAddress });
                Save();
            }
            else
            {
                XmlSerializer ser = new XmlSerializer(typeof(List<BitcoinAddress>));
                using (StreamReader st = new StreamReader(myFilePath))
                {
                    BitAddList = (List<BitcoinAddress>)ser.Deserialize(st);
                }
            }

            if (!File.Exists(mySettingPath))
            {
                Settings.BitcoinPriceInUSD = 600;
                Settings.DollarPriceInLocalCurrency = 0;
                SaveSettings();
            }
            else
            {
                XmlSerializer ser2 = new XmlSerializer(typeof(SettingsClass));
                using (StreamReader st = new StreamReader(mySettingPath))
                {
                    Settings = (SettingsClass)ser2.Deserialize(st);
                }
            }
        }
        public static void Save()
        {
            XmlSerializer ser = new XmlSerializer(typeof(List<BitcoinAddress>));
            using (Stream str = File.Create(myFilePath))
            {
                ser.Serialize(str, BitAddList);
            }
        }
        public static void SaveSettings()
        {
            XmlSerializer ser = new XmlSerializer(typeof(SettingsClass));
            using (Stream str = File.Create(mySettingPath))
            {
                ser.Serialize(str, Settings);
            }
        }


        public static async Task GetBalance(BitcoinAddress BTC)
        {
            using (var client = new HttpClient())
            {
                string url = "https://blockchain.info/address/" + BTC.Address + "?format=json&limit=0";
                try
                {
                    BlockchainInfoAPI BitcoinInfo = JsonConvert.DeserializeObject<BlockchainInfoAPI>(await client.GetStringAsync(url));
                    decimal balance = BitcoinInfo.final_balance * 0.00000001m;
                    BTC.Balance = balance;
                }
                catch (Exception)
                {
                    BTC.Balance = 0;
                }
            }
        }
    }
}
