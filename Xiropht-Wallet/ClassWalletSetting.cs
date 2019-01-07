using System.IO;
using Xiropht_Wallet.Wallet;

namespace Xiropht_Wallet
{
    public class ClassWalletSetting
    {
        private static string _walletSettingFile = "\\xiropht.ini"; // Path of the setting file.

        /// <summary>
        /// Save settings of the wallet gui into a file.
        /// </summary>
        public static void SaveSetting()
        {
            string syncModeSetting = "SYNC-MODE-SETTING=";
            string syncModeManualHostSetting = "SYNC-MODE-MANUAL-HOST-SETTING=";

            switch (ClassWalletObject.WalletSyncMode)
            {
                case 0:
                    syncModeSetting += "0";
                    syncModeManualHostSetting += "NONE";
                    break;
                case 1:
                    syncModeSetting += "1";
                    syncModeManualHostSetting += "NONE";
                    break;
                case 2:
                    syncModeSetting += "2";
                    syncModeManualHostSetting += ClassWalletObject.WalletSyncHostname;
                    break;
                default:
                    syncModeSetting += "0";
                    syncModeManualHostSetting += "NONE";
                    break;
            }

            if (!File.Exists(Directory.GetCurrentDirectory() + _walletSettingFile))
            {
                File.Create(Directory.GetCurrentDirectory() + _walletSettingFile).Close();
            }

            StreamWriter writer = new StreamWriter(Directory.GetCurrentDirectory() + _walletSettingFile, false);
            writer.WriteLine(syncModeSetting);
            writer.WriteLine(syncModeManualHostSetting);
            writer.WriteLine("CURRENT-WALLET-LANGUAGE="+ClassTranslation.CurrentLanguage);
            writer.Close();
        }

        /// <summary>
        /// Load setting file of the wallet gui.
        /// </summary>
        public static void LoadSetting()
        {
            if (!File.Exists(Directory.GetCurrentDirectory() + _walletSettingFile))
            {
                File.Create(Directory.GetCurrentDirectory() + _walletSettingFile).Close();
            }
            else
            {
                StreamReader reader = new StreamReader(Directory.GetCurrentDirectory() + _walletSettingFile);
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Contains("SYNC-MODE-SETTING="))
                    {
                        if (line.Replace("SYNC-MODE-SETTING=", "") == "0")
                        {
                            ClassWalletObject.WalletSyncMode = 0;
                        }
                        else if (line.Replace("SYNC-MODE-SETTING=", "") == "1")
                        {
                            ClassWalletObject.WalletSyncMode = 1;
                        }
                        else if (line.Replace("SYNC-MODE-SETTING=", "") == "2")
                        {
                            ClassWalletObject.WalletSyncMode = 2;
                        }
                    }
                    else if (line.Contains("SYNC-MODE-MANUAL-HOST-SETTING="))
                    {
                        ClassWalletObject.WalletSyncHostname = line.Replace("SYNC-MODE-MANUAL-HOST-SETTING=", "");
                    }
                    else if (line.Contains("CURRENT-WALLET-LANGUAGE="))
                    {
                        ClassTranslation.CurrentLanguage = line.Replace("CURRENT-WALLET-LANGUAGE=", "").ToLower();
                    }
                }
            }
        }
    }
}