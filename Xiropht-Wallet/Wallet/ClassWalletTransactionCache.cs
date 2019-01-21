using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Xiropht_Wallet.Wallet
{
    public class ClassWalletTransactionCache
    {
        private const string WalletTransMethodInvokerCacheDirectory = "/Cache/";
        private const string WalletTransMethodInvokerCacheFileExtension = "transaction.xirtra";
        private static bool _inClearCache;
        public static List<Tuple<string, string>> ListTransaction;

        /// <summary>
        /// Load transMethodInvoker in cache.
        /// </summary>
        /// <param name="walletAddress"></param>
        /// <returns></returns>
        public static void LoadWalletCache(string walletAddress)
        {
            if (ListTransaction != null)
            {
                ListTransaction.Clear();
            }
            else
            {
                ListTransaction = new List<Tuple<string, string>>();
            }

            if (Directory.Exists(
                ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransMethodInvokerCacheDirectory + walletAddress + "\\")))
            {
                if (File.Exists(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransMethodInvokerCacheDirectory + walletAddress + "\\" + WalletTransMethodInvokerCacheFileExtension)))
                {
                    using (FileStream fs = File.Open(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransMethodInvokerCacheDirectory + walletAddress + "\\" + WalletTransMethodInvokerCacheFileExtension), FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (BufferedStream bs = new BufferedStream(fs))
                    using (StreamReader sr = new StreamReader(bs))
                    {
                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            ListTransaction.Add(new Tuple<string, string>(Xiropht_Connector_All.Utils.ClassUtils.ConvertStringtoSHA512(line), line));
                        }
                    }
                }
                else
                {
                    File.Create(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransMethodInvokerCacheDirectory + walletAddress + "\\" + WalletTransMethodInvokerCacheFileExtension)).Close();
                }

            }
            else
            {
                if (Directory.Exists(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransMethodInvokerCacheDirectory)) == false)
                {
                    Directory.CreateDirectory(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransMethodInvokerCacheDirectory));
                }

                Directory.CreateDirectory(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransMethodInvokerCacheDirectory +
                                          walletAddress));
            }
        }

        /// <summary>
        /// Save each transMethodInvoker into cache
        /// </summary>
        /// <param name="walletAddress"></param>
        public static async Task SaveWalletCache(string walletAddress, string transMethodInvoker)
        {
            if (ListTransaction.Count > 0 && _inClearCache == false)
            {
                if (Directory.Exists(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransMethodInvokerCacheDirectory)) == false)
                {
                    Directory.CreateDirectory(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransMethodInvokerCacheDirectory));
                }

                if (Directory.Exists(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransMethodInvokerCacheDirectory +
                                     walletAddress)) == false)
                {
                    Directory.CreateDirectory(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransMethodInvokerCacheDirectory +
                                              walletAddress));
                }


                if (!File.Exists(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransMethodInvokerCacheDirectory +
                                walletAddress +
                                "\\" + WalletTransMethodInvokerCacheFileExtension)))
                {
                    File.Create(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransMethodInvokerCacheDirectory + walletAddress + "\\" + WalletTransMethodInvokerCacheFileExtension)).Close();
                }
                try
                {

                    using (var transMethodInvokerFile = new StreamWriter(ClassUtils.ConvertPath(
                        Directory.GetCurrentDirectory() + WalletTransMethodInvokerCacheDirectory +
                        walletAddress +
                        "\\" + WalletTransMethodInvokerCacheFileExtension), true))
                    {
                        await transMethodInvokerFile.WriteAsync(transMethodInvoker + "\n").ConfigureAwait(false);
                    }
                }
                catch
                {
                    // ignored
                }

            }
        }

        /// <summary>
        /// Clear each transMethodInvoker into cache.
        /// </summary>
        /// <param name="walletAddress"></param>
        public static void RemoveWalletCache(string walletAddress)
        {
            _inClearCache = true;
            if (Directory.Exists(
                ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransMethodInvokerCacheDirectory + walletAddress + "\\")))
            {
                if (File.Exists(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransMethodInvokerCacheDirectory + walletAddress + "\\" + WalletTransMethodInvokerCacheFileExtension)))
                {
                    File.Delete(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransMethodInvokerCacheDirectory + walletAddress + "\\" + WalletTransMethodInvokerCacheFileExtension));
                }

                Directory.Delete(
                    ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransMethodInvokerCacheDirectory + walletAddress + "\\"), true);
                Directory.CreateDirectory(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransMethodInvokerCacheDirectory +
                                          walletAddress));
            }

            ListTransaction.Clear();
            _inClearCache = false;
        }
    }
}