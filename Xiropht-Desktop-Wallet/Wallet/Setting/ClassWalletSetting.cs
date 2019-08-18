using System;
using System.IO;
using Newtonsoft.Json;
using Xiropht_Wallet.Features;
using Xiropht_Wallet.Utility;
using Xiropht_Wallet.Wallet.Tcp;

namespace Xiropht_Wallet.Wallet.Setting
{
    public class ClassWalletSettingJson
    {
        public int wallet_sync_mode = 0;
        public string wallet_sync_manual_host = string.Empty;
        public string wallet_current_language = string.Empty;
    }

    public class ClassWalletSetting
    {
        private static readonly string _WalletSettingFile = "\\setting.json"; // Path of the setting file.
        private static readonly string _WalletOldSettingFile = "\\xiropht.ini";

        /// <summary>
        ///     Save settings of the wallet gui into a file.
        /// </summary>
        public static void SaveSetting()
        {
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + _WalletOldSettingFile))
            {
                File.Delete(AppDomain.CurrentDomain.BaseDirectory + _WalletOldSettingFile);
            }
            if (!File.Exists(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + _WalletSettingFile)))
                File.Create(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + _WalletSettingFile))
                    .Close();

            var walletSettingJsonObject = new ClassWalletSettingJson
            {
                wallet_current_language = ClassTranslation.CurrentLanguage,
                wallet_sync_mode = (int)Program.WalletXiropht.WalletSyncMode,
                wallet_sync_manual_host = Program.WalletXiropht.WalletSyncHostname
            };

            using (var writer =
                new StreamWriter(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + _WalletSettingFile),
                    false))
            {
                string data = JsonConvert.SerializeObject(walletSettingJsonObject, Formatting.Indented);
                writer.WriteLine(data);
            }
        }

        /// <summary>
        ///     Load setting file of the wallet gui.
        /// </summary>
        public static bool LoadSetting()
        {
            if (!File.Exists(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + _WalletSettingFile)))
            {
                SaveSetting();
                return true; // This is the first start of the wallet gui.
            }

            string jsonSetting = string.Empty;
            using (var reader =
                new StreamReader(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + _WalletSettingFile)))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    jsonSetting += line;
                }

            }

            try
            {
                var walletSettingJsonObject = JsonConvert.DeserializeObject<ClassWalletSettingJson>(jsonSetting);
                ClassTranslation.CurrentLanguage = walletSettingJsonObject.wallet_current_language;
                Program.WalletXiropht.WalletSyncHostname = walletSettingJsonObject.wallet_sync_manual_host;
                Program.WalletXiropht.WalletSyncMode = (ClassWalletSyncMode) walletSettingJsonObject.wallet_sync_mode;
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}