using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Xiropht_Connector_All.Setting;
using Xiropht_Wallet.FormPhase;

namespace Xiropht_Wallet
{

    public class ClassTranslation
    {
        public static string CurrentLanguage;
        public const string LanguageFolderName = "\\Language\\";

        /// <summary>
        /// List of command orders to replace when it's possible.
        /// </summary>
        public const string CoinNameOrder = "%CoinName";
        public const string CoinMinNameOrder = "%CoinMinName";
        public const string AmountSendOrder = "%AmountSend";
        public const string TargetAddressOrder = "%TargetWallet";
        public const string DateOrder = "%Date";

        public static Dictionary<string, List<string>> LanguageContributors = new Dictionary<string, List<string>>();
        public static Dictionary<string, Dictionary<string, string>> LanguageDatabases = new Dictionary<string, Dictionary<string, string>>(); // Dictionnary content format -> {string:Language Name|Dictionnary:{string:text name|string:text content}}

        /// <summary>
        /// Read every language files, insert them to language database.
        /// </summary>
        public static void InitializationLanguage()
        {
            if (CurrentLanguage == null || string.IsNullOrEmpty(CurrentLanguage))
            {
                CurrentLanguage = "english"; // By Default on initialization.
            }
            if (Directory.Exists(ClassUtility.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + LanguageFolderName)))
            {
                string[] languageFilesList = Directory.GetFiles(ClassUtility.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + LanguageFolderName), "*.xirlang").Select(Path.GetFileName).ToArray();
                if (languageFilesList.Length == 0)
                {
#if DEBUG
                    Log.WriteLine("No language files found, please reinstall your gui wallet.");
#endif
#if WINDOWS
                    ClassFormPhase.MessageBoxInterface( "No language files found, please reinstall your gui wallet.", "No language files found.", MessageBoxButtons.OK, MessageBoxIcon.Error);
#else
                    MessageBox.Show(ClassFormPhase.WalletXiropht, "No language files found, please reinstall your gui wallet.", "No language files found.", MessageBoxButtons.OK, MessageBoxIcon.Error);
#endif
                    Process.GetCurrentProcess().Kill();
                }
                else
                {
                    for (int i = 0; i < languageFilesList.Length; i++)
                    {
                        if (i < languageFilesList.Length)
                        {
                            if (languageFilesList[i] != null)
                            {
                                if (!string.IsNullOrEmpty(languageFilesList[i]))
                                {
                                    string currentLanguage = string.Empty;
#if DEBUG
                                    Log.WriteLine("Read language file: "+languageFilesList[i]);
#endif
                                    using (FileStream fs = File.Open(ClassUtility.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + LanguageFolderName + "\\" + languageFilesList[i]), FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                                    using (BufferedStream bs = new BufferedStream(fs))
                                    using (StreamReader sr = new StreamReader(bs))
                                    {
                                        string line;
                                        while ((line = sr.ReadLine()) != null)
                                        {
                                            if (!line.Contains("#") && !string.IsNullOrEmpty(line)) // Ignore lines who contains # character.
                                            {
                                                if (line.Contains("LANGUAGE_NAME="))
                                                {
                                                    currentLanguage = line.Replace("LANGUAGE_NAME=", "").ToLower();
#if DEBUG
                                                    Log.WriteLine("Language name detected: " + currentLanguage);
#endif
                                                    if (!LanguageDatabases.ContainsKey(currentLanguage))
                                                    {
                                                        LanguageDatabases.Add(currentLanguage, new Dictionary<string, string>());
                                                    }
                                                }
                                                else if (line.Contains("CONTRIBUTOR="))
                                                {
                                                    if (!LanguageContributors.ContainsKey(currentLanguage))
                                                    {
                                                        LanguageContributors.Add(currentLanguage, new List<string>());
                                                    }
                                                    LanguageContributors[currentLanguage].Add(line.Replace("CONTRIBUTOR=", ""));
                                                }
                                                else
                                                {
                                                    if (currentLanguage != string.Empty) // Ignore lines if the current language name of the file is not found.
                                                    {
                                                        var splitLanguageText = line.Split(new[] { "=" }, StringSplitOptions.None);
                                                        var orderLanguageText = splitLanguageText[0];
                                                        var contentLanguageText = splitLanguageText[1];

                                                        // Replace commands.
                                                        contentLanguageText = contentLanguageText.Replace(CoinNameOrder, ClassConnectorSetting.CoinName);
                                                        contentLanguageText = contentLanguageText.Replace(CoinMinNameOrder, ClassConnectorSetting.CoinNameMin);
                                                        contentLanguageText = contentLanguageText.Replace("\\n", Environment.NewLine);

                                                        // Insert.
                                                        LanguageDatabases[currentLanguage].Add(orderLanguageText, contentLanguageText);

#if DEBUG
                                                        Log.WriteLine("Insert order language text: " + orderLanguageText + " with content language text: " + contentLanguageText + " for language name: " + currentLanguage);
#endif
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
#if DEBUG
                Log.WriteLine("No language folder found, please reinstall your gui wallet.");
#endif
#if WINDOWS
                ClassFormPhase.MessageBoxInterface( "No language folder found, please reinstall your gui wallet.", "No folder language found.", MessageBoxButtons.OK, MessageBoxIcon.Error);
#else
                MessageBox.Show(ClassFormPhase.WalletXiropht, "No language folder found, please reinstall your gui wallet.", "No folder language found.", MessageBoxButtons.OK, MessageBoxIcon.Error);
#endif
                Process.GetCurrentProcess().Kill();
            }
        }

        /// <summary>
        /// Return language text from order text.
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public static string GetLanguageTextFromOrder(string order)
        {
            if (LanguageDatabases.ContainsKey(CurrentLanguage))
            {
                if (LanguageDatabases[CurrentLanguage].ContainsKey(order))
                {
                    return LanguageDatabases[CurrentLanguage][order];
                }
                else
                {
                    return "LANGUAGE ORDER MISSING: "+order;
                }
            }
            return "LANGUAGE NAME MISSING: "+CurrentLanguage;
        }

        /// <summary>
        /// Change current language.
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        public static bool ChangeCurrentLanguage(string language)
        {
            language = language.ToLower();
            if (LanguageDatabases.ContainsKey(language))
            {
#if DEBUG
                Log.WriteLine("Old current language: "+CurrentLanguage+" to new language: "+language+" success.");
#endif
                CurrentLanguage = language;
                return true;
            }
#if DEBUG
            Log.WriteLine(language+" not exist.");
#endif
            return false;
        }

        /// <summary>
        /// Replace first letter by a upper letter.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string UppercaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            return char.ToUpper(s[0]) + s.Substring(1);
        }
    }
}
