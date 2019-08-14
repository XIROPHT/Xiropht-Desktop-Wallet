using System;
using System.IO;
using Xiropht_Wallet.Features;
using Xiropht_Wallet.Utility;
using Xiropht_Wallet.Wallet.Tcp;

namespace Xiropht_Wallet.Wallet.Setting
{
    public class ClassWalletSetting
    {
        private static readonly string _walletSettingFile = "\\xiropht.ini"; // Path of the setting file.

        /// <summary>
        ///     Save settings of the wallet gui into a file.
        /// </summary>
        public static void SaveSetting()
        {
            var syncModeSetting = "SYNC-MODE-SETTING=";
            var syncModeManualHostSetting = "SYNC-MODE-MANUAL-HOST-SETTING=";

            switch (Program.WalletXiropht.WalletSyncMode)
            {
                case ClassWalletSyncMode.WALLET_SYNC_DEFAULT:
                    syncModeSetting += "0";
                    syncModeManualHostSetting += "NONE";
                    break;
                case ClassWalletSyncMode.WALLET_SYNC_PUBLIC_NODE:
                    syncModeSetting += "1";
                    syncModeManualHostSetting += "NONE";
                    break;
                case ClassWalletSyncMode.WALLET_SYNC_MANUAL_NODE:
                    syncModeSetting += "2";
                    syncModeManualHostSetting += Program.WalletXiropht.WalletSyncHostname;
                    break;
                default:
                    syncModeSetting += "0";
                    syncModeManualHostSetting += "NONE";
                    break;
            }

            if (!File.Exists(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + _walletSettingFile)))
                File.Create(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + _walletSettingFile))
                    .Close();

            using (var writer =
                new StreamWriter(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + _walletSettingFile),
                    false))
            {
                writer.WriteLine(syncModeSetting);
                writer.WriteLine(syncModeManualHostSetting);
                writer.WriteLine("CURRENT-WALLET-LANGUAGE=" + ClassTranslation.CurrentLanguage);
            }
        }

        /// <summary>
        ///     Load setting file of the wallet gui.
        /// </summary>
        public static bool LoadSetting()
        {
            if (!File.Exists(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + _walletSettingFile)))
            {
                File.Create(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + _walletSettingFile))
                    .Close();
                return true; // This is the first start of the wallet gui.
            }

            using (var reader =
                new StreamReader(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + _walletSettingFile)))
            {
                string line;
                var counterLine = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Contains("SYNC-MODE-SETTING="))
                    {
                        if (line.Replace("SYNC-MODE-SETTING=", "") == "0")
                            Program.WalletXiropht.WalletSyncMode = ClassWalletSyncMode.WALLET_SYNC_DEFAULT;
                        else if (line.Replace("SYNC-MODE-SETTING=", "") == "1")
                            Program.WalletXiropht.WalletSyncMode = ClassWalletSyncMode.WALLET_SYNC_PUBLIC_NODE;
                        else if (line.Replace("SYNC-MODE-SETTING=", "") == "2")
                            Program.WalletXiropht.WalletSyncMode = ClassWalletSyncMode.WALLET_SYNC_MANUAL_NODE;
                    }
                    else if (line.Contains("SYNC-MODE-MANUAL-HOST-SETTING="))
                    {
                        Program.WalletXiropht.WalletSyncHostname = line.Replace("SYNC-MODE-MANUAL-HOST-SETTING=", "");
                    }
                    else if (line.Contains("CURRENT-WALLET-LANGUAGE="))
                    {
                        ClassTranslation.CurrentLanguage = line.Replace("CURRENT-WALLET-LANGUAGE=", "").ToLower();
                    }

                    counterLine++;
                }

                if (counterLine == 0) return true;
            }

            return false;
        }
    }
}