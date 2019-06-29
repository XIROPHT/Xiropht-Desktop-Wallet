using System;
using System.IO;
using Xiropht_Wallet.FormPhase;
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

            switch (Program.WalletXiropht.ClassWalletObject.WalletSyncMode)
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
                    syncModeManualHostSetting += Program.WalletXiropht.ClassWalletObject.WalletSyncHostname;
                    break;
                default:
                    syncModeSetting += "0";
                    syncModeManualHostSetting += "NONE";
                    break;
            }

            if (!File.Exists(ClassUtility.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + _walletSettingFile)))
            {
                File.Create(ClassUtility.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + _walletSettingFile)).Close();
            }

            using (StreamWriter writer = new StreamWriter(ClassUtility.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + _walletSettingFile), false))
            {
                writer.WriteLine(syncModeSetting);
                writer.WriteLine(syncModeManualHostSetting);
                writer.WriteLine("CURRENT-WALLET-LANGUAGE=" + ClassTranslation.CurrentLanguage);
            }
        }

        /// <summary>
        /// Load setting file of the wallet gui.
        /// </summary>
        public static bool LoadSetting()
        {
            if (!File.Exists(ClassUtility.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + _walletSettingFile)))
            {
                File.Create(ClassUtility.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + _walletSettingFile)).Close();
                return true; // This is the first start of the wallet gui.
            }
            else
            {
                using (StreamReader reader = new StreamReader(ClassUtility.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + _walletSettingFile)))
                {
                    string line;
                    int counterLine = 0;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.Contains("SYNC-MODE-SETTING="))
                        {
                            if (line.Replace("SYNC-MODE-SETTING=", "") == "0")
                            {
                                Program.WalletXiropht.ClassWalletObject.WalletSyncMode = (int)ClassWalletSyncMode.WALLET_SYNC_DEFAULT;
                            }
                            else if (line.Replace("SYNC-MODE-SETTING=", "") == "1")
                            {
                                Program.WalletXiropht.ClassWalletObject.WalletSyncMode = (int)ClassWalletSyncMode.WALLET_SYNC_PUBLIC_NODE;
                            }
                            else if (line.Replace("SYNC-MODE-SETTING=", "") == "2")
                            {
                                Program.WalletXiropht.ClassWalletObject.WalletSyncMode = (int)ClassWalletSyncMode.WALLET_SYNC_MANUAL_NODE;
                            }
                        }
                        else if (line.Contains("SYNC-MODE-MANUAL-HOST-SETTING="))
                        {
                            Program.WalletXiropht.ClassWalletObject.WalletSyncHostname = line.Replace("SYNC-MODE-MANUAL-HOST-SETTING=", "");
                        }
                        else if (line.Contains("CURRENT-WALLET-LANGUAGE="))
                        {
                            ClassTranslation.CurrentLanguage = line.Replace("CURRENT-WALLET-LANGUAGE=", "").ToLower();
                        }
                        counterLine++;
                    }
                    if (counterLine == 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}