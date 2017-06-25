using Newtonsoft.Json;
using System;
using System.IO;

namespace WatchOnlyBitcoinWallet.Services
{
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
                using (StreamReader st = File.OpenText(myFilePath))
                {
                    JsonSerializer ser = new JsonSerializer();
                    result = (T)ser.Deserialize(st, typeof(T));
                }

                return result;
            }
            else
            {
                return (T)Activator.CreateInstance(typeof(T));
            }
        }


        /// <summary>
        /// Writes data to disk.
        /// </summary>
        /// <typeparam name="T">Type of the data to save.</typeparam>
        /// <param name="dataToSave">Data to save.</param>
        /// <param name="f">Name of the folder to place the file.</param>
        /// <param name="fileName">Name of the file to save to.</param>
        public static void WriteFile<T>(T dataToSave, FileType f)
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

            if (!Directory.Exists(mainFolderPath))
            {
                Directory.CreateDirectory(mainFolderPath);
            }

            using (StreamWriter str = File.CreateText(myFilePath))
            {
                JsonSerializer ser = new JsonSerializer();
                ser.Serialize(str, dataToSave);
            }
        }

    }
}
