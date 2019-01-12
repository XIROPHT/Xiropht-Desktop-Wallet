using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Xiropht_Wallet.Wallet
{
    public class ClassWalletTransactionAnonymityCache
    {
        private const string WalletTransactionCacheDirectory = "/Cache/";
        private const string WalletTransactionCacheFileExtension = "transaction.xirtra";
        private static bool _inClearCache;
        public static List<Tuple<string, string>> ListTransaction;

        /// <summary>
        /// Load transaction in cache.
        /// </summary>
        /// <param name="walletAddress"></param>
        /// <returns></returns>
        public static void LoadWalletCache(string walletAddress)
        {
            walletAddress += "ANONYMITY";
            if (ListTransaction != null)
            {
                ListTransaction.Clear();
            }
            else
            {
                ListTransaction = new List<Tuple<string, string>>();
            }

            if (Directory.Exists(
                ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransactionCacheDirectory + walletAddress + "/")))
            {
                if (File.Exists(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransactionCacheDirectory + walletAddress + "/"+WalletTransactionCacheFileExtension)))
                {
                    using (FileStream fs = File.Open(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransactionCacheDirectory + walletAddress + "/" + WalletTransactionCacheFileExtension), FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
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
                    File.Create(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransactionCacheDirectory + walletAddress + "/" + WalletTransactionCacheFileExtension)).Close();
                }
  
            }
            else
            {
                if (Directory.Exists(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransactionCacheDirectory)) == false)
                {
                    Directory.CreateDirectory(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransactionCacheDirectory));
                }

                Directory.CreateDirectory(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransactionCacheDirectory +
                                          walletAddress));
            }
        }

        /// <summary>
        /// Save each transaction into cache
        /// </summary>
        /// <param name="walletAddress"></param>
        public static async Task SaveWalletCache(string walletAddress, string transaction)
        {
            walletAddress += "ANONYMITY";
            if (ListTransaction.Count > 0 && _inClearCache == false)
            {
                if (Directory.Exists(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransactionCacheDirectory)) == false)
                {
                    Directory.CreateDirectory(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransactionCacheDirectory));
                }

                if (Directory.Exists(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransactionCacheDirectory +
                                     walletAddress)) == false)
                {
                    Directory.CreateDirectory(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransactionCacheDirectory +
                                              walletAddress));
                }


                if (!File.Exists(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransactionCacheDirectory +
                                walletAddress +
                                "/" + WalletTransactionCacheFileExtension)))
                {
                    File.Create(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransactionCacheDirectory + walletAddress + "/" + WalletTransactionCacheFileExtension)).Close();
                }
                try
                {

                    using (var transactionFile = new StreamWriter(ClassUtils.ConvertPath(
                        Directory.GetCurrentDirectory() + WalletTransactionCacheDirectory +
                        walletAddress +
                        "/" + WalletTransactionCacheFileExtension), true))
                    {
                        await transactionFile.WriteAsync(transaction+"\n").ConfigureAwait(false);
                    }
                }
                catch
                {
                    // ignored
                }

            }
        }

        /// <summary>
        /// Clear each transaction into cache.
        /// </summary>
        /// <param name="walletAddress"></param>
        public static void RemoveWalletCache(string walletAddress)
        {
            walletAddress += "ANONYMITY";
            _inClearCache = true;
            if (Directory.Exists(
                ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransactionCacheDirectory + walletAddress + "/")))
            {
              if (File.Exists(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransactionCacheDirectory + walletAddress + "/" + WalletTransactionCacheFileExtension)))
              {
                  File.Delete(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransactionCacheDirectory + walletAddress + "/" + WalletTransactionCacheFileExtension));
              }

                Directory.Delete(
                    ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransactionCacheDirectory + walletAddress + "/"), true);
                Directory.CreateDirectory(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransactionCacheDirectory +
                                          walletAddress));
            }

            ListTransaction.Clear();
            _inClearCache = false;
        }
    }
}