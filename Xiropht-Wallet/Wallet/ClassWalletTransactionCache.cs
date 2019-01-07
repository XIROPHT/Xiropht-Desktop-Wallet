using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xiropht_Connector_All.Utils;

namespace Xiropht_Wallet.Wallet
{
    public class ClassWalletTransactionCache
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
            if (ListTransaction != null)
            {
                ListTransaction.Clear();
            }
            else
            {
                ListTransaction = new List<Tuple<string, string>>();
            }

            if (Directory.Exists(
                Directory.GetCurrentDirectory() + WalletTransactionCacheDirectory + walletAddress + "/"))
            {
                if (File.Exists(Directory.GetCurrentDirectory() + WalletTransactionCacheDirectory + walletAddress + "/" + WalletTransactionCacheFileExtension))
                {
                    using (FileStream fs = File.Open(Directory.GetCurrentDirectory() + WalletTransactionCacheDirectory + walletAddress + "/" + WalletTransactionCacheFileExtension, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (BufferedStream bs = new BufferedStream(fs))
                    using (StreamReader sr = new StreamReader(bs))
                    {
                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            ListTransaction.Add(new Tuple<string, string>(ClassUtils.ConvertStringtoSHA512(line), line));
                        }
                    }
                }
                else
                {
                    File.Create(Directory.GetCurrentDirectory() + WalletTransactionCacheDirectory + walletAddress + "/" + WalletTransactionCacheFileExtension).Close();
                }

            }
            else
            {
                if (Directory.Exists(Directory.GetCurrentDirectory() + WalletTransactionCacheDirectory) == false)
                {
                    Directory.CreateDirectory(Directory.GetCurrentDirectory() + WalletTransactionCacheDirectory);
                }

                Directory.CreateDirectory(Directory.GetCurrentDirectory() + WalletTransactionCacheDirectory +
                                          walletAddress);
            }
        }

        /// <summary>
        /// Save each transaction into cache
        /// </summary>
        /// <param name="walletAddress"></param>
        public static async Task SaveWalletCache(string walletAddress, string transaction)
        {
            if (ListTransaction.Count > 0 && _inClearCache == false)
            {
                if (Directory.Exists(Directory.GetCurrentDirectory() + WalletTransactionCacheDirectory) == false)
                {
                    Directory.CreateDirectory(Directory.GetCurrentDirectory() + WalletTransactionCacheDirectory);
                }

                if (Directory.Exists(Directory.GetCurrentDirectory() + WalletTransactionCacheDirectory +
                                     walletAddress) == false)
                {
                    Directory.CreateDirectory(Directory.GetCurrentDirectory() + WalletTransactionCacheDirectory +
                                              walletAddress);
                }


                if (!File.Exists(Directory.GetCurrentDirectory() + WalletTransactionCacheDirectory +
                                walletAddress +
                                "/" + WalletTransactionCacheFileExtension))
                {
                    File.Create(Directory.GetCurrentDirectory() + WalletTransactionCacheDirectory + walletAddress + "/" + WalletTransactionCacheFileExtension).Close();
                }
                try
                {

                    using (var transactionFile = new StreamWriter(
                        Directory.GetCurrentDirectory() + WalletTransactionCacheDirectory +
                        walletAddress +
                        "/" + WalletTransactionCacheFileExtension, true))
                    {
                        await transactionFile.WriteAsync(transaction + "\n").ConfigureAwait(false);
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
            _inClearCache = true;
            if (Directory.Exists(
                Directory.GetCurrentDirectory() + WalletTransactionCacheDirectory + walletAddress + "/"))
            {
                if (File.Exists(Directory.GetCurrentDirectory() + WalletTransactionCacheDirectory + walletAddress + "/" + WalletTransactionCacheFileExtension))
                {
                    File.Delete(Directory.GetCurrentDirectory() + WalletTransactionCacheDirectory + walletAddress + "/" + WalletTransactionCacheFileExtension);
                }

                Directory.Delete(
                    Directory.GetCurrentDirectory() + WalletTransactionCacheDirectory + walletAddress + "/", true);
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + WalletTransactionCacheDirectory +
                                          walletAddress);
            }

            ListTransaction.Clear();
            _inClearCache = false;
        }
    }
}