using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Xiropht_Wallet.Wallet
{
    public class ClassBlockCache
    {
        private const string WalletBlockCacheDirectory = "/Blockchain/";
        private const string WalletBlockCacheFileExtension = ".xirblock";
        public static List<string> ListBlock;

        /// <summary>
        /// Load block in cache.
        /// </summary>
        /// <returns></returns>
        public static void LoadBlockchainCache()
        {
            if (ListBlock != null)
            {
                ListBlock.Clear();
            }
            else
            {
                ListBlock = new List<string>();
            }

            if (Directory.Exists(Directory.GetCurrentDirectory() + WalletBlockCacheDirectory + "/"))
            {
                if (
                    File.Exists(Directory.GetCurrentDirectory() + WalletBlockCacheDirectory +
                                "/blockchain" + WalletBlockCacheFileExtension))
                {

                    int counter = 0;
                    using (FileStream fs = File.Open(Directory.GetCurrentDirectory() + WalletBlockCacheDirectory +
                                                     "/blockchain" + WalletBlockCacheFileExtension, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (BufferedStream bs = new BufferedStream(fs))
                    using (StreamReader sr = new StreamReader(bs))
                    {
                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            ListBlock.Add(line);
                            counter++;
                        }
                    }
                }
            }
            else
            {
                if (Directory.Exists(Directory.GetCurrentDirectory() + WalletBlockCacheDirectory) == false)
                {
                    Directory.CreateDirectory(Directory.GetCurrentDirectory() + WalletBlockCacheDirectory);
                }

                Directory.CreateDirectory(Directory.GetCurrentDirectory() + WalletBlockCacheDirectory);
            }
        }

        /// <summary>
        /// Save each block into cache
        /// </summary>
        /// <param name="block"></param>
        public static async Task SaveWalletBlockCache(string block)
        {
            if (Directory.Exists(Directory.GetCurrentDirectory() + WalletBlockCacheDirectory) == false)
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + WalletBlockCacheDirectory);
            }
            if (File.Exists(Directory.GetCurrentDirectory() + WalletBlockCacheDirectory +
                            "/blockchain" + WalletBlockCacheFileExtension) == false)
            {
                try
                {
                    File.Create(Directory.GetCurrentDirectory() + WalletBlockCacheDirectory +
                                "/blockchain" + WalletBlockCacheFileExtension).Close();

                    using (var transactionFile = new StreamWriter(Directory.GetCurrentDirectory() + WalletBlockCacheDirectory +
                                                           "/blockchain" + WalletBlockCacheFileExtension, true))
                    {
                        await transactionFile.WriteAsync(block + "\n").ConfigureAwait(false);
                    }
                }
                catch
                {
                    // ignored
                }
            }
            else
            {
                try
                {
                    using (var transactionFile = new StreamWriter(Directory.GetCurrentDirectory() + WalletBlockCacheDirectory +
                                                           "/blockchain" + WalletBlockCacheFileExtension, true))
                    {
                        await transactionFile.WriteAsync(block + "\n").ConfigureAwait(false);
                    }
                }
                catch
                {
                    //
                }
            }
        }

        /// <summary>
        /// Clear each block into cache.
        /// </summary>
        public static void RemoveWalletBlockCache()
        {
            try
            {
                if (Directory.Exists(Directory.GetCurrentDirectory() + WalletBlockCacheDirectory + "/"))
                {
                    File.Delete(Directory.GetCurrentDirectory() + WalletBlockCacheDirectory +
                                "/blockchain." + WalletBlockCacheFileExtension);
                    Directory.Delete(Directory.GetCurrentDirectory() + WalletBlockCacheDirectory + "/", true);
                    Directory.CreateDirectory(Directory.GetCurrentDirectory() + WalletBlockCacheDirectory);
                }
            }
            catch
            {
                //
            }

            ListBlock.Clear();
        }
    }
}